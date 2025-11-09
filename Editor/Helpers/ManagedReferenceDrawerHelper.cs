using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.Helpers
{
    public static class ManagedReferenceDrawerHelper
    {
        public static GenericMenu TypeSelectorMenu(SerializedProperty property, Rect position, string nullLabel = "[null]")
        {
            var context = new GenericMenu();

            if (nullLabel != null)
            {
                context.AddItem(new GUIContent(nullLabel), false, () =>
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            foreach (var type in GetAppropriateTypesForAssigningToManagedReference(GetManagedReferenceFieldType(property)))
            {
                context.AddItem(new GUIContent(type.Name), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            return context;
        }


        // https://github.com/TextusGames/UnitySerializedReferenceUI/blob/51aff6062610296c86f739a3e093c29d08388a95/Core/ManagedReferenceUtility.cs#L71

        /// Filters derived types of field typ parameter and finds ones whose are compatible with managed reference and filters.
        public static IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference(Type fieldType, List<Func<Type, bool>> filters = null)
        {
            var appropriateTypes = new List<Type>();

            var derivedTypes = TypeCache.GetTypesDerivedFrom(fieldType);
            foreach (var type in derivedTypes)
            {
                if (type.IsSubclassOf(typeof(UnityEngine.Object))) continue;
                if (type.IsAbstract) continue;
                if (type.ContainsGenericParameters) continue;
                if (type.IsClass && type.GetConstructor(Type.EmptyTypes) == null) continue;
                if (filters != null && filters.All(f => f == null || f.Invoke(type)) == false) continue;

                appropriateTypes.Add(type);
            }

            return appropriateTypes;
        }

        /// Gets real type of managed reference
        public static Type GetManagedReferenceFieldType(SerializedProperty property)
        {
            var realPropertyType = GetRealTypeFromTypename(property.managedReferenceFieldTypename);
            if (realPropertyType != null)
                return realPropertyType;

            Debug.LogError($"Can not get field type of managed reference : {property.managedReferenceFieldTypename}");
            return null;
        }

        /// Gets real type of managed reference's field typeName
        public static Type GetRealTypeFromTypename(string stringType)
        {
            var names = GetSplitNamesFromTypename(stringType);
            var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
            return realType;
        }

        /// Get assembly and class names from typeName
        public static (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                return ("", "");

            var typeSplitString = typename.Split(char.Parse(" "));
            var typeClassName = typeSplitString[1];
            var typeAssemblyName = typeSplitString[0];
            return (typeAssemblyName, typeClassName);
        }
    }
}
