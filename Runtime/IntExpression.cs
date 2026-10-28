using System.Diagnostics.Contracts;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct IntExpression<TCtx> : IExpression<int, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeField] internal NumericExpression<TCtx> m_NumericExpression;

		public IntExpression(NumericExpression<TCtx> expression) : this() { m_NumericExpression = expression; }

		public static IntExpression<TCtx> FromLiteral(int value) => new(NumericExpression<TCtx>.FromLiteral(value));
		public static implicit operator IntExpression<TCtx>(int value) => FromLiteral(value);
		public static implicit operator IntExpression<TCtx>(NumericExpression<TCtx> expression) => new(expression);

		internal static int Round(float value) => Mathf.RoundToInt(value);

		[Pure]
		public int Evaluate(TCtx ctx) => Round(m_NumericExpression.Evaluate(ctx));

		public void OnAfterDeserialize()  { if (m_NumericExpression.m_ExpressionRef == null) m_NumericExpression.OnAfterDeserialize(); }
		public void OnBeforeSerialize()   { if (m_NumericExpression.m_ExpressionRef == null) m_NumericExpression.OnBeforeSerialize(); }

		public override string ToString()
		{
			return m_NumericExpression.ToString();
		}
	}
}
