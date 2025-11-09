using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct GameObjectExpression : IExpression<GameObject>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal GameObjectExpressionBase m_ExpressionRef;

		public GameObjectExpression(GameObjectExpressionBase @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public GameObject Evaluate(Dictionary<int, object> ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);
		public GameObject Evaluate() => ((IExpression<GameObject>)this).DefaultEvaluate();


		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralGameObjectExpression(default);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralGameObjectExpression(default);
		}
	}
}
