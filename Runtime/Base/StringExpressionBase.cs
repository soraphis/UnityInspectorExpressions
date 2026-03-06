using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class StringExpressionBase : IExpression<string>
    {
        public abstract string Evaluate(Dictionary<int, object> ctx);
        public string Evaluate() => ((IExpression<string>)this).DefaultEvaluate();

    }

    [System.Serializable]
    [ExpressionLabel("string/Literal")]
    public class LiteralStringExpression : StringExpressionBase
    {
        [SerializeField] internal string m_Literal;
        public LiteralStringExpression() { }
        public LiteralStringExpression(string literal) { m_Literal = literal; }
        public override string Evaluate(Dictionary<int, object> ctx) => m_Literal;

    }


    [System.Serializable]
    [ExpressionLabel("string/Match First")]
    public class MatchFirstStringExpression : StringExpressionBase
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression m_Condition;
            public StringExpression m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal StringExpression  m_DefaultExpr;

        public override string Evaluate(Dictionary<int, object> ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1)
            {
                return m_DefaultExpr.Evaluate(ctx);
            }
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

}
