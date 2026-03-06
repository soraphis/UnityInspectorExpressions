using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    [System.Serializable]
    public abstract class Vector3ExpressionBase<TCtx> : IExpression<Vector3, TCtx>, IWrapable<UnaryVector3Expression<TCtx>>, IWrapable<BinaryVector3Expression<TCtx>>
    {
        public abstract Vector3 Evaluate(TCtx ctx);

        UnaryVector3Expression<TCtx> IWrapable<UnaryVector3Expression<TCtx>>.Wrap() => new UnaryVector3Expression<TCtx>() { m_InnerExpr = new(this) };
        BinaryVector3Expression<TCtx> IWrapable<BinaryVector3Expression<TCtx>>.Wrap() => new BinaryVector3Expression<TCtx>() { m_InnerExpr1 = new(this) };
    }

    [System.Serializable]
    public class LiteralVector3Expression<TCtx> : Vector3ExpressionBase<TCtx>
    {
        [SerializeField] internal Vector3 m_Literal;
        public LiteralVector3Expression() { }
        public LiteralVector3Expression(Vector3 literal) { m_Literal = literal; }
        public override Vector3 Evaluate(TCtx ctx) => m_Literal;
    }

    [System.Serializable]
    public class UnaryVector3Expression<TCtx> : Vector3ExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Normalize,
        }
        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override Vector3 Evaluate(TCtx ctx)
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
    public class BinaryVector3Expression<TCtx> : Vector3ExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [InspectorName("+")] Add,
            [InspectorName("-")] Subtract,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)] Cross,
        }

        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr1;
        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override Vector3 Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Add      => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Cross    => Vector3.Cross(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            _ => throw new NotImplementedException(),
        };
    }

    [System.Serializable]
    public class MatchFirstVector3Expression<TCtx> : Vector3ExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public Vector3Expression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal Vector3Expression<TCtx> m_DefaultExpr;

        public override Vector3 Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }
}
