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

			var row = CustomStyles.MakeRow();

			var lbl = new Label("dyn:");
			lbl.style.flexShrink = 0;
			lbl.style.minWidth   = 30;
			lbl.style.maxWidth   = 50;

			row.Add(lbl);
			row.Add(new PropertyField(literalProp, "").WithFlex(1, 1));
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
			return new PropertyField(literalProp, "").WithFlex(1, 1);
		}
	}
}
