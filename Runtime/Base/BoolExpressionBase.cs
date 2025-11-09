using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Expressions.Base
{

    [System.Serializable]
    public abstract class BoolExpressionBase : IExpression<bool>, IWrapable<UnaryBoolExpression>, IWrapable<BinaryBoolExpression>, IWrapable<ManyBoolExpression>
    {
        public abstract bool Evaluate(Dictionary<int, object> ctx);
        public bool Evaluate() => ((IExpression<bool>)this).DefaultEvaluate();


        UnaryBoolExpression IWrapable<UnaryBoolExpression>.Wrap() => new UnaryBoolExpression() { m_InnerExpr = new() { m_ExpressionRef = this } };
        BinaryBoolExpression IWrapable<BinaryBoolExpression>.Wrap() => new BinaryBoolExpression() { m_InnerExpr1 = new() { m_ExpressionRef = this } };
        ManyBoolExpression IWrapable<ManyBoolExpression>.Wrap() => new ManyBoolExpression() { m_InnerExpr = new() { new() { m_ExpressionRef = this } } };
    }
    [System.Serializable]
    public class LiteralBoolExpression : BoolExpressionBase
    {
        [SerializeField] internal bool m_Literal;
        public LiteralBoolExpression() { }
        public LiteralBoolExpression(bool literal) { m_Literal = literal; }
        public override bool Evaluate(Dictionary<int, object> ctx) => m_Literal;
    }

    [System.Serializable]
    public class UnaryBoolExpression : BoolExpressionBase
    {
        public enum UnaryOperator
        {
            Not
        }
        [SerializeField] internal BoolExpression m_InnerExpr;
        [SerializeField] internal UnaryOperator  m_Operator;
        public override bool Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            UnaryOperator.Not => !m_InnerExpr.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }
    [System.Serializable]
    public class BinaryBoolExpression : BoolExpressionBase
    {
        public enum BinaryOperator
        {
            [InspectorName("==")] Equal,
            [InspectorName("!=")] Unequal,
            AND,
            OR,
            XOR
        }
        [SerializeField] internal BoolExpression  m_InnerExpr1;
        [SerializeField] internal BoolExpression  m_InnerExpr2;
        [SerializeField] internal BinaryOperator  m_Operator;
        public override bool Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            BinaryOperator.Equal => m_InnerExpr1.Evaluate(ctx) == m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Unequal => m_InnerExpr1.Evaluate(ctx) != m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.AND => m_InnerExpr1.Evaluate(ctx) && m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.OR => m_InnerExpr1.Evaluate(ctx) || m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.XOR => m_InnerExpr1.Evaluate(ctx) ^ m_InnerExpr2.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }

    [System.Serializable]
    public class ManyBoolExpression : BoolExpressionBase
    {
        public enum ManyOperator
        {
            All,
            Any,
        }
        [SerializeField] internal List<BoolExpression> m_InnerExpr;
        [SerializeField] internal ManyOperator         m_Operator;
        public override bool Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            ManyOperator.All => m_InnerExpr.All(x => x.Evaluate(ctx)),
            ManyOperator.Any => m_InnerExpr.Any(x => x.Evaluate(ctx)),
            _ => throw new NotImplementedException(),
        };
    }

    public class FloatRelationalExpression : BoolExpressionBase
    {
        public enum BinaryOperator
        {
            [InspectorName("==")] Equal,
            [InspectorName("!=")] Unequal,
            [InspectorName(">=")] GreaterEqual,
            [InspectorName(">")] Greater,
            [InspectorName("<")] Less,
            [InspectorName("<=")] LessEqual,
            [InspectorName("\u2248")] Approx,

        }
        [SerializeField] internal FloatExpression  m_InnerExpr1;
        [SerializeField] internal FloatExpression  m_InnerExpr2;
        [SerializeField] internal BinaryOperator  m_Operator;
        public override bool Evaluate(Dictionary<int, object> ctx) => m_Operator switch
        {
            BinaryOperator.Equal => m_InnerExpr1.Evaluate(ctx) == m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Unequal => m_InnerExpr1.Evaluate(ctx) != m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.GreaterEqual => m_InnerExpr1.Evaluate(ctx) >= m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Greater => m_InnerExpr1.Evaluate(ctx) > m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Less => m_InnerExpr1.Evaluate(ctx) < m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.LessEqual => m_InnerExpr1.Evaluate(ctx) <= m_InnerExpr2.Evaluate(ctx),
            BinaryOperator.Approx => Mathf.Approximately(m_InnerExpr1.Evaluate(ctx), m_InnerExpr2.Evaluate(ctx)),
            _ => throw new NotImplementedException(),
        };
    }

    public class IntRelationalExpression : BoolExpressionBase
    {
	    public enum BinaryOperator
	    {
		    [InspectorName("==")] Equal,
		    [InspectorName("!=")] Unequal,
		    [InspectorName(">=")] GreaterEqual,
		    [InspectorName(">")]  Greater,
		    [InspectorName("<")]  Less,
		    [InspectorName("<=")] LessEqual,

	    }
	    [SerializeField] internal IntExpression m_InnerExpr1;
	    [SerializeField] internal IntExpression m_InnerExpr2;
	    [SerializeField] internal BinaryOperator  m_Operator;
	    public override bool Evaluate(Dictionary<int, object> ctx) => m_Operator switch
	    {
		    BinaryOperator.Equal => m_InnerExpr1.Evaluate(ctx) == m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.Unequal => m_InnerExpr1.Evaluate(ctx) != m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.GreaterEqual => m_InnerExpr1.Evaluate(ctx) >= m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.Greater => m_InnerExpr1.Evaluate(ctx) > m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.Less => m_InnerExpr1.Evaluate(ctx) < m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.LessEqual => m_InnerExpr1.Evaluate(ctx) <= m_InnerExpr2.Evaluate(ctx),
		    _ => throw new NotImplementedException(),
	    };
    }

    public class GameObjectRelationalExpression : BoolExpressionBase
    {
	    public enum BinaryOperator
	    {
		    [InspectorName("==")] Equal,

	    }
	    [SerializeField] internal GameObjectExpression m_InnerExpr1;
	    [SerializeField] internal GameObjectExpression m_InnerExpr2;
	    [SerializeField] internal BinaryOperator       m_Operator;
	    public override bool Evaluate(Dictionary<int, object> ctx) => m_Operator switch
	    {
		    BinaryOperator.Equal => m_InnerExpr1.Evaluate(ctx) == m_InnerExpr2.Evaluate(ctx),
		    _ => throw new NotImplementedException(),
	    };
    }
}
