using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    [System.Serializable]
    public abstract class Vector2ExpressionBase : IExpression<Vector2>, IWrapable<UnaryVector2Expression>, IWrapable<BinaryVector2Expression>
    {
        public abstract Vector2 Evaluate(Dictionary<int, object> ctx);
        public Vector2 Evaluate() => ((IExpression<Vector2>)this).DefaultEvaluate();

        UnaryVector2Expression IWrapable<UnaryVector2Expression>.Wrap() => new UnaryVector2Expression() { m_InnerExpr = new(this) };
        BinaryVector2Expression IWrapable<BinaryVector2Expression>.Wrap() => new BinaryVector2Expression() { m_InnerExpr1 = new(this) };
    }

    [System.Serializable]
    public class LiteralVector2Expression : Vector2ExpressionBase
    {
        [SerializeField] internal Vector2 m_Literal;
        public LiteralVector2Expression() { }
        public LiteralVector2Expression(Vector2 literal) { m_Literal = literal; }
        public override Vector2 Evaluate(Dictionary<int, object> ctx) => m_Literal;
    }

    [System.Serializable]
    public class UnaryVector2Expression : Vector2ExpressionBase
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Normalize,
        }
        [SerializeField] internal Vector2Expression m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override Vector2 Evaluate(Dictionary<int, object> ctx)
        {
            var v = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.Negate => -v,
                UnaryOperator.Normalize => v.normalized,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [System.Serializable]
    public class BinaryVector2Expression : Vector2ExpressionBase
    {
        public enum BinaryOperator
        {
            [InspectorName("+")] Add,
            [InspectorName("-")] Subtract,
        }

        [SerializeField] internal Vector2Expression m_InnerExpr1;
        [SerializeField] internal Vector2Expression m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override Vector2 Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            BinaryOperator.Add => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    [System.Serializable]
    public class MatchFirstVector2Expression : Vector2ExpressionBase
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression m_Condition;
            public Vector2Expression m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal Vector2Expression m_DefaultExpr;

        public override Vector2 Evaluate(Dictionary<int, object> ctx)
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
