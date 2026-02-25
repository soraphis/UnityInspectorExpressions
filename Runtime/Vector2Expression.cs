using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct Vector2Expression : IExpression<Vector2>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal Vector2ExpressionBase m_ExpressionRef;

		public Vector2Expression(Vector2ExpressionBase @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public Vector2 Evaluate(Dictionary<int, object> ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);
		public Vector2 Evaluate() => ((IExpression<Vector2>)this).DefaultEvaluate();

		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector2Expression(Vector2.zero);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector2Expression(Vector2.zero);
		}
	}
}
