using System;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(UnaryBoolExpression))]
    [CustomPropertyDrawer(typeof(UnaryFloatExpression))]
    [CustomPropertyDrawer(typeof(UnaryIntExpression))]
    public class UnaryExpressionDrawer : PropertyDrawer
    {
        const string s_InnerPropertyName = "m_InnerExpr";
        const string s_OperatorPropertyName = "m_Operator";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var exprProp = property.FindPropertyRelative(s_InnerPropertyName);
            var operProp = property.FindPropertyRelative(s_OperatorPropertyName);

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;
            row.style.flexGrow = 1;

            var opField = new PropertyField(operProp, "") { style = { minWidth = 40, maxWidth = 60 } };
            opField.style.flexShrink = 0;

            var lblOpen = MakeLabel("(");

            var innerField = new PropertyField(exprProp, "");
            innerField.style.flexGrow = 1;
            innerField.style.flexShrink = 1;

            var lblClose = MakeLabel(")");

            row.Add(opField);
            row.Add(lblOpen);
            row.Add(innerField);
            row.Add(lblClose);
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
