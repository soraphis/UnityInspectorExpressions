using System;
using Editor.Helpers;
using Project.Expressions.Base;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Project.Expressions
{
    [CustomPropertyDrawer(typeof(MatchFirstFloatExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstIntExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstComponentExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstGameObjectExpression))]
    public class MatchFirstFloatExpressionDrawer : PropertyDrawer
    {
        const string s_DefaultExprPropertyName = "m_DefaultExpr";
        const string s_EntriesPropertyName = "m_Entries";

        private ReorderableList reorderableList;
        private Action          delayedAction;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var entriesProp = property.FindPropertyRelative(s_EntriesPropertyName);

            return EditorGUIUtility.singleLineHeight // label line
                + Mathf.Max(1, entriesProp.arraySize) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)
                    + 10 // reorderable list padding
                + EditorGUIUtility.singleLineHeight // default line
                ;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var entriesProp = property.FindPropertyRelative(s_EntriesPropertyName);
            var defaultProp = property.FindPropertyRelative(s_DefaultExprPropertyName);

            var labelRect = position.CutTop(EditorGUIUtility.singleLineHeight, out position)
                .Padding(0, 10, 0, 0)
                .CutLeft(position.width - 55, out var plusBtnRect);
            labelRect.width = 100;
            GUI.Label(labelRect, new GUIContent("match first {"));

            //position.xMin = EditorGUIUtility.labelWidth + 10;
            position.xMin +=  10;

            var delBtnContent = EditorGUIUtility.IconContent("d_TreeEditor.Trash");
            delBtnContent.tooltip = "Delete";

            var lastLine = position.CutBottom(EditorGUIUtility.singleLineHeight, out position);

            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(entriesProp.serializedObject, entriesProp)
                {
                    elementHeight = EditorGUIUtility.singleLineHeight,
                    drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
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

            lastLine.xMin += 105;
            GUI.Label(lastLine.RowPrepend(60), new GUIContent("default"), CustomStyles.centerdLabel);
            lastLine.xMin += 30;
            GUI.Label(lastLine.RowPrepend(30), new GUIContent("\u21D2"), CustomStyles.centerdLabel);

            lastLine.xMax -= 10;
            EditorGUI.PropertyField(lastLine, defaultProp, GUIContent.none);
            GUI.Label(new Rect(lastLine) { xMin = lastLine.xMax, width = 10 }, new GUIContent("}"));

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

    // ///////////////////////////////////////////////////////////////////
    //                  MATCH-ENTRY DRAWER
    // ///////////////////////////////////////////////////////////////////


    [CustomPropertyDrawer(typeof(MatchFirstFloatExpression.MatchEntry))]
    [CustomPropertyDrawer(typeof(MatchFirstIntExpression.MatchEntry))]
    [CustomPropertyDrawer(typeof(MatchFirstComponentExpression.MatchEntry))]
    public class MatchFirstFloatExpression_MatchEntryDrawer : PropertyDrawer
    {
        const string s_ConditionPropertyName = "m_Condition";
        const string s_ResultPropertyName = "m_Result";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var condProp = property.FindPropertyRelative(s_ConditionPropertyName);
            var resuProp = property.FindPropertyRelative(s_ResultPropertyName);
            position.height = EditorGUIUtility.singleLineHeight;

            var countConditionChilds = 0;
            var countResultChilds = 0;
            foreach (var child in condProp.Copy()) { countConditionChilds++; }
            foreach (var child in resuProp.Copy()) { countResultChilds++; }


            var rects = new SpanQueue<Rect>(stackalloc Rect[3]);
            using (var x = position.Row(rects))
            {
                x.Flex(countConditionChilds, 60);
                x.Container(30);
                x.Flex(countResultChilds, 60);
            }

            EditorGUI.PropertyField(rects.Next(), condProp, GUIContent.none);
            GUI.Label(rects.Next(), new GUIContent("\u21D2"), CustomStyles.centerdLabel);
            EditorGUI.PropertyField(rects.Next(), resuProp, GUIContent.none);
        }
    }

}
