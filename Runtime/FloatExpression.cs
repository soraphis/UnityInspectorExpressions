using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct FloatExpression<TCtx> : IExpression<float, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal FloatExpressionBase<TCtx> m_ExpressionRef;

		public FloatExpression(FloatExpressionBase<TCtx> @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public float Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);


		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralFloatExpression<TCtx>(0f);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralFloatExpression<TCtx>(0f);
		}
	}
}
