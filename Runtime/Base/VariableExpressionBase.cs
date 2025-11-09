using System.Collections.Generic;
using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    public interface IGetValue<TInner>{ TInner GetValue(); }
    
    public class VariableBoolExpressionBase<T> : BoolExpressionBase
        where T : IGetValue<bool>
    {
        [SerializeField] internal T m_Variable;

        public override bool Evaluate(Dictionary<int, object> ctx)
            => m_Variable.GetValue();
    }

    public class VariableFloatExpression<T> : FloatExpressionBase
        where T : IGetValue<float>
    {
        [SerializeField] internal T m_Variable;

        public override float Evaluate(Dictionary<int, object> ctx)
            => m_Variable.GetValue();
    }
    
    public class VariableIntExpression<T> : IntExpressionBase
        where T : IGetValue<int>
    {
        [SerializeField] internal T m_Variable;

        public override int Evaluate(Dictionary<int, object> ctx)
            => m_Variable.GetValue();
    }
    
}