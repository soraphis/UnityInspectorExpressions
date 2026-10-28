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
    [CustomPropertyDrawer(typeof(FromContextNumericExpression<>))]
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
                var numericPropertyTypes = new Dictionary<string, System.Type>();
                var isNumericExpression = property.FindPropertyRelative("m_IsInteger") != null;

                var parentPropInfos = genericType.GetProperties()
                    .Where(x => x.GetCustomAttributes(typeof(CreatePropertyAttribute), true).Any());

                foreach (var propInfo in parentPropInfos)
                {
                    if (isNumericExpression && !IsNumericType(propInfo.PropertyType)) continue;
                    propertyNames.Add(propInfo.Name);
                    numericPropertyTypes[propInfo.Name] = propInfo.PropertyType;
                }

                foreach (var propInfo in parentPropInfos)
                {
                    var parentName = propInfo.Name;
                    var subtype = propInfo.PropertyType;
                    if (subtype.GetCustomAttribute<GeneratePropertyBagAttribute>() is { } bag)
                    {
                        var nestedProperties = subtype.GetProperties()
                            .Where(x => x.GetCustomAttributes<CreatePropertyAttribute>(true).Any())
                            .Where(x => !isNumericExpression || IsNumericType(x.PropertyType))
                            .ToList();
                        propertyNames.AddRange(nestedProperties.Select(x => parentName + "." + x.Name));
                        foreach (var nestedProperty in nestedProperties)
                            numericPropertyTypes[parentName + "." + nestedProperty.Name] = nestedProperty.PropertyType;
                    }
                }

                propertyNames.Add("None");

                // collect all members of the type with [CreateProperty] and show them as dropdown:
                propertyNames.Sort();


                var currentValue = property.FindPropertyRelative(s_PropertyName).stringValue;

                currentValue = string.IsNullOrEmpty(currentValue) ? "None" :
                    propertyNames.Contains(currentValue) ? currentValue : "None";

                var field = new DropdownField(propertyNames, currentValue);
                var propCopy = property.Copy();
                field.RegisterValueChangedCallback(evt =>
                {
                    var selectedPath = evt.newValue == "None" ? "" : evt.newValue;
                    propCopy.FindPropertyRelative(s_PropertyName).stringValue = selectedPath;
                    var integerFlag = propCopy.FindPropertyRelative("m_IsInteger");
                    if (integerFlag != null)
                        integerFlag.boolValue = numericPropertyTypes.TryGetValue(selectedPath, out var propertyType) && propertyType == typeof(int);
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

        private static bool IsNumericType(System.Type type) => type == typeof(int) || type == typeof(float);
    }
}
