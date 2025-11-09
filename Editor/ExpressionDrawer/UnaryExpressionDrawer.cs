using System;
using Editor.Helpers;
using Project.Expressions.Base;
using UnityEditor;
using UnityEngine;

namespace Project.Expressions
{
    [CustomPropertyDrawer(typeof(UnaryBoolExpression))]
    [CustomPropertyDrawer(typeof(UnaryFloatExpression))]
    [CustomPropertyDrawer(typeof(UnaryIntExpression))]
    public class UnaryExpressionDrawer : PropertyDrawer
    {
        const string s_InnerPropertyName = "m_InnerExpr";
        const string s_OperatorPropertyName = "m_Operator";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var exprProp = property.FindPropertyRelative(s_InnerPropertyName);
            var operProp = property.FindPropertyRelative(s_OperatorPropertyName);

            Span<Rect> rects = stackalloc Rect[4];
            using (var x = position.Row(rects))
            {
                x.Container(40);
                x.Container(8);
                x.Flex(1, 60);
                x.Container(8);
            }

            EditorGUI.PropertyField(rects[0], operProp, GUIContent.none);
            GUI.Label(rects[1].Shift(0, -1), "(");
            EditorGUI.PropertyField(rects[2], exprProp, GUIContent.none);
            GUI.Label(rects[3].Shift(0, -1), ")");
        }
    }
}
