using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(UnaryBoolExpression))]
    [CustomPropertyDrawer(typeof(UnaryFloatExpression))]
    [CustomPropertyDrawer(typeof(UnaryIntExpression))]
    public class UnaryExpressionDrawer : PropertyDrawer
    {
        const string s_InnerPropertyName    = "m_InnerExpr";
        const string s_OperatorPropertyName = "m_Operator";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var exprProp = property.FindPropertyRelative(s_InnerPropertyName);
            var operProp = property.FindPropertyRelative(s_OperatorPropertyName);

            var row = CustomStyles.MakeRow();

            var opField = new PropertyField(operProp, "");
            opField.style.flexShrink = 0;
            opField.style.minWidth   = 40;
            opField.style.maxWidth   = 60;

            var innerField = new PropertyField(exprProp, "");
            innerField.style.flexGrow   = 1;
            innerField.style.flexShrink = 1;

            row.Add(opField);
            row.Add(CustomStyles.MakeLabel("("));
            row.Add(innerField);
            row.Add(CustomStyles.MakeLabel(")"));
            return row;
        }
    }
}
