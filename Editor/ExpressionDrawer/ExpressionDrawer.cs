using System;
using Editor.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(BoolExpression<>))]
    [CustomPropertyDrawer(typeof(FloatExpression<>))]
    [CustomPropertyDrawer(typeof(IntExpression<>))]
    [CustomPropertyDrawer(typeof(Vector2Expression<>))]
    [CustomPropertyDrawer(typeof(Vector3Expression<>))]
    [CustomPropertyDrawer(typeof(ComponentExpression<>))]
    [CustomPropertyDrawer(typeof(GameObjectExpression<>))]
    [CustomPropertyDrawer(typeof(StringExpression<>))]
    public class ExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_ExpressionRef";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var conditionProp = property.FindPropertyRelative(s_PropertyName);

            // ── outer row ─────────────────────────────────────────────────
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems    = Align.FlexStart;
            root.style.flexGrow      = 1;

            // ── label ─────────────────────────────────────────────────────
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

            // ── "…" type-selector button ──────────────────────────────────
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

            // ── inner content (null hint or actual PropertyField) ─────────
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
    }
}
