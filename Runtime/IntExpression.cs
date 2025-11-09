using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct IntExpression : IExpression<int>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal IntExpressionBase m_ExpressionRef;

		public IntExpression(IntExpressionBase @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public int Evaluate(Dictionary<int, object> ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);
		public int Evaluate() => ((IExpression<int>)this).DefaultEvaluate();

		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralIntExpression(0);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralIntExpression(0);
		}
	}
}
