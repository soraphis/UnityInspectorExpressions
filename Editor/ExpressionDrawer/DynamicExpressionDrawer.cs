using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
	[CustomPropertyDrawer(typeof(DynamicGameObjectExpression))]
	public class DynamicExpressionDrawer : PropertyDrawer
	{
		const string s_PropertyName = "m_DynamicSlot";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var literalProp = property.FindPropertyRelative(s_PropertyName);

			var row = new VisualElement();
			row.style.flexDirection = FlexDirection.Row;
			row.style.alignItems = Align.Center;
			row.style.flexGrow = 1;

			var lbl = new Label("dyn:");
			lbl.style.flexShrink = 0;
			lbl.style.minWidth = 30;
			lbl.style.maxWidth = 50;

			var field = new PropertyField(literalProp, "");
			field.style.flexGrow = 1;
			field.style.flexShrink = 1;

			row.Add(lbl);
			row.Add(field);
			return row;
		}
	}


	[CustomPropertyDrawer(typeof(VariableBoolExpressionBase<>))]
	public class VariableExpressionDrawer : PropertyDrawer
	{
		const string s_PropertyName = "m_Variable";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var literalProp = property.FindPropertyRelative(s_PropertyName);
			var field = new PropertyField(literalProp, "");
			field.style.flexGrow = 1;
			field.style.flexShrink = 1;
			return field;
		}
	}
}
