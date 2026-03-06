using System.Collections.Generic;
using GameplayTags.Runtime;
using UnityEngine;
using UnityEngine.UIElements;
using UnityInspectorExpressions.Expressions.Base;


namespace UnityInspectorExpressions.Expressions
{


    [System.Serializable]
    [ExpressionLabel("Int/From Gameplay Tag")]
    public class GameplayTagExpression : IntExpressionBase
    {
        [SerializeField] internal GameplayTag m_Literal;
        public override int Evaluate(Dictionary<int, object> ctx) => m_Literal.InnerId.GetHashCode();
    }
    
    [ExpressionLabel("From GameplayTag/Fn: HashCode")]
    public class Int_GameplayTag_FunctionExpression : IntResultFunctionExpression<GameplayTag>
    {
        public override int Evaluate(Dictionary<int, object> ctx)
        {
            return m_Object.InnerId.GetHashCode();
        }
    }
    

    #if UNITY_EDITOR
    namespace Editor
    {
        using UnityEditor;
        using UnityEditor.UIElements;

        [CustomPropertyDrawer(typeof(GameplayTagExpression))]
        public class LiteralExpressionDrawer : PropertyDrawer
        {
            const string s_PropertyName = "m_Literal";

            public override VisualElement CreatePropertyGUI(SerializedProperty property)
            {
                var literalProp = property.FindPropertyRelative(s_PropertyName);
                var field = new PropertyField(literalProp, "");
                field.style.flexGrow = 1;
                field.style.flexShrink = 1;
                return field;
            }
        }
        
    }
    #endif
}