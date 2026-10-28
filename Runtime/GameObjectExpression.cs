using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct GameObjectExpression<TCtx> : IExpression<GameObject, TCtx>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal GameObjectExpressionBase<TCtx> m_ExpressionRef;

		public GameObjectExpression(GameObjectExpressionBase<TCtx> @ref) : this() { m_ExpressionRef = @ref; }

		public static GameObjectExpression<TCtx> FromLiteral(GameObject value) => new(new LiteralGameObjectExpression<TCtx>(value));
		public static implicit operator GameObjectExpression<TCtx>(GameObject value) => FromLiteral(value);

		public GameObject Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

		public void OnAfterDeserialize()  { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralGameObjectExpression<TCtx>(default); }
		public void OnBeforeSerialize()   { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralGameObjectExpression<TCtx>(default); }
	}
}
