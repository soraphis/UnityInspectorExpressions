using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct StringExpression : IExpression<string>, ISerializationCallbackReceiver
    {
        [SerializeReference] internal StringExpressionBase m_ExpressionRef;

        public StringExpression(StringExpressionBase @ref) : this()
        {
            m_ExpressionRef = @ref;
        }

        public string Evaluate(Dictionary<int, object> ctx = null) => m_ExpressionRef?.Evaluate(ctx);
        public string Evaluate() => ((IExpression<string>)this).DefaultEvaluate();

        
        public void OnAfterDeserialize()
        {
            if (m_ExpressionRef == null) m_ExpressionRef = new LiteralStringExpression("");
        }

        public void OnBeforeSerialize()
        {
            if (m_ExpressionRef == null) m_ExpressionRef = new LiteralStringExpression("");
        }
    }
}
