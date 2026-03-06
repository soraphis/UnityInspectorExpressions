using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct ComponentExpression<TCtx> : IExpression<Component, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal ComponentExpressionBase<TCtx> m_ExpressionRef;

		public ComponentExpression(ComponentExpressionBase<TCtx> @ref) : this() { m_ExpressionRef = @ref; }

		public Component Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()  { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralComponentExpression<TCtx>(default); }
		public void OnBeforeSerialize()   { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralComponentExpression<TCtx>(default); }
	}
}
