using System.Linq;
using System.Reflection;
using System.Collections.Generic;
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
    [CustomPropertyDrawer(typeof(FromContextVector2Expression<>))]
    [CustomPropertyDrawer(typeof(FromContextVector3Expression<>))]
    [CustomPropertyDrawer(typeof(FromContextComponentExpression<>))]
    [CustomPropertyDrawer(typeof(FromContextBoolExpression<>))]
    [CustomPropertyDrawer(typeof(FromContextFloatExpression<>))]
    [CustomPropertyDrawer(typeof(FromContextIntExpression<>))]
    [CustomPropertyDrawer(typeof(FromContextStringExpression<>))]
    public class ContextExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_PathToProperty";

        private static string s_ClipboardPath;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var genericType = fieldInfo.FieldType.GenericTypeArguments.FirstOrDefault(x =>
                x.GetCustomAttributes(typeof(GeneratePropertyBagAttribute), true).Any()
            );

            var row = CustomStyles.MakeRow();

            var pathProp = property.FindPropertyRelative(s_PropertyName);
            row.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Copy", _ => s_ClipboardPath = pathProp.stringValue);
                evt.menu.AppendAction("Paste",
                    _ =>
                    {
                        Undo.RecordObject(pathProp.serializedObject.targetObject, "Paste Context Path");
                        pathProp.stringValue = s_ClipboardPath;
                        pathProp.serializedObject.ApplyModifiedProperties();
                    },
                    s_ClipboardPath != null ? DropdownMenuAction.AlwaysEnabled : DropdownMenuAction.AlwaysDisabled);
            }));

            if (genericType != null)
            {
                List<string> propertyNames = new List<string>();

                var parentPropInfos = genericType.GetProperties()
                    .Where(x => x.GetCustomAttributes(typeof(CreatePropertyAttribute), true).Any());


                propertyNames.AddRange(parentPropInfos
                    .Select(x => x.Name)
                    .Append("None")
                    .ToList());

                foreach (var propInfo in parentPropInfos)
                {
                    var parentName = propInfo.Name;
                    var subtype = propInfo.PropertyType;
                    if (subtype.GetCustomAttribute<GeneratePropertyBagAttribute>() is { } bag)
                    {
                        propertyNames.AddRange(subtype.GetProperties()
                            .Where(x => x.GetCustomAttributes<CreatePropertyAttribute>(true).Any())
                            .Select(x => parentName + "." + x.Name));
                    }
                }

                // collect all members of the type with [CreateProperty] and show them as dropdown:
                propertyNames.Sort();


                var currentValue = property.FindPropertyRelative(s_PropertyName).stringValue;

                currentValue = string.IsNullOrEmpty(currentValue) ? "None" :
                    propertyNames.Contains(currentValue) ? currentValue : "None";

                var field = new DropdownField(propertyNames, currentValue);
                var propCopy = property.Copy();
                field.RegisterValueChangedCallback(evt =>
                {
                    propCopy.FindPropertyRelative(s_PropertyName).stringValue = evt.newValue;
                    propCopy.serializedObject.ApplyModifiedProperties();
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
