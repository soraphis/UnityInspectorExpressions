using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class StringExpressionBase<TCtx> : IExpression<string, TCtx>
    {
        public abstract string Evaluate(TCtx ctx);
    }

    [System.Serializable]
    [ExpressionLabel("string/Literal")]
    public class LiteralStringExpression<TCtx> : StringExpressionBase<TCtx>
    {
        [SerializeField] internal string m_Literal;
        public LiteralStringExpression() { }
        public LiteralStringExpression(string literal) { m_Literal = literal; }
        public override string Evaluate(TCtx ctx) => m_Literal;

    }


    [System.Serializable]
    [ExpressionLabel("string/Match First")]
    public class MatchFirstStringExpression<TCtx> : StringExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public StringExpression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal StringExpression<TCtx> m_DefaultExpr;

        public override string Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

}
