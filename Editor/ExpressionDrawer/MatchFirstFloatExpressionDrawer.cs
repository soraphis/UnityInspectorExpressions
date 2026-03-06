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
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems    = Align.Center;
            header.style.marginBottom  = 2;

            var lblHeader = new Label("match first {");
            lblHeader.style.flexGrow   = 1;
            lblHeader.style.flexShrink = 0;

            var addBtn = new Button(() =>
            {
                entriesProp.InsertArrayElementAtIndex(entriesProp.arraySize);
                ResetChild(entriesProp.GetArrayElementAtIndex(entriesProp.arraySize - 1));
                entriesProp.serializedObject.ApplyModifiedProperties();
            });
            addBtn.style.flexShrink    = 0;
            addBtn.style.width         = 20;
            addBtn.style.height        = 20;
            addBtn.style.paddingLeft   = 0;
            addBtn.style.paddingRight  = 0;
            addBtn.style.paddingTop    = 0;
            addBtn.style.paddingBottom = 0;
            addBtn.Add(new Image
            {
                image     = EditorGUIUtility.IconContent("d_Toolbar Plus").image as Texture2D,
                scaleMode = ScaleMode.ScaleToFit,
                style     = { flexGrow = 1 }
            });
            addBtn.tooltip = "Add case";

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

                    var row = new VisualElement();
                    row.style.flexDirection = FlexDirection.Row;
                    row.style.alignItems    = Align.Center;
                    row.style.marginBottom  = 2;

                    var delBtn = new Button();
                    delBtn.style.flexShrink    = 0;
                    delBtn.style.width         = 16;
                    delBtn.style.height        = 16;
                    delBtn.style.paddingLeft   = 0;
                    delBtn.style.paddingRight  = 0;
                    delBtn.style.paddingTop    = 0;
                    delBtn.style.paddingBottom = 0;
                    delBtn.Add(new Image
                    {
                        image     = EditorGUIUtility.IconContent("d_TreeEditor.Trash").image as Texture2D,
                        scaleMode = ScaleMode.ScaleToFit,
                        style     = { flexGrow = 1 }
                    });
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
            root.TrackPropertyValue(entriesProp, _ => RebuildList());

            // ── footer row:  "  default ⇒ [expr] }" ──────────────────────
            var footer = new VisualElement();
            footer.style.flexDirection = FlexDirection.Row;
            footer.style.alignItems    = Align.Center;
            footer.style.marginTop     = 2;
            footer.style.paddingLeft   = 10;

            var lblDefault = new Label("default");
            lblDefault.style.flexShrink          = 0;
            lblDefault.style.minWidth            = 55;
            lblDefault.style.unityTextAlign      = TextAnchor.MiddleCenter;

            var lblArrow = new Label("\u21D2");
            lblArrow.style.flexShrink      = 0;
            lblArrow.style.paddingLeft     = 4;
            lblArrow.style.paddingRight    = 4;
            lblArrow.style.unityTextAlign  = TextAnchor.MiddleCenter;

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

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems    = Align.Center;
            row.style.flexGrow      = 1;

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
