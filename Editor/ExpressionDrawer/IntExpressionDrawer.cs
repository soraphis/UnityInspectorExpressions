using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(IntExpression<>))]
    public class IntExpressionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var numericExpression = property.FindPropertyRelative("m_NumericExpression");
            var field = new PropertyField(numericExpression, preferredLabel);
            field.style.flexGrow = 1;
            field.style.flexShrink = 1;
            return field;
        }
    }
}