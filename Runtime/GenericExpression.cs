using UnityInspectorExpressions.Expressions.Base;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions
{
    [System.Serializable]
    public struct GenericExpression<TCtx, TResult> : IExpression<TResult, TCtx>, ISerializationCallbackReceiver
    {
        [SerializeReference] internal GenericExpressionBase<TCtx, TResult> m_ExpressionRef;

        public GenericExpression(GenericExpressionBase<TCtx, TResult> @ref) : this() { m_ExpressionRef = @ref; }

        public static GenericExpression<TCtx, TResult> FromLiteral(TResult value) =>
            new(new LiteralGenericExpression<TCtx, TResult> { m_Literal = value });

        public TResult Evaluate(TCtx ctx) => m_ExpressionRef == null ? default : m_ExpressionRef.Evaluate(ctx);

        public void OnAfterDeserialize()  { m_ExpressionRef ??= new LiteralGenericExpression<TCtx, TResult>(); }
        public void OnBeforeSerialize()   { m_ExpressionRef ??= new LiteralGenericExpression<TCtx, TResult>(); }
    }
}
