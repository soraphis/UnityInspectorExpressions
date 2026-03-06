using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityInspectorExpressions.Expressions;

namespace Editor.Helpers
{
    public static class ManagedReferenceDrawerHelper
    {
        public static GenericMenu TypeSelectorMenu(SerializedProperty property, Rect position, Type[] ctxArgs = null, string nullLabel = "[null]")
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

            var fieldType = GetManagedReferenceFieldType(property);

            foreach (var type in GetAppropriateTypesForAssigningToManagedReference(fieldType, ctxArgs))
            {
                var label = GetMenuLabel(type);
                var closedType = type;
                context.AddItem(new GUIContent(label), false, () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(closedType);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            return context;
        }

        /// Returns the ExpressionLabelAttribute label if present, otherwise the type name.
        public static string GetMenuLabel(Type type)
        {
            var attr = type.GetCustomAttributes(typeof(ExpressionLabelAttribute), false);
            return attr.Length > 0 ? ((ExpressionLabelAttribute)attr[0]).Label : type.Name;
        }


        // https://github.com/TextusGames/UnitySerializedReferenceUI/blob/51aff6062610296c86f739a3e093c29d08388a95/Core/ManagedReferenceUtility.cs#L71

        /// Filters derived types of field type parameter and finds ones compatible with managed reference.
        /// Open generic types are closed with <paramref name="ctxArgs"/> when provided.
        public static IEnumerable<Type> GetAppropriateTypesForAssigningToManagedReference(Type fieldType, Type[] ctxArgs = null, List<Func<Type, bool>> filters = null)
        {
            var appropriateTypes = new List<Type>();

            var derivedTypes = TypeCache.GetTypesDerivedFrom(fieldType);
            foreach (var type in derivedTypes)
            {
                if (type.IsSubclassOf(typeof(UnityEngine.Object))) continue;
                if (type.IsAbstract) continue;

                var resolved = type;

                // Close open generics using the TCtx args from the field type
                if (type.ContainsGenericParameters)
                {
                    if (ctxArgs == null || type.GetGenericArguments().Length != ctxArgs.Length)
                        continue;
                    try { resolved = type.MakeGenericType(ctxArgs); }
                    catch { continue; }
                }

                if (resolved.IsClass && resolved.GetConstructor(Type.EmptyTypes) == null) continue;
                if (filters != null && filters.All(f => f == null || f.Invoke(resolved)) == false) continue;

                appropriateTypes.Add(resolved);
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

        /// Gets real type of managed reference's field typeName.
        /// Returns the open generic definition when the field is generic
        /// (e.g. BoolExpressionBase`1 for BoolExpressionBase<TCtx>).
        /// The caller is responsible for supplying the concrete TCtx args separately.
        public static Type GetRealTypeFromTypename(string stringType)
        {
            if (string.IsNullOrEmpty(stringType)) return null;

            var names = GetSplitNamesFromTypename(stringType);
            if (string.IsNullOrEmpty(names.AssemblyName)) return null;

            // Strip any generic args section from the class name (everything from '[' onward)
            var className = names.ClassName;
            var bracketIdx = className.IndexOf('[');
            if (bracketIdx >= 0) className = className.Substring(0, bracketIdx);

            return Type.GetType($"{className}, {names.AssemblyName}");
        }

        /// Get assembly and class names from Unity's managedReferenceFieldTypename format.
        /// Handles both plain ("Assembly ClassName") and generic ("Assembly ClassName`N[[...]]") formats.
        public static (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename)
        {
            if (string.IsNullOrEmpty(typename)) return ("", "");

            // Unity format: "AssemblyName ClassName" or "AssemblyName ClassName`N[[...]]"
            // Find the first space — everything before it is the assembly name.
            var spaceIdx = typename.IndexOf(' ');
            if (spaceIdx < 0) return ("", "");

            var assemblyName = typename.Substring(0, spaceIdx);
            var className    = typename.Substring(spaceIdx + 1);
            return (assemblyName, className);
        }
    }
}
