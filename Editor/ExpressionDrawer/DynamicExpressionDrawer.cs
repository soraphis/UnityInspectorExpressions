using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
	[CustomPropertyDrawer(typeof(VariableBoolExpressionBase<,>))]
	[CustomPropertyDrawer(typeof(VariableFloatExpression<,>))]
	[CustomPropertyDrawer(typeof(VariableIntExpression<,>))]
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
