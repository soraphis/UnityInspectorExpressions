using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class ComponentExpressionBase<TCtx> : IExpression<Component, TCtx>
    {
        public abstract Component Evaluate(TCtx ctx);
    }

    [System.Serializable]
    [ExpressionLabel("Component/Literal")]
    public class LiteralComponentExpression<TCtx> : ComponentExpressionBase<TCtx>
    {
        [SerializeField] internal Component m_Literal;
        public LiteralComponentExpression() { }
        public LiteralComponentExpression(Component literal) { m_Literal = literal; }
        public override Component Evaluate(TCtx ctx) => m_Literal;
    }


    [System.Serializable]
    [ExpressionLabel("Component/Match First")]
    public class MatchFirstComponentExpression<TCtx> : ComponentExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public ComponentExpression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal ComponentExpression<TCtx> m_DefaultExpr;

        public override Component Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

}
