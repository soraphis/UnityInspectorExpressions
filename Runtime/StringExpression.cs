using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct StringExpression<TCtx> : IExpression<string, TCtx>, ISerializationCallbackReceiver
    {
        [SerializeReference] internal StringExpressionBase<TCtx> m_ExpressionRef;

        public StringExpression(StringExpressionBase<TCtx> @ref) : this() { m_ExpressionRef = @ref; }

        public string Evaluate(TCtx ctx) => m_ExpressionRef?.Evaluate(ctx);

        public void OnAfterDeserialize()  { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralStringExpression<TCtx>(""); }
        public void OnBeforeSerialize()   { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralStringExpression<TCtx>(""); }
    }
}
