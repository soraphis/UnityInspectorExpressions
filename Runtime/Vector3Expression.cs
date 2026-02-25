using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
	[System.Serializable]
	public struct Vector3Expression : IExpression<Vector3>, ISerializationCallbackReceiver
	{
		[SerializeReference] internal Vector3ExpressionBase m_ExpressionRef;

		public Vector3Expression(Vector3ExpressionBase @ref) : this()
		{
			m_ExpressionRef = @ref;
		}

		public Vector3 Evaluate(Dictionary<int, object> ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);
		public Vector3 Evaluate() => ((IExpression<Vector3>)this).DefaultEvaluate();

		public void OnAfterDeserialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector3Expression(Vector3.zero);
		}

		public void OnBeforeSerialize()
		{
			if (m_ExpressionRef == null) m_ExpressionRef = new LiteralVector3Expression(Vector3.zero);
		}
	}
}
