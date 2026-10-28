using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class FloatExpressionBase<TCtx> : IExpression<float, TCtx>, IWrapable<UnaryFloatExpression<TCtx>>, IWrapable<BinaryFloatExpression<TCtx>>
    {
        public abstract float Evaluate(TCtx ctx);

        UnaryFloatExpression<TCtx> IWrapable<UnaryFloatExpression<TCtx>>.Wrap() => new UnaryFloatExpression<TCtx>() { m_InnerExpr = new(this) };
        BinaryFloatExpression<TCtx> IWrapable<BinaryFloatExpression<TCtx>>.Wrap() => new BinaryFloatExpression<TCtx>() { m_InnerExpr1 = new(this) };
    }

    [System.Serializable]
    [ExpressionLabel("Float/Literal")]
    public class LiteralFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        [SerializeField] internal float m_Literal;
        public LiteralFloatExpression() { }
        public LiteralFloatExpression(float literal) { m_Literal = literal; }
        public override float Evaluate(TCtx ctx) => m_Literal;
    }

    [System.Serializable]
    [ExpressionLabel("From Context")]
    public class FromContextFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        [SerializeField] internal string m_PathToProperty;

        public override float Evaluate(TCtx ctx)
        {
            var propertyPath = new PropertyPath(m_PathToProperty);
            return PropertyContainer.GetValue<TCtx, float>(ref ctx, propertyPath);
        }
    }

    [System.Serializable]
    [ExpressionLabel("Float/Unary")]
    public class UnaryFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Sign,
            Square,
            Sqrt,
            // kept only so existing serialized data keeps working; ignores its argument. use Float/Random01
            [Obsolete("Use RandomFloat01Expression (Float/Random01)")] Random01,
        }
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override float Evaluate(TCtx ctx)
        {
            var f = m_InnerExpr.Evaluate(ctx);
#pragma warning disable CS0618
            return m_Operator switch
            {
                UnaryOperator.Negate    => -f,
                UnaryOperator.Sign      => Mathf.Sign(f),
                UnaryOperator.Square    => f * f,
                UnaryOperator.Sqrt      => Mathf.Sqrt(f),
                UnaryOperator.Random01  => UnityEngine.Random.value,
                _ => throw new NotImplementedException(),
            };
#pragma warning restore CS0618
        }
    }

    [System.Serializable]
    [ExpressionLabel("Float/Random01")]
    public class RandomFloat01Expression<TCtx> : FloatExpressionBase<TCtx>
    {
        public override float Evaluate(TCtx ctx) => UnityEngine.Random.value;
    }

    [System.Serializable]
    [ExpressionLabel("Float/Binary")]
    public class BinaryFloatExpression<TCtx> : FloatExpressionBase<TCtx>
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
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Random float between min and max (inclusive)", "min: lower bound", "max: upper bound")] RandomRange,
            [InspectorName("+%"), OperatorDoc("a * (1 + b), e.g. 100 +% 0.4 = 140", "a: base value", "b: percent as fraction (0.4 = +40%)")] AddPercent,
        }

        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr2;
        [SerializeField] internal BinaryOperator m_Operator;
        public override float Evaluate(TCtx ctx) => m_Operator switch
        {
            BinaryOperator.Max         => Mathf.Max(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Min         => Mathf.Min(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.RandomRange => UnityEngine.Random.Range(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.AddPercent  => m_InnerExpr1.Evaluate(ctx) * (1f + m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Add      => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Multiply => m_InnerExpr1.Evaluate(ctx) * m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Divide   => m_InnerExpr1.Evaluate(ctx) / m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Modulo   => m_InnerExpr1.Evaluate(ctx) % m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    [System.Serializable]
    [ExpressionLabel("Float/Ternary")]
    public class TernaryFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        public enum TernaryOperator
        {
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Linear interpolation from a to b (t clamped to 0..1)", "a: result at t = 0", "b: result at t = 1", "t: blend factor (0..1)")] Lerp,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Where value lies between a and b, as 0..1 (clamped)", "a: maps to 0", "b: maps to 1", "value: the value to locate")] InverseLerp,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall), OperatorDoc("Random pick: b with probability 'chance', otherwise a", "a: result on a failed roll", "b: result on a successful roll", "chance: probability of b (0..1)")] Chance,
        }

        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr1;
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr2;
        [SerializeField] internal FloatExpression<TCtx> m_InnerExpr3;
        [SerializeField] internal TernaryOperator m_Operator;

        public override float Evaluate(TCtx ctx) => m_Operator switch
        {
            TernaryOperator.Lerp        => Mathf.Lerp(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx), m_InnerExpr3.Evaluate(ctx)),
            TernaryOperator.InverseLerp => Mathf.InverseLerp(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx), m_InnerExpr3.Evaluate(ctx)),
            TernaryOperator.Chance      => UnityEngine.Random.value < m_InnerExpr3.Evaluate(ctx) ? m_InnerExpr2.Evaluate(ctx) : m_InnerExpr1.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }


    [System.Serializable]
    [ExpressionLabel("Float/Match First")]
    public class MatchFirstFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public FloatExpression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal FloatExpression<TCtx> m_DefaultExpr;

        public override float Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

    [System.Serializable]
    [ExpressionLabel("Float/Cast from Int")]
    public class IntToFloatCastExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        [SerializeField] internal IntExpression<TCtx> m_InnerExpr;
        public override float Evaluate(TCtx ctx) => m_InnerExpr.Evaluate(ctx);
    }

    [System.Serializable]
    [ExpressionLabel("Float/Vec2 Unary")]
    public class UnaryVector2CastToFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        public enum UnaryOperator { X, Y, Magnitude }
        [SerializeField] internal Vector2Expression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override float Evaluate(TCtx ctx)
        {
            var v = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.X         => v.x,
                UnaryOperator.Y         => v.y,
                UnaryOperator.Magnitude => v.magnitude,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [System.Serializable]
    [ExpressionLabel("Float/Vec2 Binary")]
    public class BinaryVector2CastToFloatExpression<TCtx> : FloatExpressionBase<TCtx>
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

    [System.Serializable]
    [ExpressionLabel("Float/Vec3 Unary")]
    public class UnaryVector3CastToFloatExpression<TCtx> : FloatExpressionBase<TCtx>
    {
        public enum UnaryOperator { X, Y, Z, Magnitude }
        [SerializeField] internal Vector3Expression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator m_Operator;
        public override float Evaluate(TCtx ctx)
        {
            var v = m_InnerExpr.Evaluate(ctx);
            return m_Operator switch
            {
                UnaryOperator.X         => v.x,
                UnaryOperator.Y         => v.y,
                UnaryOperator.Z         => v.z,
                UnaryOperator.Magnitude => v.magnitude,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [System.Serializable]
    [ExpressionLabel("Float/Vec3 Binary")]
    public class BinaryVector3CastToFloatExpression<TCtx> : FloatExpressionBase<TCtx>
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
