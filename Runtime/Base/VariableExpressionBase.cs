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

    [System.Serializable]
    [ExpressionLabel("Numeric/From Variable")]
    public class VariableNumericExpression<TCtx, T> : NumericExpressionBase<TCtx>
    {
        [SerializeField] internal T m_Variable;

        public override float Evaluate(TCtx ctx)
        {
            if (m_Variable is IGetValue<float> floatValue) return floatValue.GetValue();
            if (m_Variable is IGetValue<int> intValue) return intValue.GetValue();
            throw new System.InvalidOperationException($"{typeof(T)} must implement IGetValue<float> or IGetValue<int>.");
        }
    }
}