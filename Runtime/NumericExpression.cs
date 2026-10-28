using System.Diagnostics.Contracts;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct NumericExpression<TCtx> : IExpression<float, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal NumericExpressionBase<TCtx> m_ExpressionRef;

		public NumericExpression(NumericExpressionBase<TCtx> expression) : this() { m_ExpressionRef = expression; }

		public static NumericExpression<TCtx> FromLiteral(float value) => new(new LiteralNumericExpression<TCtx>(value));
		public static implicit operator NumericExpression<TCtx>(float value) => FromLiteral(value);
		public static implicit operator NumericExpression<TCtx>(int value) => FromLiteral(value);
		public static implicit operator NumericExpression<TCtx>(IntExpression<TCtx> expression) => expression.m_NumericExpression;

		[Pure]
		public float Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()  { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralNumericExpression<TCtx>(0f); }
		public void OnBeforeSerialize()   { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralNumericExpression<TCtx>(0f); }

		public override string ToString() => m_ExpressionRef == null ? base.ToString() : m_ExpressionRef.ToString();
	}
}