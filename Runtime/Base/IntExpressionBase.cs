using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace UnityInspectorExpressions.Expressions.Base
{
    [System.Serializable]
    public abstract class IntExpressionBase<TCtx> : IExpression<int, TCtx>, IWrapable<UnaryIntExpression<TCtx>>, IWrapable<BinaryIntExpression<TCtx>>
    {
        public abstract int Evaluate(TCtx ctx);

        UnaryIntExpression<TCtx> IWrapable<UnaryIntExpression<TCtx>>.Wrap() => new UnaryIntExpression<TCtx>() { m_InnerExpr = new(this) };
        BinaryIntExpression<TCtx> IWrapable<BinaryIntExpression<TCtx>>.Wrap() => new BinaryIntExpression<TCtx>() { m_InnerExpr1 = new(this) };
    }

    [System.Serializable]
    [ExpressionLabel("Int/Literal")]
    public class LiteralIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [SerializeField] internal int m_Literal;
        public LiteralIntExpression() { }
        public LiteralIntExpression(int literal) { m_Literal = literal; }
        public override int Evaluate(TCtx ctx) => m_Literal;
    }

    public abstract class IntResultFunctionExpression<TCtx, TObj> : IntExpressionBase<TCtx>
    {
        [SerializeField] protected internal TObj m_Object;
    }

    public abstract class IntResultFunctionExpression<TCtx, TObj, TArg0> : IntExpressionBase<TCtx>
    {
        [SerializeField] protected internal TObj m_Object;
        [SerializeField] protected internal TArg0 m_Argument;
    }

    [System.Serializable]
    [ExpressionLabel("Int/Unary")]
    public class UnaryIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Sign,
            Square,
        }
        [SerializeField] internal IntExpression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override int Evaluate(TCtx ctx)
        {
            var f = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.Negate => -f,
                UnaryOperator.Sign   => Math.Sign(f),
                UnaryOperator.Square => f * f,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [System.Serializable]
    [ExpressionLabel("Int/Binary")]
    public class BinaryIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [InspectorName("+")] Add,
            [InspectorName("-")] Subtract,
            [InspectorName("*")] Multiply,
            [InspectorName("÷")] Divide,
            [InspectorName("%")] Modulo,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)] Max,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)] Min,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)] RandomRange,
        }

        [SerializeField] internal IntExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal IntExpression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override int Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Max         => Mathf.Max(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Min         => Mathf.Min(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.RandomRange => UnityEngine.Random.Range(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Add         => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract    => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Multiply    => m_InnerExpr1.Evaluate(ctx) * m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Divide      => m_InnerExpr1.Evaluate(ctx) / m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Modulo      => m_InnerExpr1.Evaluate(ctx) % m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    [System.Serializable]
    [ExpressionLabel("Int/Match First")]
    public class MatchFirstIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public IntExpression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal IntExpression<TCtx> m_DefaultExpr;

        public override int Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

    [System.Serializable]
    [ExpressionLabel("Int/Cast from Float")]
    public class FloatToIntCastExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr;
        public override int Evaluate(TCtx ctx) => (int)m_InnerExpr.Evaluate(ctx);
    }
}
