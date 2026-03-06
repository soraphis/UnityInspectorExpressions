using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(LiteralBoolExpression<>))]
    public class BoolLiteralExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_Literal";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var literalProp = property.FindPropertyRelative(s_PropertyName);

            var btn = new Button(() =>
            {
                literalProp.boolValue = !literalProp.boolValue;
                literalProp.serializedObject.ApplyModifiedProperties();
            });
            btn.style.flexGrow = 1;
            btn.style.flexShrink = 1;

            // keep the button label in sync
            void Refresh() => btn.text = literalProp.boolValue.ToString();
            Refresh();
            btn.TrackPropertyValue(literalProp, _ => Refresh());

            return btn;
        }
    }

    [CustomPropertyDrawer(typeof(LiteralFloatExpression<>))]
    [CustomPropertyDrawer(typeof(LiteralIntExpression<>))]
    [CustomPropertyDrawer(typeof(LiteralComponentExpression<>))]
    [CustomPropertyDrawer(typeof(LiteralGameObjectExpression<>))]
    [CustomPropertyDrawer(typeof(LiteralStringExpression<>))]
    public class LiteralExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_Literal";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var literalProp = property.FindPropertyRelative(s_PropertyName);
            var field = new PropertyField(literalProp, "");
            field.style.flexGrow = 1;
            field.style.flexShrink = 1;
            return field;
        }
    }
}
