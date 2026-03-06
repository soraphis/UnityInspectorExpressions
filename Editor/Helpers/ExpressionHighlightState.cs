using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Helpers
{
    /// <summary>
    /// Tracks the single currently-highlighted expression row in the Inspector.
    /// Call <see cref="SetSelected"/> with the row element to highlight it,
    /// or with <c>null</c> to clear the highlight.
    /// </summary>
    public static class ExpressionHighlightState
    {
        private static VisualElement s_Selected;

        public static void SetSelected(VisualElement element)
        {
            if (s_Selected != null)
            {
                s_Selected.style.color           = StyleKeyword.Null;
                s_Selected.style.backgroundColor = StyleKeyword.Null;
            }

            s_Selected = element;

            if (s_Selected != null)
            {
                s_Selected.style.color           = Color.cyan;
                s_Selected.style.backgroundColor = Color.cyan.WithAlpha(0.2f);
            }
        }
    }
}

