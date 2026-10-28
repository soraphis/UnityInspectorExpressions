using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using Random = System.Random;

namespace UnityInspectorExpressions.Expressions.Base
{
    [Serializable]
    public abstract class IntExpressionBase<TCtx> : IExpression<int, TCtx>, IWrapable<UnaryIntExpression<TCtx>>, IWrapable<BinaryIntExpression<TCtx>>
    {
        public abstract int Evaluate(TCtx ctx);

        UnaryIntExpression<TCtx> IWrapable<UnaryIntExpression<TCtx>>.Wrap() => new UnaryIntExpression<TCtx> { m_InnerExpr = new(this) };
        BinaryIntExpression<TCtx> IWrapable<BinaryIntExpression<TCtx>>.Wrap() => new BinaryIntExpression<TCtx> { m_InnerExpr1 = new(this) };
    }

    [Serializable]
    [ExpressionLabel("Int/Literal")]
    public class LiteralIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [SerializeField] internal int m_Literal;
        public LiteralIntExpression() { }
        public LiteralIntExpression(int literal) { m_Literal = literal; }
        public override int Evaluate(TCtx ctx) => m_Literal;

        public override string ToString() => m_Literal.ToString();
    }

    [Serializable]
    [ExpressionLabel("Int From Context")]
    public class FromContextIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [SerializeField] internal string m_PathToProperty;

        public override int Evaluate(TCtx ctx)
        {
            var propertyPath = new PropertyPath(m_PathToProperty);
            return PropertyContainer.GetValue<TCtx, int>(ref ctx, propertyPath);
        }

        public override string ToString() => m_PathToProperty;
    }

    [Serializable]
    public abstract class IntResultFunctionExpression<TCtx, TObj> : IntExpressionBase<TCtx>
    {
        [SerializeField] protected internal TObj m_Object;
    }

    [Serializable]
    public abstract class IntResultFunctionExpression<TCtx, TObj, TArg0> : IntExpressionBase<TCtx>
    {
        [SerializeField] protected internal TObj m_Object;
        [SerializeField] protected internal TArg0 m_Argument;
    }

    [Serializable]
    [ExpressionLabel("Int/Unary")]
    public class UnaryIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Sign,
            Square,
            Sqrt, // floored
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
                UnaryOperator.Sqrt   => Mathf.FloorToInt(Mathf.Sqrt(f)),
                _ => throw new NotImplementedException(),
            };
        }

        public override string ToString()
        {
            return m_Operator switch
            {
                UnaryOperator.Negate => $"-({m_InnerExpr})",
                UnaryOperator.Sign   => $"Sign({m_InnerExpr})",
                UnaryOperator.Square => $"Square({m_InnerExpr})",
                UnaryOperator.Sqrt   => $"Sqrt({m_InnerExpr})",
                _ => throw new NotImplementedException(),
            };

        }
    }

    [Serializable]
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
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("The larger of a and b", "a: first value", "b: second value")] Max,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("The smaller of a and b", "a: first value", "b: second value")] Min,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Random int between min (inclusive) and max (exclusive)", "min: lower bound (inclusive)", "max: upper bound (exclusive)")] RandomRange,
            [InspectorName("+%"), OperatorDoc("a * (100 + b) / 100, e.g. 100 +% 40 = 140", "a: base value", "b: percent in whole numbers (40 = +40%)")] AddPercent,
        }

        [SerializeField] internal IntExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal IntExpression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override int Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Max         => Mathf.Max(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Min         => Mathf.Min(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.RandomRange => UnityEngine.Random.Range(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.AddPercent  => m_InnerExpr1.Evaluate(ctx) * (100 + m_InnerExpr2.Evaluate(ctx)) / 100,
            BinaryOperator.Add         => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract    => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Multiply    => m_InnerExpr1.Evaluate(ctx) * m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Divide      => m_InnerExpr1.Evaluate(ctx) / m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Modulo      => m_InnerExpr1.Evaluate(ctx) % m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };

        public override string ToString()
        {
            return m_Operator switch
            {
                BinaryOperator.Max         => $"Max({m_InnerExpr1}, {m_InnerExpr2})",
                BinaryOperator.Min         => $"Min({m_InnerExpr1}, {m_InnerExpr2})",
                BinaryOperator.RandomRange => $"RandomRange({m_InnerExpr1}, {m_InnerExpr2})",
                BinaryOperator.AddPercent  => $"({m_InnerExpr1} +% {m_InnerExpr2})",
                BinaryOperator.Add         => $"({m_InnerExpr1} + {m_InnerExpr2})",
                BinaryOperator.Subtract    => $"({m_InnerExpr1} - {m_InnerExpr2})",
                BinaryOperator.Multiply    => $"({m_InnerExpr1} * {m_InnerExpr2})",
                BinaryOperator.Divide      => $"({m_InnerExpr1} / {m_InnerExpr2})",
                BinaryOperator.Modulo      => $"({m_InnerExpr1} % {m_InnerExpr2})",
                _ => throw new NotImplementedException(),
            };
        }
    }

    [Serializable]
    [ExpressionLabel("Int/Ternary")]
    public class TernaryIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        public enum TernaryOperator
        {
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Linear interpolation from a to b, rounded to nearest int (t clamped to 0..1)", "a: result at t = 0", "b: result at t = 1", "t: blend factor (0..1, float)")] Lerp,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Random pick: b with probability 'chance', otherwise a", "a: result on a failed roll", "b: result on a successful roll", "chance: probability of b (0..1, float)")] Chance,
        }

        [SerializeField] internal IntExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal IntExpression<TCtx> m_InnerExpr2;
        // t / chance: fractional, so float
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr3;
        [SerializeField] internal TernaryOperator m_Operator;

        public override int Evaluate(TCtx ctx) => m_Operator switch
        {
            TernaryOperator.Lerp   => Mathf.RoundToInt(Mathf.Lerp(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx), m_InnerExpr3.Evaluate(ctx))),
            TernaryOperator.Chance => UnityEngine.Random.value < m_InnerExpr3.Evaluate(ctx) ? m_InnerExpr2.Evaluate(ctx) : m_InnerExpr1.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };

        public override string ToString()
        {
            return m_Operator switch
            {
                TernaryOperator.Lerp   => $"Lerp({m_InnerExpr1}, {m_InnerExpr2}, {m_InnerExpr3})",
                TernaryOperator.Chance => $"Chance({m_InnerExpr1}, {m_InnerExpr2}, {m_InnerExpr3})",
                _ => throw new NotImplementedException(),
            };
        }
    }

    [Serializable]
    [ExpressionLabel("Int/Match First")]
    public class MatchFirstIntExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [Serializable]
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

        public override string ToString()
        {
            var entriesStr = string.Join(", ", m_Entries.ConvertAll(x => $"if ({x.m_Condition}) then {x.m_Result}"));
            return $"MatchFirstInt({entriesStr}, default: {m_DefaultExpr})";
        }
    }

    [Serializable]
    [ExpressionLabel("Int/Cast from Float")]
    public class FloatToIntCastExpression<TCtx> : IntExpressionBase<TCtx>
    {
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr;
        public override int Evaluate(TCtx ctx) => (int)m_InnerExpr.Evaluate(ctx);

        public override string ToString()
        {
            return $"(int)({m_InnerExpr})";
        }
    }
}
