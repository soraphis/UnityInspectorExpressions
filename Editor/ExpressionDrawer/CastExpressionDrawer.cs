using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(IntToFloatCastExpression))]
    [CustomPropertyDrawer(typeof(FloatToIntCastExpression))]
    public class CastExpressionDrawer : PropertyDrawer
    {
        const string s_InnerPropertyName = "m_InnerExpr";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var exprProp  = property.FindPropertyRelative(s_InnerPropertyName);
            var typename  = fieldInfo.FieldType.Name;
            var castName  = typename.Substring(0, typename.Length - "ExpressionBase".Length);

            var row = CustomStyles.MakeRow();

            var lbl = new Label($"({castName.ToLower()})");
            lbl.style.flexShrink = 0;
            lbl.style.minWidth   = 40;
            lbl.style.paddingRight = 2;

            row.Add(lbl);
            row.Add(new PropertyField(exprProp, "").WithFlex(1, 1));
            return row;
        }
    }

    [CustomPropertyDrawer(typeof(ComponentToGameObjectExpression))]
    public class PropertyAccessExpressionDrawer : PropertyDrawer
    {
        const string s_InnerPropertyName = "m_InnerExpr";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var exprProp = property.FindPropertyRelative(s_InnerPropertyName);
            var typename = fieldInfo.FieldType.Name;
            var castName = typename.Substring(0, typename.Length - "ExpressionBase".Length);

            var row = CustomStyles.MakeRow();

            var lbl = new Label($".{castName.ToLower()}");
            lbl.style.flexShrink  = 0;
            lbl.style.minWidth    = 40;
            lbl.style.paddingLeft = 2;

            row.Add(new PropertyField(exprProp, "").WithFlex(1, 1));
            row.Add(lbl);
            return row;
        }
    }
}
