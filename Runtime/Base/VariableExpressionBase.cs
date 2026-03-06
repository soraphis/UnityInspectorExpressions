using System.Collections.Generic;
using UnityEngine;
using UnityInspectorExpressions.Expressions.Base;

namespace UnityInspectorExpressions.Expressions.Base
{
    public interface IGetValue<TInner> { TInner GetValue(); }

    public class VariableBoolExpressionBase<TCtx, T> : BoolExpressionBase<TCtx>
        where T : IGetValue<bool>
    {
        [SerializeField] internal T m_Variable;
        public override bool Evaluate(TCtx ctx) => m_Variable.GetValue();
    }

    public class VariableFloatExpression<TCtx, T> : FloatExpressionBase<TCtx>
        where T : IGetValue<float>
    {
        [SerializeField] internal T m_Variable;
        public override float Evaluate(TCtx ctx) => m_Variable.GetValue();
    }

    public class VariableIntExpression<TCtx, T> : IntExpressionBase<TCtx>
        where T : IGetValue<int>
    {
        [SerializeField] internal T m_Variable;
        public override int Evaluate(TCtx ctx) => m_Variable.GetValue();
    }
}