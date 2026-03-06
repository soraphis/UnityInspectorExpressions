using System.Linq;
using System.Runtime.Remoting.Contexts;
using Editor.Helpers;
using Unity.Properties;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{

    [CustomPropertyDrawer(typeof(FromContextGameObjectExpression<>))]
    public class ContextExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_PathToProperty";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var genericType = fieldInfo.FieldType.GenericTypeArguments.FirstOrDefault(x =>
                x.GetCustomAttributes(typeof(GeneratePropertyBagAttribute), true).Any()
            );
            
            var row = CustomStyles.MakeRow();

            if (genericType != null)
            {
                // collect all members of the type with [CreateProperty] and show them as dropdown:
                var propertyInfos = genericType.GetProperties()
                    .Where(x => x.GetCustomAttributes(typeof(CreatePropertyAttribute), true).Any())
                    .Select(x => x.Name)
                    .Append("None")
                    .ToList();


                var currentValue = property.FindPropertyRelative(s_PropertyName).stringValue;
                
                var field = new DropdownField(propertyInfos, string.IsNullOrEmpty(currentValue) ? "None" : currentValue);
                field.RegisterValueChangedCallback(evt =>
                {
                    property.FindPropertyRelative(s_PropertyName).stringValue = evt.newValue;
                    property.serializedObject.ApplyModifiedProperties();
                });
                row.Add(field);
            }
            else
            {
                // just show string field:
                var field = new PropertyField(property.FindPropertyRelative(s_PropertyName), "");
                row.Add(field);
            }

            return row;
        }
    }
}
