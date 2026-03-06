using Editor.Helpers;
using UnityInspectorExpressions.Expressions.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityInspectorExpressions.Expressions
{
    [CustomPropertyDrawer(typeof(MatchFirstFloatExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstIntExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstStringExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstComponentExpression))]
    [CustomPropertyDrawer(typeof(MatchFirstGameObjectExpression))]
    public class MatchFirstFloatExpressionDrawer : PropertyDrawer
    {
        const string s_DefaultExprPropertyName = "m_DefaultExpr";
        const string s_EntriesPropertyName      = "m_Entries";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var entriesProp = property.FindPropertyRelative(s_EntriesPropertyName);
            var defaultProp = property.FindPropertyRelative(s_DefaultExprPropertyName);

            // ── outer column ──────────────────────────────────────────────
            var root = new VisualElement();
            root.style.flexGrow = 1;

            // ── header row:  "match first {"    [+] ──────────────────────
            var header = CustomStyles.MakeRow();
            header.style.marginBottom = 2;
            header.style.flexGrow     = 0;

            var lblHeader = new Label("match first {");
            lblHeader.style.flexGrow   = 1;
            lblHeader.style.flexShrink = 0;

            var addBtn = CustomStyles.MakeIconButton("d_Toolbar Plus");
            addBtn.tooltip  = "Add case";
            addBtn.clicked += () =>
            {
                entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);
                CustomStyles.ResetSerializedChildren(entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1));
                entriesProp.serializedObject.ApplyModifiedProperties();
            };

            header.Add(lblHeader);
            header.Add(addBtn);
            root.Add(header);

            // ── body: indented list of match-entries ──────────────────────
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

            // ── footer row:  "  default ⇒ [expr] }" ──────────────────────
            var footer = CustomStyles.MakeRow();
            footer.style.marginTop   = 2;
            footer.style.paddingLeft = 10;
            footer.style.flexGrow    = 0;

            var lblDefault = new Label("default");
            lblDefault.style.flexShrink     = 0;
            lblDefault.style.minWidth       = 55;
            lblDefault.style.unityTextAlign = TextAnchor.MiddleCenter;

            var lblArrow = new Label("\u21D2");
            lblArrow.style.flexShrink     = 0;
            lblArrow.style.paddingLeft    = 4;
            lblArrow.style.paddingRight   = 4;
            lblArrow.style.unityTextAlign = TextAnchor.MiddleCenter;

            var defaultField = new PropertyField(defaultProp, "");
            defaultField.style.flexGrow   = 1;
            defaultField.style.flexShrink = 1;

            var lblClose = new Label("}");
            lblClose.style.flexShrink  = 0;
            lblClose.style.paddingLeft = 4;

            footer.Add(lblDefault);
            footer.Add(lblArrow);
            footer.Add(defaultField);
            footer.Add(lblClose);
            root.Add(footer);

            return root;
        }
    }

    // ///////////////////////////////////////////////////////////////////
    //                  MATCH-ENTRY DRAWER
    // ///////////////////////////////////////////////////////////////////

    [CustomPropertyDrawer(typeof(MatchFirstFloatExpression.MatchEntry))]
    [CustomPropertyDrawer(typeof(MatchFirstIntExpression.MatchEntry))]
    [CustomPropertyDrawer(typeof(MatchFirstComponentExpression.MatchEntry))]
    [CustomPropertyDrawer(typeof(MatchFirstStringExpression.MatchEntry))]
    public class MatchFirstFloatExpression_MatchEntryDrawer : PropertyDrawer
    {
        const string s_ConditionPropertyName = "m_Condition";
        const string s_ResultPropertyName    = "m_Result";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var condProp = property.FindPropertyRelative(s_ConditionPropertyName);
            var resuProp = property.FindPropertyRelative(s_ResultPropertyName);

            var row = CustomStyles.MakeRow();

            var condField = new PropertyField(condProp, "");
            condField.style.flexGrow   = 1;
            condField.style.flexShrink = 1;

            var lblArrow = new Label("\u21D2");
            lblArrow.style.flexShrink     = 0;
            lblArrow.style.paddingLeft    = 4;
            lblArrow.style.paddingRight   = 4;
            lblArrow.style.unityTextAlign = TextAnchor.MiddleCenter;

            var resultField = new PropertyField(resuProp, "");
            resultField.style.flexGrow   = 1;
            resultField.style.flexShrink = 1;

            row.Add(condField);
            row.Add(lblArrow);
            row.Add(resultField);
            return row;
        }
    }
}
