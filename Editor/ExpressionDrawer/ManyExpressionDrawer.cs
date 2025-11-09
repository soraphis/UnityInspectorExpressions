using System;
using Editor.Helpers;
using Project.Expressions.Base;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Project.Expressions
{
    [CustomPropertyDrawer(typeof(ManyBoolExpression))]
    public class ManyBoolExpressionDrawer : PropertyDrawer
    {
        const string s_OperatorPropertyName = "m_Operator";
        const string s_EntriesPropertyName = "m_InnerExpr";
        
        private ReorderableList reorderableList;
        private Action          delayedAction;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var operatorProp = property.FindPropertyRelative(s_OperatorPropertyName);
            var entriesProp = property.FindPropertyRelative(s_EntriesPropertyName);

            if (entriesProp.arraySize < 4)
            {
                // TODO: render in a single line
            }

            float arrayHeightSum = 0;
            for (int i = 0; i < entriesProp.arraySize; ++i)
            {
                arrayHeightSum += EditorGUI.GetPropertyHeight(entriesProp.GetArrayElementAtIndex(i), GUIContent.none) 
                                  + 2; // spacing between elements
            }
            
            return EditorGUI.GetPropertyHeight(operatorProp, GUIContent.none) +
                   +arrayHeightSum
                   + 10 // reorderable list padding;
                ;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var operatorProp = property.FindPropertyRelative(s_OperatorPropertyName);
            var entriesProp = property.FindPropertyRelative(s_EntriesPropertyName);
            
            var labelRect = position.CutTop(EditorGUIUtility.singleLineHeight, out position)
                .Padding(0, 10, 0, 0)
                .CutLeft(position.width - 55, out var plusBtnRect);
            labelRect.width = 100;
            EditorGUI.PropertyField(labelRect, operatorProp, GUIContent.none);
            GUI.Label(labelRect.Shift(100, 0), new GUIContent("{"));
            
            position.xMin +=  10;
            
            var delBtnContent = EditorGUIUtility.IconContent("d_TreeEditor.Trash");
            delBtnContent.tooltip = "Delete";
            
            var lastLine = position.CutBottom(EditorGUIUtility.singleLineHeight, out position);
            if (reorderableList == null)
            {
                // TODO: ReorderableList does not support variable height elements. Needs replacement
                reorderableList = new ReorderableList(entriesProp.serializedObject, entriesProp)
                {
                    elementHeight = EditorGUIUtility.singleLineHeight,
                    drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        // -----
                        // address variable heights, dirty fix. does not look nice.
                        float arrayHeightSum = 0;
                        for (int i = 0; i < index; ++i)
                        {
                            arrayHeightSum += EditorGUI.GetPropertyHeight(entriesProp.GetArrayElementAtIndex(i), GUIContent.none) 
                                              + 2; // spacing between elements
                        }
                        var y = arrayHeightSum - ((EditorGUIUtility.singleLineHeight+2) * index);
                        rect.y += y;
                        // ----
                        
                        rect = rect.Padding(30, 0, 0, 0);
                        
                        var element = entriesProp.GetArrayElementAtIndex(index);

                        // Draw the property field for the current element
                        EditorGUI.PropertyField(rect, element, GUIContent.none);

                        if (GUI.Button(rect.RowPrepend(16).Padding(0, 4, 0, 0).Shift(0, 1), delBtnContent, EditorStyles.iconButton))
                        {
                            var indexScoped = index;
                            delayedAction += () => entriesProp.DeleteArrayElementAtIndex(indexScoped);
                        }

                    },
                    drawHeaderCallback = (Rect rect) => { },
                    displayAdd = false,
                    displayRemove = false,
                    headerHeight = 0,
                };
            }
            reorderableList.DoList(position.Padding(0, 10, 1, 3));
            
            delayedAction?.Invoke();
            delayedAction = null;

            var newBtnContent = EditorGUIUtility.IconContent("d_Toolbar Plus");
            newBtnContent.tooltip = "Add case";
            if (GUI.Button(plusBtnRect, newBtnContent))
            {
                entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);
                ResetChild(entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1));
            }

            lastLine.CutLeft(lastLine.width - 10, out lastLine);
            GUI.Label(lastLine, new GUIContent("}"));

        }
        private void ResetChild(SerializedProperty serializedProperty)
        {
            foreach (SerializedProperty child in serializedProperty)
            {
                if (child.propertyType == SerializedPropertyType.ManagedReference)
                {
                    child.managedReferenceValue = null;
                }
                else if (child.propertyType == SerializedPropertyType.ArraySize)
                {
                    child.arraySize = 0;
                }
            }
        }
    }
}