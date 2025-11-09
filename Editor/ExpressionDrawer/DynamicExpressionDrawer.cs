using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[CustomPropertyDrawer(typeof(DynamicGameObjectExpression))]
	public class DynamicExpressionDrawer : PropertyDrawer
	{
		const string s_PropertyName = "m_DynamicSlot";

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var literalProp = property.FindPropertyRelative(s_PropertyName);

			position.height = EditorGUIUtility.singleLineHeight;
			var rects = new SpanQueue<Rect>(stackalloc Rect[2]);
			using (var x = position.Row(rects))
			{
				x.Container(50);
				x.Flex(1, 50);
			}

			EditorGUI.LabelField(rects.Next(), "dyn:");
			EditorGUI.PropertyField(rects.Next(), literalProp, GUIContent.none);
		}
	}


	[CustomPropertyDrawer(typeof(VariableBoolExpressionBase<>))]
	public class VariableExpressionDrawer : PropertyDrawer
	{
		const string s_PropertyName = "m_Variable";

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
