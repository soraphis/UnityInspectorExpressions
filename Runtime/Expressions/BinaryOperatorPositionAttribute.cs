using System;

namespace UnityInspectorExpressions.Expressions
{
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
