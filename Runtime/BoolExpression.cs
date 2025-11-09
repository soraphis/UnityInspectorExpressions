using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct BoolExpression : IExpression<bool>, ISerializationCallbackReceiver
    {
        [SerializeReference] internal BoolExpressionBase m_ExpressionRef;

        public BoolExpression(BoolExpressionBase @ref) : this()
        {
            m_ExpressionRef = @ref;
        }

        public bool Evaluate(Dictionary<int, object> ctx = null) => m_ExpressionRef == null || m_ExpressionRef.Evaluate(ctx);
        public bool Evaluate() => ((IExpression<bool>)this).DefaultEvaluate();

        
        public void OnAfterDeserialize()
        {
            if (m_ExpressionRef == null) m_ExpressionRef = new LiteralBoolExpression(true);
        }

        public void OnBeforeSerialize()
        {
            if (m_ExpressionRef == null) m_ExpressionRef = new LiteralBoolExpression(true);
        }
    }
}
