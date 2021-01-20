using UnityEditor;
using UnityEngine;

namespace Packages.AutoDarkMode
{
    public class AutoDarkModeMenuItems
    {
        private const string MenuPrefix = "Window/Auto Dark Mode/";

        [MenuItem(MenuPrefix + "Toggle Auto Dark Mode")]
        public static void Toggle()
        {
            var autoDarkModeSettings = AutoDarkModeSettings.Instance;
            autoDarkModeSettings.EnableAutoDarkMode = !autoDarkModeSettings.EnableAutoDarkMode;
            EditorUtility.SetDirty(autoDarkModeSettings);
            Debug.Log($"Auto Dark Mode has been {(autoDarkModeSettings.EnableAutoDarkMode ? "enabled" : "disabled")}.");
        }
    }
}