using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct Vector2Expression<TCtx> : IExpression<Vector2, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal Vector2ExpressionBase<TCtx> m_ExpressionRef;

		public Vector2Expression(Vector2ExpressionBase<TCtx> @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public Vector2 Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector2Expression<TCtx>(Vector2.zero);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector2Expression<TCtx>(Vector2.zero);
		}
	}
}
