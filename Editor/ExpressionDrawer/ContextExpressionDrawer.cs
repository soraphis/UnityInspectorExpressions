using Editor.Helpers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityInspectorExpressions.Expressions.Base;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(ContextGameObjectExpression))]
    public class ContextExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_ContextSlot";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var literalProp = property.FindPropertyRelative(s_PropertyName);

            var row = CustomStyles.MakeRow();
            row.Add(CustomStyles.MakeLabel("Context["));
            row.Add(new PropertyField(literalProp, "").WithFlex(1, 1));
            row.Add(CustomStyles.MakeLabel("]"));
            return row;
        }
    }
}