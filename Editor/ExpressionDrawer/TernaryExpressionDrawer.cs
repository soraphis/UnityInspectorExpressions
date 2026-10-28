using System;
using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{

    [CustomPropertyDrawer(typeof(TernaryNumericExpression<>))]
    public class TernaryNumericExpressionDrawer : PropertyDrawer
    {
        const string s_InnerProperty1Name   = "m_InnerExpr1";
        const string s_InnerProperty2Name   = "m_InnerExpr2";
        const string s_InnerProperty3Name   = "m_InnerExpr3";
        const string s_OperatorPropertyName = "m_Operator";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var expr1Prop = property.FindPropertyRelative(s_InnerProperty1Name);
            var expr2Prop = property.FindPropertyRelative(s_InnerProperty2Name);
            var expr3Prop = property.FindPropertyRelative(s_InnerProperty3Name);
            var operProp  = property.FindPropertyRelative(s_OperatorPropertyName);

            var root = new VisualElement();
            root.style.flexGrow = 1;
            root.style.color    = Color.white;

            void Rebuild()
            {
                root.Clear();
                var so           = property.serializedObject;
                var positioning  = GetPositioningInfo(property, operProp);
                bool isFunction  = positioning != null && positioning.m_Position == BinaryOperatorPosition.FunctionCall;

                var row = CustomStyles.MakeHighlightableRow();

                var inner1 = new PropertyField(expr1Prop, "");
                inner1.style.flexGrow   = 1;
                inner1.style.flexShrink = 1;
                inner1.Bind(so);

                var inner2 = new PropertyField(expr2Prop, "");
                inner2.style.flexGrow   = 1;
                inner2.style.flexShrink = 1;
                inner2.Bind(so);

                var inner3 = new PropertyField(expr3Prop, "");
                inner3.style.flexGrow   = 1;
                inner3.style.flexShrink = 1;
                inner3.Bind(so);

                var opField = new PropertyField(operProp, "");
                opField.style.flexShrink = 0;
                opField.style.flexGrow   = 1;
                var cachedVal = operProp.enumValueIndex;
                opField.RegisterValueChangeCallback(_ =>
                {
                    if (cachedVal != operProp.enumValueIndex)
                    {
                        cachedVal = operProp.enumValueIndex;
                        Rebuild();
                    }
                });
                opField.Bind(so);
                OperatorDocs.Apply(property, operProp, opField, inner1, inner2, inner3);

                if (isFunction)
                {
                    // op(a, b, c)
                    opField.style.minWidth = 60;
                    opField.style.maxWidth = 180;

                    row.Add(opField);
                    row.Add(CustomStyles.MakeLabel("("));
                    row.Add(inner1);
                    row.Add(CustomStyles.MakeLabel(","));
                    row.Add(inner2);
                    row.Add(CustomStyles.MakeLabel(","));
                    row.Add(inner3);
                    row.Add(CustomStyles.MakeLabel(")"));
                }
                else
                {
                    // (a op b)
                    opField.style.minWidth = 30;
                    opField.style.maxWidth = 60;

                    row.Add(CustomStyles.MakeLabel("("));
                    row.Add(inner1);
                    row.Add(opField);
                    row.Add(inner2);
                    row.Add(CustomStyles.MakeLabel(")"));
                }

                root.Add(row);
            }

            Rebuild();
            root.TrackPropertyValue(operProp, _ => Rebuild());
            return root;
        }


        private BinaryOperatorPositionAttribute GetPositioningInfo(SerializedProperty property, SerializedProperty operProp)
        {
            var declaringtype = property.managedReferenceValue?.GetType();
            if (declaringtype == null) return null;
            var opField = declaringtype.GetField(s_OperatorPropertyName,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public   |
                System.Reflection.BindingFlags.NonPublic);
            if (opField == null) return null;
            var type = opField.FieldType;
            var memberName = Enum.GetName(type, operProp.intValue);
            if (memberName == null) return null;
            var memInfo = type.GetMember(memberName);
            if (memInfo.Length == 0) return null;
            var attributes = memInfo[0].GetCustomAttributes(typeof(BinaryOperatorPositionAttribute), false);
            return (attributes.Length > 0) ? (BinaryOperatorPositionAttribute)attributes[0] : null;
        }
    }

}
