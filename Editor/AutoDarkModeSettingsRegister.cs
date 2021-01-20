using System.Collections.Generic;
using UnityEditor;

namespace Packages.AutoDarkMode
{
    internal static class AutoDarkModeSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateAutoDarkModeSettingsProvider()
        {
            var provider = new SettingsProvider($"Project/{Constants.Name}", SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    AutoDarkModeSettings.DrawSettings();
                },
                keywords = new HashSet<string>(new[] {"Skin", "Auto Dark Mode", "Professional", "Theme", "Scheme"})
            };

            return provider;
        }
    }
}