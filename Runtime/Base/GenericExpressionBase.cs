using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    [Serializable]
    public abstract class GenericExpressionBase<TCtx, TResult> : IExpression<TResult, TCtx>
    {
        public abstract TResult Evaluate(TCtx ctx);
    }

    [Serializable]
    [ExpressionLabel("Literal")]
    public class LiteralGenericExpression<TCtx, TResult> : GenericExpressionBase<TCtx, TResult>
    {
        [SerializeField] internal TResult m_Literal;
        public override TResult Evaluate(TCtx ctx) => m_Literal;
    }

    [Serializable]
    [ExpressionLabel("Match First")]
    public class MatchFirstGenericExpression<TCtx, TResult> : GenericExpressionBase<TCtx, TResult>
    {
        [Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public GenericExpression<TCtx, TResult> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries = new();
        [SerializeField] internal GenericExpression<TCtx, TResult> m_DefaultExpr;

        public override TResult Evaluate(TCtx ctx)
        {
            var idx = m_Entries?.FindIndex(x => x.m_Condition.Evaluate(ctx)) ?? -1;
            return idx == -1 ? m_DefaultExpr.Evaluate(ctx) : m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

    public interface IPropertySetter<TCtx, TResult>
    {
        void Apply(TResult target, TCtx ctx);
    }

    [Serializable]
    [ExpressionLabel("Set/Numeric")]
    public class NumericPropertySetter<TCtx, TResult> : IPropertySetter<TCtx, TResult>
    {
        [SerializeField] internal string m_Path;
        public NumericExpression<TCtx> m_Value;
        public void Apply(TResult target, TCtx ctx) =>
            PropertyContainer.SetValue(ref target, new PropertyPath(m_Path), m_Value.Evaluate(ctx));
    }

    [Serializable]
    [ExpressionLabel("Set/Int")]
    public class IntPropertySetter<TCtx, TResult> : IPropertySetter<TCtx, TResult>
    {
        [SerializeField] internal string m_Path;
        public IntExpression<TCtx> m_Value;
        public void Apply(TResult target, TCtx ctx) =>
            PropertyContainer.SetValue(ref target, new PropertyPath(m_Path), m_Value.Evaluate(ctx));
    }

    [Serializable]
    [ExpressionLabel("Set/Bool")]
    public class BoolPropertySetter<TCtx, TResult> : IPropertySetter<TCtx, TResult>
    {
        [SerializeField] internal string m_Path;
        public BoolExpression<TCtx> m_Value;
        public void Apply(TResult target, TCtx ctx) =>
            PropertyContainer.SetValue(ref target, new PropertyPath(m_Path), m_Value.Evaluate(ctx));
    }

    [Serializable]
    [ExpressionLabel("With")]
    public class WithGenericExpression<TCtx, TResult> : GenericExpressionBase<TCtx, TResult>
        where TResult : class, ICloneable
    {
        public GenericExpression<TCtx, TResult> m_Base;
        [SerializeReference] public List<IPropertySetter<TCtx, TResult>> m_Overrides = new();

        public override TResult Evaluate(TCtx ctx)
        {
            var result = (TResult)m_Base.Evaluate(ctx)?.Clone();
            if (result == null) return null;
            foreach (var setter in m_Overrides)
                setter.Apply(result, ctx);
            return result;
        }
    }
}
