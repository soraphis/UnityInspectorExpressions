using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityInspectorExpressions.Expressions.Base
{

    [System.Serializable]
    public abstract class GameObjectExpressionBase : IExpression<GameObject>
    {
        public abstract GameObject Evaluate(Dictionary<int, object> ctx);
        public GameObject Evaluate() => ((IExpression<GameObject>)this).DefaultEvaluate();

    }

    [System.Serializable]
    public class LiteralGameObjectExpression : GameObjectExpressionBase
    {
        [SerializeField] internal GameObject m_Literal;
        public LiteralGameObjectExpression() { }
        public LiteralGameObjectExpression(GameObject literal) { m_Literal = literal; }
        public override GameObject Evaluate(Dictionary<int, object> ctx) => m_Literal;
    }

    [System.Serializable]
    public class DynamicGameObjectExpression : GameObjectExpressionBase
    {
        [SerializeField] internal int m_DynamicSlot;
        public override GameObject Evaluate(Dictionary<int, object> ctx) => ctx[m_DynamicSlot] as GameObject;
    }

    [System.Serializable]
    public class MatchFirstGameObjectExpression : GameObjectExpressionBase
    {
        [System.Serializable]
        public struct MatchEntry
        {
            public BoolExpression m_Condition;
            public GameObjectExpression m_Result;
        }

        [SerializeField] internal List<MatchEntry> m_Entries;
        [SerializeField] internal GameObjectExpression  m_DefaultExpr;

        public override GameObject Evaluate(Dictionary<int, object> ctx)
        {
            var idx = m_Entries.FindIndex(x => x.m_Condition.Evaluate(ctx));
            if (idx == -1)
            {
                return m_DefaultExpr.Evaluate(ctx);
            }
            return m_Entries[idx].m_Result.Evaluate(ctx);
        }
    }

    [System.Serializable]
    public class ComponentToGameObjectExpression : GameObjectExpressionBase
    {
	    [SerializeField] internal ComponentExpression m_InnerExpr;
	    public override GameObject Evaluate(Dictionary<int, object> ctx) => m_InnerExpr.Evaluate(ctx).gameObject;
    }

}
