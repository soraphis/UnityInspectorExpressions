using UnityEditor;
using UnityEngine;

namespace Editor.Helpers
{
    public static class CustomStyles
    {

        private static GUIStyle s_centerdLabel = null;
        public static GUIStyle centerdLabel
        {
            get
            {
                if (s_centerdLabel != null) return s_centerdLabel;
                s_centerdLabel = new GUIStyle(EditorStyles.label);
                s_centerdLabel.alignment = TextAnchor.MiddleCenter;
                return s_centerdLabel;
            }
        }
    }
}
