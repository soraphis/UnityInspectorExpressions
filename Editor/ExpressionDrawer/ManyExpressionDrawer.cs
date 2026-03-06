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
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems    = Align.Center;
            header.style.marginBottom  = 2;

            var opField = new PropertyField(operatorProp, "");
            opField.style.flexShrink = 0;
            opField.style.minWidth   = 60;
            opField.style.maxWidth   = 80;

            var lblBrace = new Label("{");
            lblBrace.style.flexShrink  = 0;
            lblBrace.style.paddingLeft = 4;
            lblBrace.style.flexGrow    = 1;    // pushes the + button to the right

            var addBtn = new Button(() =>
            {
                entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);
                ResetChild(entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1));
                entriesProp.serializedObject.ApplyModifiedProperties();
            });
            addBtn.style.flexShrink     = 0;
            addBtn.style.width          = 20;
            addBtn.style.height         = 20;
            addBtn.style.paddingLeft    = 0;
            addBtn.style.paddingRight   = 0;
            addBtn.style.paddingTop     = 0;
            addBtn.style.paddingBottom  = 0;
            var addIcon = EditorGUIUtility.IconContent("d_Toolbar Plus");
            addBtn.Add(new Image { image = addIcon.image as Texture2D, scaleMode = ScaleMode.ScaleToFit, style = { flexGrow = 1 } });
            addBtn.tooltip = "Add case";

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
                    var idx         = i;
                    var elemProp    = entriesProp.GetArrayElementAtIndex(i);

                    var row = new VisualElement();
                    row.style.flexDirection = FlexDirection.Row;
                    row.style.alignItems   = Align.Center;
                    row.style.marginBottom = 2;

                    var delBtn = new Button();
                    delBtn.style.flexShrink    = 0;
                    delBtn.style.width         = 16;
                    delBtn.style.height        = 16;
                    delBtn.style.paddingLeft   = 0;
                    delBtn.style.paddingRight  = 0;
                    delBtn.style.paddingTop    = 0;
                    delBtn.style.paddingBottom = 0;
                    var delIcon = EditorGUIUtility.IconContent("d_TreeEditor.Trash");
                    delBtn.Add(new Image { image = delIcon.image as Texture2D, scaleMode = ScaleMode.ScaleToFit, style = { flexGrow = 1 } });
                    delBtn.tooltip = "Delete";
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

            // TrackPropertyValue on the array fires only when the array size changes,
            // not on every value edit inside the elements (avoids flickering).
            root.TrackPropertyValue(entriesProp, _ => RebuildList());

            // ── footer: closing brace ─────────────────────────────────────
            var footer = new Label("}");
            footer.style.paddingLeft = 4;
            footer.style.marginTop   = 2;
            root.Add(footer);

            return root;
        }

        private static void ResetChild(SerializedProperty serializedProperty)
        {
            foreach (SerializedProperty child in serializedProperty)
            {
                if (child.propertyType == SerializedPropertyType.ManagedReference)
                    child.managedReferenceValue = null;
                else if (child.propertyType == SerializedPropertyType.ArraySize)
                    child.arraySize = 0;
            }
        }
    }
}