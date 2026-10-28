using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct NumericAssignmentExpression<TCtx>
    {
        [SerializeField] private string m_PathToProperty;
        [SerializeField] private NumericExpression<TCtx> m_Value;
        [SerializeField] private bool m_IsIntegerPath;

        public TCtx Apply(TCtx ctx)
        {
            var path = new PropertyPath(m_PathToProperty);
            var value = m_Value.Evaluate(ctx);
            if (m_IsIntegerPath)
                PropertyContainer.SetValue(ref ctx, path, IntExpression<TCtx>.Round(value));
            else
                PropertyContainer.SetValue(ref ctx, path, value);
            return ctx;
        }
    }
}
