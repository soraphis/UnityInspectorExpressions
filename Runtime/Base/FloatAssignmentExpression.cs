using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct FloatAssignmentExpression<TCtx>
    {
        [SerializeField] private string m_PathToProperty;
        [SerializeField] private FloatExpression<TCtx> m_Value;

        public TCtx Apply(TCtx ctx)
        {
            var path = new PropertyPath(m_PathToProperty);
            PropertyContainer.SetValue(ref ctx, path, m_Value.Evaluate(ctx));
            return ctx;
        }
    }
}
