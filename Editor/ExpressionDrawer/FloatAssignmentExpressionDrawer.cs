using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Editor.Helpers;
using Unity.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(NumericAssignmentExpression<>))]
    public class NumericAssignmentExpressionDrawer : PropertyDrawer
    {
        const string s_PathPropertyName  = "m_PathToProperty";
        const string s_ValuePropertyName = "m_Value";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var pathProp  = property.FindPropertyRelative(s_PathPropertyName);
            var valueProp = property.FindPropertyRelative(s_ValuePropertyName);

            var row = CustomStyles.MakeHighlightableRow();

            // -- [prop] dropdown --------------------------------------------
            var genericType = FindContextType(fieldInfo.FieldType);

            VisualElement pathField;
            if (genericType != null)
            {
                var propertyNames = new List<string>();
                var propertyTypes = new Dictionary<string, System.Type>();

                var parentPropInfos = genericType.GetProperties()
                    .Where(x => x.GetCustomAttributes(typeof(CreatePropertyAttribute), true).Any())
                    .Where(x => x.PropertyType == typeof(int) || x.PropertyType == typeof(float))
                    .ToList();

                propertyNames.AddRange(parentPropInfos.Select(x => x.Name));
                foreach (var propInfo in parentPropInfos)
                    propertyTypes[propInfo.Name] = propInfo.PropertyType;

                foreach (var propInfo in parentPropInfos)
                {
                    var parentName = propInfo.Name;
                    var subtype    = propInfo.PropertyType;
                    if (subtype.GetCustomAttribute<GeneratePropertyBagAttribute>() is not null)
                    {
                        var nestedProperties = subtype.GetProperties()
                            .Where(x => x.GetCustomAttributes<CreatePropertyAttribute>(true).Any())
                            .Where(x => x.PropertyType == typeof(int) || x.PropertyType == typeof(float))
                            .ToList();
                        propertyNames.AddRange(nestedProperties.Select(x => parentName + "." + x.Name));
                        foreach (var nestedProperty in nestedProperties)
                            propertyTypes[parentName + "." + nestedProperty.Name] = nestedProperty.PropertyType;
                    }
                }

                propertyNames.Add("None");

                propertyNames.Sort();

                var currentValue = pathProp.stringValue;
                currentValue = string.IsNullOrEmpty(currentValue) ? "None" :
                    propertyNames.Contains(currentValue) ? currentValue : "None";

                var dropdown = new DropdownField(propertyNames, currentValue);
                dropdown.style.flexShrink = 0;
                dropdown.style.minWidth   = 140;
                dropdown.style.maxWidth   = 220;

                var pathPropCopy = pathProp.Copy();
                dropdown.RegisterValueChangedCallback(evt =>
                {
                    var selectedPath = evt.newValue == "None" ? "" : evt.newValue;
                    pathPropCopy.stringValue = selectedPath;
                    var integerFlag = property.FindPropertyRelative("m_IsIntegerPath");
                    if (integerFlag != null)
                        integerFlag.boolValue = propertyTypes.TryGetValue(selectedPath, out var propertyType) && propertyType == typeof(int);
                    pathPropCopy.serializedObject.ApplyModifiedProperties();
                });
                pathField = dropdown;
            }
            else
            {
                pathField = new PropertyField(pathProp, "");
                pathField.style.flexShrink = 0;
                pathField.style.minWidth   = 140;
                pathField.style.maxWidth   = 220;
            }

            // -- := value expression -----------------------------------------
            var valueField = new PropertyField(valueProp, "");
            valueField.style.flexGrow   = 1;
            valueField.style.flexShrink = 1;

            row.Add(pathField);
            row.Add(CustomStyles.MakeLabel(":="));
            row.Add(valueField);

            return row;
        }

        // fieldInfo.FieldType may be the raw element type, or wrapped in List<> (when used
        // behind [ArrayDrawer]), or further nested generics - recurse through generic arguments
        // until we find the one actually marked [GeneratePropertyBag].
        private static System.Type FindContextType(System.Type type)
        {
            if (type == null) return null;
            if (type.GetCustomAttributes(typeof(GeneratePropertyBagAttribute), true).Any())
                return type;

            if (!type.IsGenericType) return null;

            foreach (var arg in type.GetGenericArguments())
            {
                var found = FindContextType(arg);
                if (found != null) return found;
            }

            return null;
        }
    }
}
