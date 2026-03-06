using System;
using UnityEngine.UIElements;

namespace Editor.Helpers
{
    public static class VisualElementExtensions
    {
        public static VisualElement WithFontSize(this VisualElement self, StyleLength fontSize)
        {
            self.style.fontSize = fontSize;
            return self;
        }
        
        public static VisualElement WithFlex(this VisualElement self, float flexGrow)
        {
            self.style.flexGrow = flexGrow;
            return self;
        }
        
        public static VisualElement WithFlex(this VisualElement self, float flexGrow, float flexShrink)
        {
            self.style.flexGrow = flexGrow;
            self.style.flexShrink = flexShrink;
            return self;
        }
    }

    public static class StringExtensions
    {
        // returns the substring that is between the first found "from" and last found "to" char. (exclusive bounds)
        public static ReadOnlySpan<char> SubstringBetween(this string self, char from, char to)
        {
            var i = self.IndexOf(from);
            var j = self.LastIndexOf(to);
            return self.AsSpan(i + 1, (j) - (i + 1));
        }
    }
}