using System;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct BoolExpression<TCtx> : IExpression<bool, TCtx>, ISerializationCallbackReceiver
    {
        [SerializeReference] internal BoolExpressionBase<TCtx> m_ExpressionRef;

        public BoolExpression(BoolExpressionBase<TCtx> @ref) : this() { m_ExpressionRef = @ref; }

        public bool Evaluate(TCtx ctx) => m_ExpressionRef == null || m_ExpressionRef.Evaluate(ctx);

        public void OnAfterDeserialize()  { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralBoolExpression<TCtx>(true); }
        public void OnBeforeSerialize()   { if (m_ExpressionRef == null) m_ExpressionRef = new LiteralBoolExpression<TCtx>(true); }
    }
}
