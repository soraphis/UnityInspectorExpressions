using System.Collections.Generic;
using Project.Expressions.Base;
using UnityEngine;

namespace Project.Expressions
{
	[System.Serializable]
	public struct FloatExpression : IExpression<float>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal FloatExpressionBase m_ExpressionRef;

		public FloatExpression(FloatExpressionBase @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public float Evaluate(Dictionary<int, object> ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);
		public float Evaluate() => ((IExpression<float>)this).DefaultEvaluate();

		
		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralFloatExpression(0f);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralFloatExpression(0f);
		}
	}
}
