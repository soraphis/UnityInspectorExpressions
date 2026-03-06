using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Helpers
{
    /// <summary>
    /// Shared factory helpers used by the expression PropertyDrawers.
    /// </summary>
    public static class CustomStyles
    {
        // ── Labels ────────────────────────────────────────────────────────

        /// <summary>Creates a non-growing label styled as a syntax token (e.g. "(", ")", ",").</summary>
        public static Label MakeLabel(string text)
        {
            var lbl = new Label(text);
            lbl.style.flexShrink        = 0;
            lbl.style.paddingLeft       = 2;
            lbl.style.paddingRight      = 2;
            lbl.style.height            = 20;
            lbl.style.fontSize          = 20;
            lbl.style.unityTextAlign    = TextAnchor.MiddleCenter;
            return lbl;
        }

        // ── Rows ─────────────────────────────────────────────────────────

        /// <summary>Creates a horizontal flex row with centered items that grows to fill available space.</summary>
        public static VisualElement MakeRow()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems    = Align.Center;
            row.style.flexGrow      = 1;
            return row;
        }

        /// <summary>
        /// Creates a horizontal flex row that highlights itself (cyan tint) on mouse-over and
        /// clears the highlight when the pointer leaves.
        /// </summary>
        public static VisualElement MakeHighlightableRow()
        {
            var row = MakeRow();
            row.RegisterCallback<MouseOverEvent>(evt =>
            {
                ExpressionHighlightState.SetSelected(row);
                evt.StopPropagation();
            });
            row.RegisterCallback<MouseLeaveEvent>(_ => ExpressionHighlightState.SetSelected(null));
            return row;
        }

        // ── Icon Buttons ──────────────────────────────────────────────────

        /// <summary>
        /// Creates a square, padding-free icon button using the given Unity built-in icon name.
        /// <paramref name="size"/> controls both width and height (default 20).
        /// </summary>
        public static Button MakeIconButton(string iconName, float size = 20f)
        {
            var btn = new Button();
            btn.style.flexShrink    = 0;
            btn.style.width         = size;
            btn.style.height        = size;
            btn.style.paddingLeft   = 0;
            btn.style.paddingRight  = 0;
            btn.style.paddingTop    = 0;
            btn.style.paddingBottom = 0;
            btn.Add(new Image
            {
                image     = EditorGUIUtility.IconContent(iconName).image as Texture2D,
                scaleMode = ScaleMode.ScaleToFit,
                style     = { flexGrow = 1 }
            });
            return btn;
        }

        // ── SerializedProperty helpers ────────────────────────────────────

        /// <summary>
        /// Resets direct children of <paramref name="property"/>:
        /// managed references are set to null, arrays are cleared.
        /// </summary>
        public static void ResetSerializedChildren(SerializedProperty property)
        {
            foreach (SerializedProperty child in property)
            {
                if (child.propertyType == SerializedPropertyType.ManagedReference)
                    child.managedReferenceValue = null;
                else if (child.propertyType == SerializedPropertyType.ArraySize)
                    child.arraySize = 0;
            }
        }
    }
}
