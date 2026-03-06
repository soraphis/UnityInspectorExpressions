using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct IntExpression<TCtx> : IExpression<int, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal IntExpressionBase<TCtx> m_ExpressionRef;

		public IntExpression(IntExpressionBase<TCtx> @ref) : this() { m_ExpressionRef = @ref; }

		public int Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralIntExpression<TCtx>(0);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralIntExpression<TCtx>(0);
		}
	}
}
