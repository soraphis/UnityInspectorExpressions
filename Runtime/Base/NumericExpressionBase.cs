using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    [Serializable]
    public abstract class NumericExpressionBase<TCtx> : IExpression<float, TCtx>, IWrapable<UnaryNumericExpression<TCtx>>, IWrapable<BinaryNumericExpression<TCtx>>
    {
        public abstract float Evaluate(TCtx ctx);

        UnaryNumericExpression<TCtx> IWrapable<UnaryNumericExpression<TCtx>>.Wrap() => new() { m_InnerExpr = new(this) };
        BinaryNumericExpression<TCtx> IWrapable<BinaryNumericExpression<TCtx>>.Wrap() => new() { m_InnerExpr1 = new(this) };
    }

    [Serializable]
    [ExpressionLabel("Numeric/Literal")]
    public class LiteralNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        [SerializeField] internal float m_Literal;
        public LiteralNumericExpression() { }
        public LiteralNumericExpression(float literal) { m_Literal = literal; }
        public override float Evaluate(TCtx ctx) => m_Literal;
        public override string ToString() => m_Literal.ToString();
    }

    [Serializable]
    [ExpressionLabel("Numeric/From Context")]
    public class FromContextNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        [SerializeField] internal string m_PathToProperty;
        [SerializeField] internal bool m_IsInteger;

        public override float Evaluate(TCtx ctx)
        {
            var propertyPath = new PropertyPath(m_PathToProperty);
            return m_IsInteger
                ? PropertyContainer.GetValue<TCtx, int>(ref ctx, propertyPath)
                : PropertyContainer.GetValue<TCtx, float>(ref ctx, propertyPath);
        }

        public override string ToString() => m_PathToProperty;
    }

    [Serializable]
    public abstract class NumericResultFunctionExpression<TCtx, TObj> : NumericExpressionBase<TCtx>
    {
        [SerializeField] protected internal TObj m_Object;
    }

    [Serializable]
    public abstract class NumericResultFunctionExpression<TCtx, TObj, TArg0> : NumericExpressionBase<TCtx>
    {
        [SerializeField] protected internal TObj m_Object;
        [SerializeField] protected internal TArg0 m_Argument;
    }

    [Serializable]
    [ExpressionLabel("Numeric/Unary")]
    public class UnaryNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Sign,
            Square,
            Sqrt,
        }
        [SerializeField] internal NumericExpression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;

        public override float Evaluate(TCtx ctx)
        {
            var value = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.Negate => -value,
                UnaryOperator.Sign => Mathf.Sign(value),
                UnaryOperator.Square => value * value,
                UnaryOperator.Sqrt => Mathf.Sqrt(value),
                _ => throw new NotImplementedException(),
            };
        }

        public override string ToString() => m_Operator switch
        {
            UnaryOperator.Negate => $"-({m_InnerExpr})",
            UnaryOperator.Sign => $"Sign({m_InnerExpr})",
            UnaryOperator.Square => $"Square({m_InnerExpr})",
            UnaryOperator.Sqrt => $"Sqrt({m_InnerExpr})",
            _ => throw new NotImplementedException(),
        };
    }

    [Serializable]
    [ExpressionLabel("Numeric/Random01")]
    public class RandomNumeric01Expression<TCtx> : NumericExpressionBase<TCtx>
    {
        public override float Evaluate(TCtx ctx) => UnityEngine.Random.value;
    }

    [Serializable]
    [ExpressionLabel("Numeric/Binary")]
    public class BinaryNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [InspectorName("+")] Add,
            [InspectorName("-")] Subtract,
            [InspectorName("*")] Multiply,
            [InspectorName("/")] Divide,
            [InspectorName("//"), OperatorDoc("Floor division of a by b", "a: dividend", "b: divisor")] FloorDivide,
            [InspectorName("%")] Modulo,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("The larger of a and b", "a: first value", "b: second value")] Max,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("The smaller of a and b", "a: first value", "b: second value")] Min,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Random float between min and max (inclusive)", "min: lower bound", "max: upper bound")] RandomRange,
            [InspectorName("+%"), OperatorDoc("a * (1 + b), e.g. 100 +% 0.4 = 140", "a: base value", "b: percent as fraction (0.4 = +40%)")] AddPercent,
        }

        [SerializeField] internal NumericExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal NumericExpression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;

        public override float Evaluate(TCtx ctx)
        {
            var a = m_InnerExpr1.Evaluate(ctx);
            var b = m_InnerExpr2.Evaluate(ctx);
            return m_Operator switch
            {
                BinaryOperator.Max => Mathf.Max(a, b),
                BinaryOperator.Min => Mathf.Min(a, b),
                BinaryOperator.RandomRange => UnityEngine.Random.Range(a, b),
                BinaryOperator.AddPercent => a * (1f + b),
                BinaryOperator.FloorDivide => Mathf.Floor(a / b),
                BinaryOperator.Add => a + b,
                BinaryOperator.Subtract => a - b,
                BinaryOperator.Multiply => a * b,
                BinaryOperator.Divide => a / b,
                BinaryOperator.Modulo => a % b,
                _ => throw new NotImplementedException(),
            };
        }

        public override string ToString() => m_Operator switch
        {
            BinaryOperator.Max => $"Max({m_InnerExpr1}, {m_InnerExpr2})",
            BinaryOperator.Min => $"Min({m_InnerExpr1}, {m_InnerExpr2})",
            BinaryOperator.RandomRange => $"RandomRange({m_InnerExpr1}, {m_InnerExpr2})",
            BinaryOperator.AddPercent => $"({m_InnerExpr1} +% {m_InnerExpr2})",
            BinaryOperator.FloorDivide => $"({m_InnerExpr1} // {m_InnerExpr2})",
            BinaryOperator.Add => $"({m_InnerExpr1} + {m_InnerExpr2})",
            BinaryOperator.Subtract => $"({m_InnerExpr1} - {m_InnerExpr2})",
            BinaryOperator.Multiply => $"({m_InnerExpr1} * {m_InnerExpr2})",
            BinaryOperator.Divide => $"({m_InnerExpr1} / {m_InnerExpr2})",
            BinaryOperator.Modulo => $"({m_InnerExpr1} % {m_InnerExpr2})",
            _ => throw new NotImplementedException(),
        };
    }

    [Serializable]
    [ExpressionLabel("Numeric/Ternary")]
    public class TernaryNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum TernaryOperator
        {
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Linear interpolation from a to b (t clamped to 0..1)", "a: result at t = 0", "b: result at t = 1", "t: blend factor (0..1)")] Lerp,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Where value lies between a and b, as 0..1 (clamped)", "a: maps to 0", "b: maps to 1", "value: the value to locate")] InverseLerp,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Random pick: b with probability 'chance', otherwise a", "a: result on a failed roll", "b: result on a successful roll", "chance: probability of b (0..1)")] Chance,
        }

        [SerializeField] internal NumericExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal NumericExpression<TCtx> m_InnerExpr2;
        [SerializeField] internal NumericExpression<TCtx> m_InnerExpr3;
        [SerializeField] internal TernaryOperator m_Operator;

        public override float Evaluate(TCtx ctx) => m_Operator switch
        {
            TernaryOperator.Lerp => Mathf.Lerp(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx), m_InnerExpr3.Evaluate(ctx)),
            TernaryOperator.InverseLerp => Mathf.InverseLerp(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx), m_InnerExpr3.Evaluate(ctx)),
            TernaryOperator.Chance => UnityEngine.Random.value < m_InnerExpr3.Evaluate(ctx)
                ? m_InnerExpr2.Evaluate(ctx)
                : m_InnerExpr1.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };

        public override string ToString() => m_Operator switch
        {
            TernaryOperator.Lerp => $"Lerp({m_InnerExpr1}, {m_InnerExpr2}, {m_InnerExpr3})",
            TernaryOperator.InverseLerp => $"InverseLerp({m_InnerExpr1}, {m_InnerExpr2}, {m_InnerExpr3})",
            TernaryOperator.Chance => $"Chance({m_InnerExpr1}, {m_InnerExpr2}, {m_InnerExpr3})",
            _ => throw new NotImplementedException(),
        };
    }

    [Serializable]
    [ExpressionLabel("Numeric/Match First")]
    public class MatchFirstNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        [Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public NumericExpression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries = new();
        [SerializeField] internal NumericExpression<TCtx> m_DefaultExpr;

        public override float Evaluate(TCtx ctx)
        {
            var idx = m_Entries?.FindIndex(x => x.m_Condition.Evaluate(ctx)) ?? -1;
            return idx == -1 ? m_DefaultExpr.Evaluate(ctx) : m_Entries[idx].m_Result.Evaluate(ctx);
        }

        public override string ToString() => $"MatchFirst({string.Join(", ", m_Entries.ConvertAll(x => $"if ({x.m_Condition}) then {x.m_Result}"))}, default: {m_DefaultExpr})";
    }

    [Serializable]
    [ExpressionLabel("Numeric/Vec2 Unary")]
    public class UnaryVector2CastToNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum UnaryOperator { X, Y, Magnitude }
        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override float Evaluate(TCtx ctx)
        {
            var value = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.X => value.x,
                UnaryOperator.Y => value.y,
                UnaryOperator.Magnitude => value.magnitude,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [Serializable]
    [ExpressionLabel("Numeric/Vec2 Binary")]
    public class BinaryVector2CastToNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)] Dot,
        }
        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr1;
        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override float Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Dot => Vector2.Dot(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            _ => throw new NotImplementedException(),
        };
    }

    [Serializable]
    [ExpressionLabel("Numeric/Vec3 Unary")]
    public class UnaryVector3CastToNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum UnaryOperator { X, Y, Z, Magnitude }
        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override float Evaluate(TCtx ctx)
        {
            var value = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.X => value.x,
                UnaryOperator.Y => value.y,
                UnaryOperator.Z => value.z,
                UnaryOperator.Magnitude => value.magnitude,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [Serializable]
    [ExpressionLabel("Numeric/Vec3 Binary")]
    public class BinaryVector3CastToNumericExpression<TCtx> : NumericExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)] Dot,
        }
        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr1;
        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override float Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Dot => Vector3.Dot(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            _ => throw new NotImplementedException(),
        };
    }
}