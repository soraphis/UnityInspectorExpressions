using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct Vector3Expression<TCtx> : IExpression<Vector3, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal Vector3ExpressionBase<TCtx> m_ExpressionRef;

		public Vector3Expression(Vector3ExpressionBase<TCtx> @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public Vector3 Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector3Expression<TCtx>(Vector3.zero);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector3Expression<TCtx>(Vector3.zero);
		}
	}
}
