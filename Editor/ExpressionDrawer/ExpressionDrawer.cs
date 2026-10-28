using System;
using System.Reflection;
using Editor.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(BoolExpression<>))]
    [CustomPropertyDrawer(typeof(FloatExpression<>))]
    [CustomPropertyDrawer(typeof(IntExpression<>))]
    [CustomPropertyDrawer(typeof(GenericExpression<,>))]
    [CustomPropertyDrawer(typeof(Vector2Expression<>))]
    [CustomPropertyDrawer(typeof(Vector3Expression<>))]
    [CustomPropertyDrawer(typeof(ComponentExpression<>))]
    [CustomPropertyDrawer(typeof(GameObjectExpression<>))]
    [CustomPropertyDrawer(typeof(StringExpression<>))]
    public class ExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_ExpressionRef";

        private static string s_ClipboardTypename;
        private static string s_ClipboardJson;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var conditionProp = property.FindPropertyRelative(s_PropertyName);

            // TODO: ensure that conditionProp's values' type is assignable to conditionProps type. E.g. if the context-generic type changes during development unity will not reset this property!
            if (conditionProp.managedReferenceValue != null)
            {
                var currentValuesType = conditionProp.managedReferenceValue.GetType();
                var currentValuesField = ManagedReferenceUtility.GetType(conditionProp.managedReferenceFieldTypename);
                if (currentValuesField != null)
                {
                    if (!currentValuesField.IsAssignableFrom(currentValuesType))
                    {
                        Debug.LogWarning($"ExpressionDrawer ({conditionProp.serializedObject.targetObject.name}): The serialized property '{conditionProp.name}' is of type '{currentValuesType}', which is not assignable to the expected type '{currentValuesField}'. Resetting the property to null.");
                        conditionProp.managedReferenceValue = null;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }

            // -- outer row -------------------------------------------------
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems    = Align.FlexStart;
            root.style.flexGrow      = 1;

            root.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Copy", _ => CopyProperty(conditionProp));
                var canPaste = !string.IsNullOrEmpty(s_ClipboardJson) &&
                               !string.IsNullOrEmpty(s_ClipboardTypename) &&
                               conditionProp.managedReferenceFieldTypename != null &&
                               CanPasteInto(conditionProp);
                evt.menu.AppendAction("Paste",
                    _ => PasteProperty(conditionProp),
                    canPaste ? DropdownMenuAction.AlwaysEnabled : DropdownMenuAction.AlwaysDisabled);
            }));

            // -- label -----------------------------------------------------
            var labelText = preferredLabel;
            if (!string.IsNullOrEmpty(labelText))
            {
                var lbl = new Label(labelText);
                lbl.style.flexShrink     = 0;
                lbl.style.height         = Length.Percent(100);
                lbl.style.minWidth       = 100;
                lbl.style.maxWidth       = 200;
                lbl.style.paddingRight   = 4;
                lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
                root.Add(lbl);
            }


            // -- "…" type-selector button ----------------------------------
            var moreBtn = CustomStyles.MakeIconButton("d_more", 16);
            moreBtn.style.marginRight = 2;

            moreBtn.clicked += () =>
            {
                // fieldInfo.FieldType is the closed generic, e.g. BoolExpression<TriggerCtx>
                // → extract TCtx so the type-selector can close open-generic derived types
                var ctxArgs = fieldInfo.FieldType.IsGenericType
                    ? fieldInfo.FieldType.GetGenericArguments()
                    : null;

                var menu = ManagedReferenceDrawerHelper.TypeSelectorMenu(conditionProp, new Rect(), ctxArgs);
                menu.AddSeparator("");

                var wrappables = ReflectionHelper.GetAllGenericImplementationsTypes(
                    conditionProp.managedReferenceValue, typeof(IWrapable<>));

                foreach (var wrappableType in wrappables)
                {
                    var scopedType = wrappableType;
                    menu.AddItem(new GUIContent("Wrap with " + scopedType.Name), false, () =>
                    {
                        var method = typeof(IWrapable<>).MakeGenericType(scopedType).GetMethod("Wrap");
                        conditionProp.managedReferenceValue =
                            method.Invoke(conditionProp.managedReferenceValue, Array.Empty<object>());
                        conditionProp.serializedObject.ApplyModifiedProperties();
                    });
                }

                menu.ShowAsContext();
            };

            root.Add(moreBtn);

            // -- inner content (null hint or actual PropertyField) ---------
            var content = new VisualElement();
            content.style.flexGrow   = 1;
            content.style.flexShrink = 1;
            root.Add(content);

            void RebuildContent()
            {
                content.Clear();
                if (conditionProp.managedReferenceValue == null)
                {
                    var helpBox = new HelpBox("null", HelpBoxMessageType.Info);
                    helpBox.style.flexGrow = 1;
                    content.Add(helpBox);
                }
                else
                {
                    var field = new PropertyField(conditionProp, "");
                    field.style.flexGrow = 1;
                    content.Add(field);
                    field.Bind(conditionProp.serializedObject);
                }
            }

            RebuildContent();

            // Only rebuild when the managed reference itself changes (type swap / null / wrap).
            root.TrackPropertyValue(conditionProp, _ => RebuildContent());

            return root;
        }

        private static void CopyProperty (SerializedProperty property)
        {
            var value = property.managedReferenceValue;
            if (value == null)
            {
                s_ClipboardTypename = string.Empty;
                s_ClipboardJson = string.Empty;
                return;
            }

            s_ClipboardTypename = property.managedReferenceFullTypename;
            s_ClipboardJson = JsonUtility.ToJson(value);
        }

        private static bool CanPasteInto (SerializedProperty property)
        {
            if (string.IsNullOrEmpty(s_ClipboardTypename)) return false;
            var targetBaseType = ManagedReferenceUtility.GetType(property.managedReferenceFieldTypename);
            var clipType = ManagedReferenceUtility.GetType(s_ClipboardTypename);
            return targetBaseType != null && clipType != null && targetBaseType.IsAssignableFrom(clipType);
        }

        private static void PasteProperty (SerializedProperty property)
        {
            if (!CanPasteInto(property)) return;

            var clipType = ManagedReferenceUtility.GetType(s_ClipboardTypename);
            var instance = System.Activator.CreateInstance(clipType);
            JsonUtility.FromJsonOverwrite(s_ClipboardJson, instance);

            Undo.RecordObject(property.serializedObject.targetObject, "Paste SerializeReference");
            property.managedReferenceValue = instance;
            property.isExpanded = true;
            property.serializedObject.ApplyModifiedProperties();
        }


        internal static class ManagedReferenceUtility
        {

            public static Type GetType (string typeName)
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    return null;
                }

                var (assemblyName, className) = ManagedReferenceDrawerHelper.GetSplitNamesFromTypename(typeName);
                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(className))
                    return null;

                return Type.GetType($"{className}, {assemblyName}");
            }

        }
    }
}
