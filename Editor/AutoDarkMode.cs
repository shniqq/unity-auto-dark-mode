using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Packages.AutoDarkMode
{
    public static class AutoDarkMode
    {
        public const int ProSkinIndex = 1;
        public const int DefaultSkinIndex = 0;

        [InitializeOnLoadMethod]
        public static void OnInit()
        {
            PerformAutoDarkMode();

            EditorApplication.projectChanged += PerformAutoDarkMode;
        }

        private static void PerformAutoDarkMode()
        {
            ShowWelcomeMessage();

            var settings = AutoDarkModeSettings.Instance;
            if (!settings.EnableAutoDarkMode)
            {
                if (settings.ShowExtraLogs)
                {
                    Debug.Log(
                        $"Skipping Auto Dark Mode check since {nameof(settings.EnableAutoDarkMode)} is set to false!");
                }

                return;
            }

            if (settings.AutoFetch)
            {
                var timeSinceLastFetch = DateTime.UtcNow - settings.LastAutoFetchTime;
                if (timeSinceLastFetch > Constants.AutoFetchInterval)
                {
                    AutoDarkModeSettings.FetchAll(
                        () =>
                        {
                            settings.LastAutoFetchTime = DateTime.UtcNow;
                            EditorUtility.SetDirty(AutoDarkModeSettings.Instance);
                            SetEditorTheme();
                        },
                        () =>
                        {
                            if (settings.ShowExtraLogs)
                            {
                                Debug.Log("Failed to auto-fetch.");
                            }
                        }
                    );
                }
                else if (settings.ShowExtraLogs)
                {
                    Debug.Log(
                        $"Skipping auto fetching sunrise/sunset data since time of last fetch is only {timeSinceLastFetch.ToString()} old.");
                }
            }

            SetEditorTheme();
        }

        public static void SetEditorTheme()
        {
            var settings = AutoDarkModeSettings.Instance;
            var timeNowUtc = DateTime.UtcNow.TimeOfDay;
            var sunset = settings.Sunset;
            var sunrise = settings.Sunrise;
            if (timeNowUtc > sunrise && timeNowUtc < sunset)
            {
                if (settings.ShowExtraLogs)
                {
                    Debug.Log("It's day, bring out the light UI!");
                }

                if (GetCurrentSkinIndex() == ProSkinIndex)
                {
                    ToggleSkin();
                }
            }
            else
            {
                if (settings.ShowExtraLogs)
                {
                    Debug.Log("It's night, bring out the dark UI!");
                }

                if (GetCurrentSkinIndex() == DefaultSkinIndex)
                {
                    ToggleSkin();
                }
            }
        }

        private static void ShowWelcomeMessage()
        {
            if (!AutoDarkModeSettings.Instance.HasSeenWelcomeMessage)
            {
                AutoDarkModeSettings.Instance.HasSeenWelcomeMessage = true;
                EditorUtility.SetDirty(AutoDarkModeSettings.Instance);
                if (EditorUtility.DisplayDialog("Auto Dark Mode",
                    $"Thank you for using Auto Dark Mode!{Environment.NewLine}Head over to the settings window and configure Auto Dark Mode there.",
                    "Let's go!"))
                {
                    AutoDarkModeSettingsRegister.Open();
                }
            }
        }

        private static int GetCurrentSkinIndex()
        {
            const string skinIndexPropertyName = "skinIndex";
            var propertyInfo =
                typeof(EditorGUIUtility).GetProperty(skinIndexPropertyName,
                    BindingFlags.Static | BindingFlags.NonPublic);
            if (propertyInfo is null)
            {
                Debug.LogError(
                    $"Couldn't find property '{skinIndexPropertyName}' in {nameof(EditorGUIUtility)}.");
                return 0;
            }

            return (int) propertyInfo.GetMethod.Invoke(null, new object[] { });
        }

        private static void ToggleSkin()
        {
            InternalEditorUtility.SwitchSkinAndRepaintAllViews();
            SettingsService.NotifySettingsProviderChanged();
            EditorApplication.RepaintProjectWindow();
            if (AutoDarkModeSettings.Instance.ShowExtraLogs)
            {
                Debug.Log("Toggled Editor Theme.");
            }
        }
    }
}