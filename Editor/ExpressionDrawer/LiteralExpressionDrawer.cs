using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(LiteralBoolExpression))]
    public class BoolLiteralExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_Literal";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var literalProp = property.FindPropertyRelative(s_PropertyName);

            if (GUI.Button(position, new GUIContent(literalProp.boolValue.ToString())))
            {
                literalProp.boolValue = !literalProp.boolValue;
            }
        }
    }

    [CustomPropertyDrawer(typeof(LiteralFloatExpression))]
    [CustomPropertyDrawer(typeof(LiteralIntExpression))]
    [CustomPropertyDrawer(typeof(LiteralComponentExpression))]
    [CustomPropertyDrawer(typeof(LiteralGameObjectExpression))]
    public class LiteralExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_Literal";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var literalProp = property.FindPropertyRelative(s_PropertyName);
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, literalProp, GUIContent.none);
        }
    }
}
