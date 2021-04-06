using System.Collections.Generic;
using UnityEditor;

namespace Packages.AutoDarkMode
{
    internal static class AutoDarkModeSettingsRegister
    {
        private static readonly string SettingsPath = $"Project/{Constants.Name}";
        
        [SettingsProvider]
        public static SettingsProvider CreateAutoDarkModeSettingsProvider()
        {
            var provider = new SettingsProvider(SettingsPath, SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    AutoDarkModeSettings.DrawSettings();
                },
                keywords = new HashSet<string>(new[] {"Skin", "Auto Dark Mode", "Professional", "Theme", "Scheme"})
            };

            return provider;
        }

        public static void Open()
        {
            SettingsService.OpenUserPreferences(SettingsPath);
        }
    }
}