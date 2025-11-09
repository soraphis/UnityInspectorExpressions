using System;
using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(BinaryBoolExpression))]
    [CustomPropertyDrawer(typeof(GameObjectRelationalExpression))]
    [CustomPropertyDrawer(typeof(FloatRelationalExpression))]
    [CustomPropertyDrawer(typeof(IntRelationalExpression))]
    public class BinaryBoolExpressionDrawer : PropertyDrawer
    {
        const string s_InnerProperty1Name = "m_InnerExpr1";
        const string s_InnerProperty2Name = "m_InnerExpr2";
        const string s_OperatorPropertyName = "m_Operator";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var expr1Prop = property.FindPropertyRelative(s_InnerProperty1Name);
            var expr2Prop = property.FindPropertyRelative(s_InnerProperty2Name);

            return Mathf.Max(EditorGUIUtility.singleLineHeight, Mathf.Max(EditorGUI.GetPropertyHeight(expr1Prop), EditorGUI.GetPropertyHeight(expr2Prop)));
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var expr1Prop = property.FindPropertyRelative(s_InnerProperty1Name);
            var expr2Prop = property.FindPropertyRelative(s_InnerProperty2Name);
            var operProp = property.FindPropertyRelative(s_OperatorPropertyName);

            position.height = EditorGUIUtility.singleLineHeight;
            var rects = new SpanQueue<Rect>(stackalloc Rect[5]);
            using (var x = position.Row(rects))
            {
                x.Container(8);
                x.Flex(1, 60);
                x.Container(60);
                x.Flex(1, 60);
                x.Container(8);
            }

            GUI.Label(rects.Next().Shift(0, -1), "(");
            EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(expr1Prop)), expr1Prop, GUIContent.none);
            EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(operProp)), operProp, GUIContent.none);
            EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(expr2Prop)), expr2Prop, GUIContent.none);
            GUI.Label(rects.Next().Shift(0, -1), ")");
        }
    }


    [CustomPropertyDrawer(typeof(BinaryFloatExpression))]
    [CustomPropertyDrawer(typeof(BinaryIntExpression))]
    public class BinaryFloatExpressionDrawer : PropertyDrawer
    {
        const string s_InnerProperty1Name = "m_InnerExpr1";
        const string s_InnerProperty2Name = "m_InnerExpr2";
        const string s_OperatorPropertyName = "m_Operator";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var expr1Prop = property.FindPropertyRelative(s_InnerProperty1Name);
            var expr2Prop = property.FindPropertyRelative(s_InnerProperty2Name);

            return Mathf.Max(EditorGUIUtility.singleLineHeight, Mathf.Max(EditorGUI.GetPropertyHeight(expr1Prop), EditorGUI.GetPropertyHeight(expr2Prop)));
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var expr1Prop = property.FindPropertyRelative(s_InnerProperty1Name);
            var expr2Prop = property.FindPropertyRelative(s_InnerProperty2Name);
            var operProp = property.FindPropertyRelative(s_OperatorPropertyName);

            var countConditionChilds = 0;
            var countResultChilds = 0;
            foreach (var child in expr1Prop.Copy()) { countConditionChilds++; }
            foreach (var child in expr2Prop.Copy()) { countResultChilds++; }


            var positioning = GetPositioningInfo(property, operProp);

            position.height = EditorGUIUtility.singleLineHeight;

            if (positioning != null && positioning.m_Position == BinaryOperatorPosition.FunctionCall)
            {
                var rects = new SpanQueue<Rect>(stackalloc Rect[6]);
                using (var x = position.Row(rects))
                {
                    x.Container(60);
                    x.Container(8);
                    x.Flex(countConditionChilds, 60);
                    x.Container(8);
                    x.Flex(countResultChilds, 60);
                    x.Container(8);
                }

                EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(operProp)), operProp, GUIContent.none);
                GUI.Label(rects.Next().Shift(0, -1), "(");
                EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(expr1Prop)), expr1Prop, GUIContent.none);
                GUI.Label(rects.Next().Shift(0, -1), ",");
                EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(expr2Prop)), expr2Prop, GUIContent.none);
                GUI.Label(rects.Next().Shift(0, -1), ")");
            }
            else
            {
                var rects = new SpanQueue<Rect>(stackalloc Rect[5]);
                using (var x = position.Row(rects))
                {
                    x.Container(8);
                    x.Flex(countConditionChilds, 60);
                    x.Container(30);
                    x.Flex(countResultChilds, 60);
                    x.Container(8);
                };

                GUI.Label(rects.Next().Shift(0, -1), "(");
                EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(expr1Prop)), expr1Prop, GUIContent.none);
                EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(operProp)), operProp, GUIContent.none);
                EditorGUI.PropertyField(rects.Next().WithHeight(EditorGUI.GetPropertyHeight(expr2Prop)), expr2Prop, GUIContent.none);
                GUI.Label(rects.Next().Shift(0, -1), ")");
            }
        }

        private BinaryOperatorPositionAttribute GetPositioningInfo(SerializedProperty property, SerializedProperty operProp)
        {
            var declaringtype = property.managedReferenceValue.GetType();
            var opField = declaringtype.GetField(s_OperatorPropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            var type = opField.FieldType;
            var memInfo = type.GetMember(Enum.GetName(type, operProp.intValue));
            var attributes = memInfo[0].GetCustomAttributes(typeof(BinaryOperatorPositionAttribute), false);
            return (attributes.Length > 0) ? (BinaryOperatorPositionAttribute)attributes[0] : null;
        }
    }


}
