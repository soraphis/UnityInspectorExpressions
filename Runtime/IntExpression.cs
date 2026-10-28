using System.Diagnostics.Contracts;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct IntExpression<TCtx> : IExpression<int, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal IntExpressionBase<TCtx> m_ExpressionRef;

		public IntExpression(IntExpressionBase<TCtx> @ref) : this() { m_ExpressionRef = @ref; }

		public static IntExpression<TCtx> FromLiteral(int value) => new(new LiteralIntExpression<TCtx>(value));
		public static implicit operator IntExpression<TCtx>(int value) => FromLiteral(value);

		[Pure]
		public int Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()  { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralIntExpression<TCtx>(0); }
		public void OnBeforeSerialize()   { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralIntExpression<TCtx>(0); }

		public override string ToString()
		{
			if (m_ExpressionRef == null) return base.ToString();
			return m_ExpressionRef.ToString();
		}
	}
}
