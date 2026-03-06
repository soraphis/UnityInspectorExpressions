using Editor.Helpers;
using System;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(ManyBoolExpression))]
    public class ManyBoolExpressionDrawer : PropertyDrawer
    {
        const string s_OperatorPropertyName = "m_Operator";
        const string s_EntriesPropertyName  = "m_InnerExpr";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var operatorProp = property.FindPropertyRelative(s_OperatorPropertyName);
            var entriesProp  = property.FindPropertyRelative(s_EntriesPropertyName);

            // ── outer column ──────────────────────────────────────────────
            var root = new VisualElement();
            root.style.flexGrow = 1;

            // ── header row:  [operator] {    [+] ──────────────────────────
            var header = CustomStyles.MakeRow();
            header.style.marginBottom = 2;
            header.style.flexGrow     = 0;   // don't grow the header itself

            var opField = new PropertyField(operatorProp, "");
            opField.style.flexShrink = 0;
            opField.style.minWidth   = 60;
            opField.style.maxWidth   = 80;

            var lblBrace = new Label("{");
            lblBrace.style.flexShrink  = 0;
            lblBrace.style.paddingLeft = 4;
            lblBrace.style.flexGrow    = 1;    // pushes the + button to the right

            var addBtn = CustomStyles.MakeIconButton("d_Toolbar Plus");
            addBtn.tooltip = "Add case";
            addBtn.clicked += () =>
            {
                entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);
                CustomStyles.ResetSerializedChildren(entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1));
                entriesProp.serializedObject.ApplyModifiedProperties();
            };

            header.Add(opField);
            header.Add(lblBrace);
            header.Add(addBtn);
            root.Add(header);

            // ── body: indented list of items ─────────────────────────────
            var body = new VisualElement();
            body.style.paddingLeft = 10;
            root.Add(body);

            void RebuildList()
            {
                body.Clear();
                var so = entriesProp.serializedObject;
                for (int i = 0; i < entriesProp.arraySize; i++)
                {
                    var idx      = i;
                    var elemProp = entriesProp.GetArrayElementAtIndex(i);

                    var row = CustomStyles.MakeRow();
                    row.style.marginBottom = 2;

                    var delBtn = CustomStyles.MakeIconButton("d_TreeEditor.Trash", 16);
                    delBtn.tooltip  = "Delete";
                    delBtn.clicked += () =>
                    {
                        entriesProp.DeleteArrayElementAtIndex(idx);
                        entriesProp.serializedObject.ApplyModifiedProperties();
                        RebuildList();
                    };

                    var elem = new PropertyField(elemProp, "");
                    elem.style.flexGrow   = 1;
                    elem.style.flexShrink = 1;
                    elem.Bind(so);

                    row.Add(delBtn);
                    row.Add(elem);
                    body.Add(row);
                }
            }

            RebuildList();
            root.TrackPropertyValue(entriesProp, _ => RebuildList());

            // ── footer: closing brace ─────────────────────────────────────
            var footer = new Label("}");
            footer.style.paddingLeft = 4;
            footer.style.marginTop   = 2;
            root.Add(footer);

            return root;
        }
    }
}