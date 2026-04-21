using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    [System.Serializable]
    public abstract class Vector2ExpressionBase<TCtx> : IExpression<Vector2, TCtx>, IWrapable<UnaryVector2Expression<TCtx>>, IWrapable<BinaryVector2Expression<TCtx>>
    {
        public abstract Vector2 Evaluate(TCtx ctx);

        UnaryVector2Expression<TCtx> IWrapable<UnaryVector2Expression<TCtx>>.Wrap() => new UnaryVector2Expression<TCtx>() { m_InnerExpr = new(this) };
        BinaryVector2Expression<TCtx> IWrapable<BinaryVector2Expression<TCtx>>.Wrap() => new BinaryVector2Expression<TCtx>() { m_InnerExpr1 = new(this) };
    }

    [System.Serializable]
    public class LiteralVector2Expression<TCtx> : Vector2ExpressionBase<TCtx>
    {
        [SerializeField] internal Vector2 m_Literal;
        public LiteralVector2Expression() { }
        public LiteralVector2Expression(Vector2 literal) { m_Literal = literal; }
        public override Vector2 Evaluate(TCtx ctx) => m_Literal;
    }

    public class FromContextVector2Expression<TCtx> : Vector2ExpressionBase<TCtx>
    {
        [SerializeField] internal string m_PathToProperty;

        public override Vector2 Evaluate(TCtx ctx)
        {
            var propertyPath = new PropertyPath(m_PathToProperty);
            return PropertyContainer.GetValue<TCtx, Vector2>(ref ctx, propertyPath);
        }
    }

    [System.Serializable]
    public class UnaryVector2Expression<TCtx> : Vector2ExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Normalize,
        }
        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override Vector2 Evaluate(TCtx ctx)
        {
            var v = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.Negate    => -v,
                UnaryOperator.Normalize => v.normalized,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [System.Serializable]
    public class BinaryVector2Expression<TCtx> : Vector2ExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [InspectorName("+")] Add,
            [InspectorName("-")] Subtract,
        }

        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr1;
        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override Vector2 Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Add      => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    [System.Serializable]
    public class MatchFirstVector2Expression<TCtx> : Vector2ExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public Vector2Expression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal Vector2Expression<TCtx> m_DefaultExpr;

        public override Vector2 Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }
}
