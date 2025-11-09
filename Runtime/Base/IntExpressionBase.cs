using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace UnityInspectorExpressions.Expressions.Base
{
    [System.Serializable]
    public abstract class IntExpressionBase : IExpression<int>, IWrapable<UnaryIntExpression>, IWrapable<BinaryIntExpression>
    {
        public abstract int Evaluate(Dictionary<int, object> ctx);
        public int Evaluate() => ((IExpression<int>)this).DefaultEvaluate();
        
        UnaryIntExpression IWrapable<UnaryIntExpression>.Wrap() => new UnaryIntExpression() { m_InnerExpr = new(this) };
        BinaryIntExpression IWrapable<BinaryIntExpression>.Wrap() => new BinaryIntExpression() { m_InnerExpr1 = new(this) };

    }

    [System.Serializable]
    public class LiteralIntExpression : IntExpressionBase
    {
        [SerializeField] internal int m_Literal;
        public LiteralIntExpression() { }
        public LiteralIntExpression(int literal) { m_Literal = literal; }
        public override int Evaluate(Dictionary<int, object> ctx) => m_Literal;

    }



    [System.Serializable]
    public class UnaryIntExpression : IntExpressionBase
    {
        public enum UnaryOperator
        {
            [InspectorName("-")] Negate,
            Sign,
            Square,
            // Sine, Cosine, Abs ...

        }
        [SerializeField] internal IntExpression m_InnerExpr;
        [SerializeField] internal UnaryOperator  m_Operator;
        public override int Evaluate(Dictionary<int, object> ctx)
        {
	        var f = m_InnerExpr.Evaluate(ctx);
	        return m_Operator switch
	        {
		        UnaryOperator.Negate => -f,
		        UnaryOperator.Sign => Math.Sign(f),
		        UnaryOperator.Square => f * f,
		        _ => throw new NotImplementedException(),
	        };
        }
    }

    [System.Serializable]
    public class BinaryIntExpression : IntExpressionBase
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
            [BinaryOperatorPosition(BinaryOperatorPosition.FunctionCall)]RandomRange,
            // step
        }

        [SerializeField] internal IntExpression  m_InnerExpr1;
        [SerializeField] internal IntExpression  m_InnerExpr2;
        [SerializeField] internal BinaryOperator  m_Operator;
        public override int Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            BinaryOperator.Max => Mathf.Max(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.Min => Mathf.Min(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            BinaryOperator.RandomRange => UnityEngine.Random.Range(m_InnerExpr1.Evaluate(ctx),m_InnerExpr2.Evaluate(ctx)),

            BinaryOperator.Add => m_InnerExpr1.Evaluate(ctx) + m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Subtract => m_InnerExpr1.Evaluate(ctx) - m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Multiply => m_InnerExpr1.Evaluate(ctx) * m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Divide => m_InnerExpr1.Evaluate(ctx) / m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Modulo => m_InnerExpr1.Evaluate(ctx) % m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    // TODO: ternary -> int?

    [System.Serializable]
    public class MatchFirstIntExpression : IntExpressionBase
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression m_Condition;
            public IntExpression m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal IntExpression  m_DefaultExpr;

        public override int Evaluate(Dictionary<int, object> ctx)
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
    public class FloatToIntCastExpression : IntExpressionBase
    {
	    // TODO: maybe make it a "unary" expression, where Floor/Ceil/Round/Cast can be chosen.

	    [SerializeField] internal FloatExpression m_InnerExpr;
	    public override int Evaluate(Dictionary<int, object> ctx)
	    {
		    return (int)m_InnerExpr.Evaluate(ctx);
	    }
    }


}
