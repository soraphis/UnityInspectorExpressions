using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct ComponentExpression : IExpression<Component>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal ComponentExpressionBase m_ExpressionRef;

		public ComponentExpression(ComponentExpressionBase @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public Component Evaluate(Dictionary<int, object> ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);
		public Component Evaluate() => ((IExpression<Component>)this).DefaultEvaluate();

		
		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralComponentExpression(default);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralComponentExpression(default);
		}
	}
}
