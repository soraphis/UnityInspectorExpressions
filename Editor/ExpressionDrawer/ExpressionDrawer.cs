using System;
using Editor.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(BoolExpression))]
    [CustomPropertyDrawer(typeof(FloatExpression))]
    [CustomPropertyDrawer(typeof(IntExpression))]
    [CustomPropertyDrawer(typeof(Vector2Expression))]
    [CustomPropertyDrawer(typeof(Vector3Expression))]
    [CustomPropertyDrawer(typeof(ComponentExpression))]
    [CustomPropertyDrawer(typeof(GameObjectExpression))]
    [CustomPropertyDrawer(typeof(StringExpression))]
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
                lbl.style.flexShrink    = 0;
                lbl.style.height        = Length.Percent(100);
                lbl.style.minWidth      = 100;
                lbl.style.maxWidth      = 200;
                lbl.style.paddingRight  = 4;
                lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
                root.Add(lbl);
            }

            // ── "…" type-selector button ──────────────────────────────────
            var moreBtn = new Button();
            moreBtn.style.flexShrink    = 0;
            moreBtn.style.width         = 16; 
            moreBtn.style.height        = 20;
            moreBtn.style.paddingLeft   = 0;
            moreBtn.style.paddingRight  = 0;
            moreBtn.style.paddingTop    = 0;
            moreBtn.style.paddingBottom = 0;
            moreBtn.style.marginRight   = 2;
            moreBtn.Add(new Image
            {
                image     = EditorGUIUtility.IconContent("d_more").image as Texture2D,
                scaleMode = ScaleMode.ScaleToFit,
                style     = { flexGrow = 1 }
            });

            moreBtn.clicked += () =>
            {
                var menu = ManagedReferenceDrawerHelper.TypeSelectorMenu(conditionProp, new Rect());
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
            // TrackPropertyValue on a [SerializeReference] field fires when the reference is
            // reassigned, but NOT on simple value-changes inside the referenced object, so the
            // bool-toggle button (and any other child field) won't get torn down on every edit.
            root.TrackPropertyValue(conditionProp, _ => RebuildContent());

            return root;
        }
    }
}
