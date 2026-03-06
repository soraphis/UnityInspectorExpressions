using System;
using System.Linq;
using Editor.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityInspectorExpressions.Expressions.Base;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(BoolResultFunctionExpression<,>))]
    public class FunctionExpressionDrawer : PropertyDrawer
    {
        const string s_ObjectPropertyName = "m_Object";
        const string s_ArgumentPropertyName = "m_Argument";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var objProp = property.FindPropertyRelative(s_ObjectPropertyName);
            var argProp = property.FindPropertyRelative(s_ArgumentPropertyName);

            // // Get function name, from type attribute "ExpressionLabel":

            
            
            var typename = property.type.SubstringBetween('<', '>').ToString();
            var fieldType = TypeCache
                .GetTypesWithAttribute(typeof(ExpressionLabelAttribute))
                .FirstOrDefault(t => t.Name == typename);
                
            var labelAttr = fieldType?.GetCustomAttributes(typeof(ExpressionLabelAttribute), false).FirstOrDefault() as ExpressionLabelAttribute;
            var functionName = labelAttr?.Label?.Split(new[] { "Fn:" }, System.StringSplitOptions.None).LastOrDefault()?.Trim() ?? "FN_NAME";
            
            
            
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexGrow = 1;
            
            row.Add(new PropertyField(objProp, "").WithFlex(1, 1));
            row.Add(MakeLabel("."));
            row.Add(MakeLabel(functionName).WithFontSize(12));
            row.Add(MakeLabel("("));
            row.Add(new PropertyField(argProp, "").WithFlex(1, 1));
            row.Add(MakeLabel(")"));
            return row;
        }
        
        static Label MakeLabel(string text)
        {
            var lbl = new Label(text);
            lbl.style.flexShrink = 0;
            lbl.style.paddingLeft = 2;
            lbl.style.paddingRight = 2;
            lbl.style.height = 20;
            lbl.style.fontSize = 20;
            lbl.style.unityTextAlign = TextAnchor.MiddleCenter;
            return lbl;
        }
    }
}