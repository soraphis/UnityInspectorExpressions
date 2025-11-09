using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class ComponentExpressionBase : IExpression<Component>
    {
        public abstract Component Evaluate(Dictionary<int, object> ctx);
        public Component Evaluate() => ((IExpression<Component>)this).DefaultEvaluate();

    }

    [System.Serializable]
    public class LiteralComponentExpression : ComponentExpressionBase
    {
        [SerializeField] internal Component m_Literal;
        public LiteralComponentExpression() { }
        public LiteralComponentExpression(Component literal) { m_Literal = literal; }
        public override Component Evaluate(Dictionary<int, object> ctx) => m_Literal;

    }


    [System.Serializable]
    public class MatchFirstComponentExpression : ComponentExpressionBase
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression m_Condition;
            public ComponentExpression m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal ComponentExpression  m_DefaultExpr;

        public override Component Evaluate(Dictionary<int, object> ctx)
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
