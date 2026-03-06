using UnityEngine;

namespace Editor.Helpers
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color self, float alpha)
        {
            return new Color(self.r, self.g, self.b, alpha);
        }
    }
}