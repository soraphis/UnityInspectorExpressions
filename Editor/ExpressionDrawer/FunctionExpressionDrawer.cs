using System.Linq;
using Editor.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityInspectorExpressions.Expressions.Base;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(BoolResultFunctionExpression<,>))]
    
    [CustomPropertyDrawer(typeof(IntResultFunctionExpression<>))]
    [CustomPropertyDrawer(typeof(IntResultFunctionExpression<,>))]
    public class FunctionExpressionDrawer : PropertyDrawer
    {
        const string s_ObjectPropertyName   = "m_Object";
        const string s_ArgumentPropertyName = "m_Argument";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var objProp = property.FindPropertyRelative(s_ObjectPropertyName);
            var argProp = property.FindPropertyRelative(s_ArgumentPropertyName);

            var typename  = property.type.SubstringBetween('<', '>').ToString();
            var fieldType = TypeCache
                .GetTypesWithAttribute(typeof(ExpressionLabelAttribute))
                .FirstOrDefault(t => t.Name == typename);

            var labelAttr    = fieldType?.GetCustomAttributes(typeof(ExpressionLabelAttribute), false).FirstOrDefault() as ExpressionLabelAttribute;
            var functionName = labelAttr?.Label?.Split(new[] { "Fn:" }, System.StringSplitOptions.None).LastOrDefault()?.Trim() ?? "FN_NAME";

            var row = CustomStyles.MakeRow();
            row.Add(new PropertyField(objProp, "").WithFlex(1, 1));
            row.Add(CustomStyles.MakeLabel("."));
            row.Add(CustomStyles.MakeLabel(functionName).WithFontSize(12));
            row.Add(CustomStyles.MakeLabel("("));
            if(argProp != null) row.Add(new PropertyField(argProp, "").WithFlex(1, 1));
            row.Add(CustomStyles.MakeLabel(")"));
            return row;
        }
    }
}