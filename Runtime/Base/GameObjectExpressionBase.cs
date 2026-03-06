using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{
    [System.Serializable]
    public abstract class GameObjectExpressionBase<TCtx> : IExpression<GameObject, TCtx>
    {
        public abstract GameObject Evaluate(TCtx ctx);
    }

    [System.Serializable]
    [ExpressionLabel("GameObject/Literal")]
    public class LiteralGameObjectExpression<TCtx> : GameObjectExpressionBase<TCtx>
    {
        [SerializeField] internal GameObject m_Literal;
        public LiteralGameObjectExpression() { }
        public LiteralGameObjectExpression(GameObject literal) { m_Literal = literal; }
        public override GameObject Evaluate(TCtx ctx) => m_Literal;
    }

    public class FromContextGameObjectExpression<TCtx> : GameObjectExpressionBase<TCtx>
    {
        // TODO: see https://docs.unity3d.com/Packages/com.unity.properties@2.0/manual/index.html
        // context must be flagged as GeneratePropertyBag, and only [CreateProperty] members can be retrieved. 
        
        [SerializeField] internal string m_PathToProperty; 
        
        public override GameObject Evaluate(TCtx ctx)
        {
            var propertyPath = new PropertyPath(m_PathToProperty);
            PropertyVisitor visitor = null; // TODO
            return PropertyContainer.GetValue<TCtx, GameObject>(ref ctx, propertyPath);
        }
    }
    
    [System.Serializable]
    [ExpressionLabel("GameObject/Match First")]
    public class MatchFirstGameObjectExpression<TCtx> : GameObjectExpressionBase<TCtx>
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression<TCtx> m_Condition;
            public GameObjectExpression<TCtx> m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal GameObjectExpression<TCtx> m_DefaultExpr;

        public override GameObject Evaluate(TCtx ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1) return m_DefaultExpr.Evaluate(ctx);
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

    [System.Serializable]
    [ExpressionLabel("GameObject/from Component")]
    public class ComponentToGameObjectExpression<TCtx> : GameObjectExpressionBase<TCtx>
    {
        [SerializeField] internal ComponentExpression<TCtx> m_InnerExpr;
        public override GameObject Evaluate(TCtx ctx) => m_InnerExpr.Evaluate(ctx).gameObject;
    }
}
