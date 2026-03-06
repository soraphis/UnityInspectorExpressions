using System;

namespace UnityInspectorExpressions.Expressions
{
	/// <summary>
	/// Overrides the label shown for this expression type in the type-selector menu.
	/// Use forward slashes to create sub-menus, e.g. "Bool/Literal".
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ExpressionLabelAttribute : Attribute
	{
		public readonly string Label;
		public ExpressionLabelAttribute(string label) => Label = label;
	}

	public enum BinaryOperatorPosition { Operator, FunctionCall }

	public class BinaryOperatorPositionAttribute : Attribute
	{
		public BinaryOperatorPosition m_Position = BinaryOperatorPosition.Operator;

		public BinaryOperatorPositionAttribute(BinaryOperatorPosition mPosition)
		{
			m_Position = mPosition;
		}
	}

	public enum UnaryOperatorPosition { Operator, FunctionCall, Property }

	public class UnaryOperatorPositionAttribute : Attribute
	{
		public UnaryOperatorPosition m_Position = UnaryOperatorPosition.Operator;
	}
}
