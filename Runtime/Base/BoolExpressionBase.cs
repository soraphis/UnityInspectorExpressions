using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class BoolExpressionBase<TCtx> : IExpression<bool, TCtx>, IWrapable<UnaryBoolExpression<TCtx>>, IWrapable<BinaryBoolExpression<TCtx>>, IWrapable<ManyBoolExpression<TCtx>>
    {
        public abstract bool Evaluate(TCtx ctx);

        UnaryBoolExpression<TCtx> IWrapable<UnaryBoolExpression<TCtx>>.Wrap() => new UnaryBoolExpression<TCtx>() { m_InnerExpr = new() { m_ExpressionRef = this } };
        BinaryBoolExpression<TCtx> IWrapable<BinaryBoolExpression<TCtx>>.Wrap() => new BinaryBoolExpression<TCtx>() { m_InnerExpr1 = new() { m_ExpressionRef = this } };
        ManyBoolExpression<TCtx> IWrapable<ManyBoolExpression<TCtx>>.Wrap() => new ManyBoolExpression<TCtx>() { m_InnerExpr = new() { new() { m_ExpressionRef = this } } };
    }
    [System.Serializable]
    [ExpressionLabel("Bool/Literal")]
    public class LiteralBoolExpression<TCtx> : BoolExpressionBase<TCtx>
    {
        [SerializeField] internal bool m_Literal;
        public LiteralBoolExpression() { }
        public LiteralBoolExpression(bool literal) { m_Literal = literal; }
        public override bool Evaluate(TCtx ctx) => m_Literal;
    }

    public class FromContextBoolExpression<TCtx> : BoolExpressionBase<TCtx>
    {
        [SerializeField] internal string m_PathToProperty;

        public override bool Evaluate(TCtx ctx)
        {
            var propertyPath = new PropertyPath(m_PathToProperty);
            return PropertyContainer.GetValue<TCtx, bool>(ref ctx, propertyPath);
        }
    }

    [System.Serializable]
    [ExpressionLabel("Bool/Unary")]
    public class UnaryBoolExpression<TCtx> : BoolExpressionBase<TCtx>
    {
        public enum UnaryOperator
        {
            Not
        }
        [SerializeField] internal BoolExpression<TCtx> m_InnerExpr;
        [SerializeField] internal UnaryOperator  m_Operator;
        public override bool Evaluate(TCtx ctx) => m_Operator switch
        {
            UnaryOperator.Not => !m_InnerExpr.Evaluate(ctx),
            _ => throw new NotImplementedException(),
        };
    }
    [System.Serializable]
    [ExpressionLabel("Bool/Binary")]
    public class BinaryBoolExpression<TCtx> : BoolExpressionBase<TCtx>
    {
        public enum BinaryOperator
        {
            [InspectorName("==")] Equal,
            [InspectorName("!=")] Unequal,
            AND,
            OR,
            XOR
        }
        [SerializeField] internal BoolExpression<TCtx>  m_InnerExpr1;
        [SerializeField] internal BoolExpression<TCtx>  m_InnerExpr2;
        [SerializeField] internal BinaryOperator  m_Operator;
        public override bool Evaluate(TCtx ctx) => m_Operator switch
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
    [ExpressionLabel("Bool/Many")]
    public class ManyBoolExpression<TCtx> : BoolExpressionBase<TCtx>
    {
        public enum ManyOperator
        {
            All,
            Any,
        }
        [SerializeField] internal List<BoolExpression<TCtx>> m_InnerExpr;
        [SerializeField] internal ManyOperator               m_Operator;
        public override bool Evaluate(TCtx ctx) => m_Operator switch
        {
            ManyOperator.All => m_InnerExpr.All(x => x.Evaluate(ctx)),
            ManyOperator.Any => m_InnerExpr.Any(x => x.Evaluate(ctx)),
            _ => throw new NotImplementedException(),
        };
    }

    [ExpressionLabel("From Float/Relational")]
    public class FloatRelationalExpression<TCtx> : BoolExpressionBase<TCtx>
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
        [SerializeField] internal FloatExpression<TCtx>  m_InnerExpr1;
        [SerializeField] internal FloatExpression<TCtx>  m_InnerExpr2;
        [SerializeField] internal BinaryOperator  m_Operator;
        public override bool Evaluate(TCtx ctx) => m_Operator switch
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

    [ExpressionLabel("From Int/Relational")]
    public class IntRelationalExpression<TCtx> : BoolExpressionBase<TCtx>
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
	    [SerializeField] internal IntExpression<TCtx> m_InnerExpr1;
	    [SerializeField] internal IntExpression<TCtx> m_InnerExpr2;
	    [SerializeField] internal BinaryOperator  m_Operator;
	    public override bool Evaluate(TCtx ctx) => m_Operator switch
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

    [ExpressionLabel("From GameObject/Relational")]
    public class GameObjectRelationalExpression<TCtx> : BoolExpressionBase<TCtx>
    {
	    public enum BinaryOperator
	    {
		    [InspectorName("==")] Equal,
		    [InspectorName("!=")] Unequal,
		    [InspectorName("Is Child Of")] IsChildOf,
	    }
	    [SerializeField] internal GameObjectExpression<TCtx> m_InnerExpr1;
	    [SerializeField] internal GameObjectExpression<TCtx> m_InnerExpr2;
	    [SerializeField] internal BinaryOperator             m_Operator;
	    public override bool Evaluate(TCtx ctx) => m_Operator switch
	    {
		    BinaryOperator.Equal => m_InnerExpr1.Evaluate(ctx) == m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.Unequal => m_InnerExpr1.Evaluate(ctx) != m_InnerExpr2.Evaluate(ctx),
		    BinaryOperator.IsChildOf => m_InnerExpr1.Evaluate(ctx).transform.IsChildOf(m_InnerExpr2.Evaluate(ctx).transform),
		    _ => throw new NotImplementedException(),
	    };
    }
    

    public abstract class BoolResultFunctionExpression<TCtx, TObj, TArg0> : BoolExpressionBase<TCtx>
    {
	    [SerializeField] internal TObj m_Object;
	    [SerializeField] internal TArg0 m_Argument;
    }
    
    [ExpressionLabel("From GameObject/Fn: Compare Tag")]
    public class GameObject_CompareTag_FunctionExpression<TCtx> : BoolResultFunctionExpression<TCtx, GameObjectExpression<TCtx>, StringExpression<TCtx>>
	{
	    public override bool Evaluate(TCtx ctx)
	    {
		    var go = m_Object.Evaluate(ctx);
		    var tag = m_Argument.Evaluate(ctx);
		    return go.CompareTag(tag);
	    }
	}
    
	
    
}
