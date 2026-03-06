using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct GameObjectExpression<TCtx> : IExpression<GameObject, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal GameObjectExpressionBase<TCtx> m_ExpressionRef;

		public GameObjectExpression(GameObjectExpressionBase<TCtx> @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public GameObject Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);


		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralGameObjectExpression<TCtx>(default);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralGameObjectExpression<TCtx>(default);
		}
	}
}
