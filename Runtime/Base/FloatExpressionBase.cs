using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class FloatExpressionBase : IExpression<float>, IWrapable<UnaryFloatExpression>, IWrapable<BinaryFloatExpression>
    {
        public abstract float Evaluate(Dictionary<int, object> ctx);
        public float Evaluate() => ((IExpression<float>)this).DefaultEvaluate();

        
        UnaryFloatExpression IWrapable<UnaryFloatExpression>.Wrap() => new UnaryFloatExpression() { m_InnerExpr = new(this) };
        BinaryFloatExpression IWrapable<BinaryFloatExpression>.Wrap() => new BinaryFloatExpression() { m_InnerExpr1 = new(this) };

    }

    [System.Serializable]
    public class LiteralFloatExpression : FloatExpressionBase
    {
        [SerializeField] internal float m_Literal;
        public LiteralFloatExpression() { }
        public LiteralFloatExpression(float literal) { m_Literal = literal; }
        public override float Evaluate(Dictionary<int, object> ctx) => m_Literal;

    }



    [System.Serializable]
    public class UnaryFloatExpression : FloatExpressionBase
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Sign,
			Square,
			Sqrt,
            Random01,
            // Sine, Cosine, Abs ...

        }
        [SerializeField] internal FloatExpression m_InnerExpr;
        [SerializeField] internal UnaryOperator  m_Operator;
        public override float Evaluate(Dictionary<int, object> ctx)
        {
	        var f = m_InnerExpr.Evaluate(ctx);
	        return m_Operator switch
	        {
		        UnaryOperator.Negate => -f,
		        UnaryOperator.Sign => Mathf.Sign(f),
		        UnaryOperator.Square => f * f,
		        UnaryOperator.Sqrt => Mathf.Sqrt(f),
                UnaryOperator.Random01 => UnityEngine.Random.value,
		        _ => throw new NotImplementedException(),
	        };
        }
    }

    [System.Serializable]
    public class BinaryFloatExpression : FloatExpressionBase
    {
        public enum BinaryOperator
        {
            [InspectorName("+")] Add,
            [InspectorName("-")] Subtract,
            [InspectorName("*")] Multiply,
            [InspectorName("÷")] Divide,
            [InspectorName("%")] Modulo,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)]Max,
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)]Min,
            // step

        }

        [SerializeField] internal FloatExpression  m_InnerExpr1;
        [SerializeField] internal FloatExpression  m_InnerExpr2;
        [SerializeField] internal BinaryOperator  m_Operator;
        public override float Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            BinaryOperator.Max => Mathf.Max(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Min => Mathf.Min(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Add => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Multiply => m_InnerExpr1.Evaluate(ctx) * m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Divide => m_InnerExpr1.Evaluate(ctx) / m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Modulo => m_InnerExpr1.Evaluate(ctx) % m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    // TODO: ternary -> float?

    [System.Serializable]
    public class MatchFirstFloatExpression : FloatExpressionBase
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression m_Condition;
            public FloatExpression m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal FloatExpression  m_DefaultExpr;

        public override float Evaluate(Dictionary<int, object> ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1)
            {
                return m_DefaultExpr.Evaluate(ctx);
            }
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

    [System.Serializable]
    public class IntToFloatCastExpression : FloatExpressionBase
    {
	    [SerializeField] internal IntExpression m_InnerExpr;
	    public override float Evaluate(Dictionary<int, object> ctx)
	    {
		    return m_InnerExpr.Evaluate(ctx);
	    }
    }

}
