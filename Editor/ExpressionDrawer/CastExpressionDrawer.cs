using System;
using Editor.Helpers;
using Project.Expressions.Base;
using UnityEditor;
using UnityEngine;

namespace Project.Expressions
{
	[CustomPropertyDrawer(typeof(IntToFloatCastExpression))]
	[CustomPropertyDrawer(typeof(FloatToIntCastExpression))]
	public class CastExpressionDrawer : PropertyDrawer
	{
		const string s_InnerPropertyName    = "m_InnerExpr";

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var exprProp = property.FindPropertyRelative(s_InnerPropertyName);

			Span<Rect> rects = stackalloc Rect[2];
			using (var x = position.Row(rects))
			{
				x.Container(40);
				x.Flex(1, 60);
			}

			var typename = fieldInfo.FieldType.Name;
			// typename always ends with "ExpressionBase"
			var castName = typename.Substring(0, typename.Length - "ExpressionBase".Length);
			var content = new GUIContent($"({castName.ToLower()})");

			GUI.Label(rects[0].Shift(0, -1), content);
			EditorGUI.PropertyField(rects[1], exprProp, GUIContent.none);
		}
	}

	[CustomPropertyDrawer(typeof(ComponentToGameObjectExpression))]
	public class PropertyAccessExpressionDrawer : PropertyDrawer
	{
		const string s_InnerPropertyName = "m_InnerExpr";

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var exprProp = property.FindPropertyRelative(s_InnerPropertyName);

			Span<Rect> rects = stackalloc Rect[2];
			using (var x = position.Row(rects))
			{
				x.Flex(1, 60);
				x.Container(100);
			}

			var typename = fieldInfo.FieldType.Name;
			// typename always ends with "ExpressionBase"
			var castName = typename.Substring(0, typename.Length - "ExpressionBase".Length);
			var content = new GUIContent($".{castName.ToLower()}");

			EditorGUI.PropertyField(rects[0], exprProp, GUIContent.none);
			GUI.Label(rects[1].Shift(0, -1), content);
		}
	}
}
