using System;
using UnityEditor;
using UnityEngine;

namespace Packages.AutoDarkMode
{
    internal class AutoDarkModeSettings : ScriptableObject
    {
        private const string SettingsFilePath = "Assets/AutoDarkModeSettings.asset";

        #region Singleton & Creation

        public static AutoDarkModeSettings Instance => GetOrCreateSettings();

        private static AutoDarkModeSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AutoDarkModeSettings>(SettingsFilePath);
            if (settings == null)
            {
                settings = CreateInstance<AutoDarkModeSettings>();
                AssetDatabase.CreateAsset(settings, SettingsFilePath);
                AssetDatabase.SaveAssets();

                return settings;
            }

            return settings;
        }

        private static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        #endregion

        [SerializeField, Header("Use the Settings Window to edit the settings for Auto Dark Mode.")]
        public bool HasSeenWelcomeMessage;

        [SerializeField, HideInInspector]
        public bool EnableAutoDarkMode;

        [SerializeField, HideInInspector]
        public bool UseCoordinates;

        [SerializeField, HideInInspector]
        public Vector2 Geolocation;

        [SerializeField, HideInInspector]
        public bool AutoFetchSunriseSunsetTimes;
        [SerializeField, HideInInspector]
        public int FetchTimeout;

        public TimeSpan Sunrise => Convert.ToDateTime(_sunrise).TimeOfDay;
        [SerializeField, HideInInspector]
        private string _sunrise;

        public TimeSpan Sunset => Convert.ToDateTime(_sunset).TimeOfDay;
        [SerializeField, HideInInspector]
        private string _sunset;

        [SerializeField, HideInInspector]
        public bool ShowExtraLogs;

        public static void DrawSettings()
        {
            var settings = GetSerializedSettings();
            EditorGUILayout.PropertyField(settings.FindProperty(nameof(EnableAutoDarkMode)),
                new GUIContent("Enable Dark Mode"));

            var useCoordinates = settings.FindProperty(nameof(UseCoordinates));
            EditorGUILayout.LabelField(
                "Automatically fetch Sunrise & Sunset time from a server based on longitude and latitude?");
            EditorGUILayout.PropertyField(useCoordinates, new GUIContent("Auto Fetch"));

            if (useCoordinates.boolValue)
            {
                EditorGUILayout.PropertyField(settings.FindProperty(nameof(Geolocation)),
                    new GUIContent("Longitude/Latitude"));
                EditorGUILayout.PropertyField(settings.FindProperty(nameof(FetchTimeout)),
                    new GUIContent("Fetch Timeout"));

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Fetch Sunrise/Sunset times from API"))
                {
                    EditorUtility.DisplayProgressBar("Auto Dark Mode", "Fetching Sunrise/Sunset from API..", 0f);
                    SunriseSunsetApi.FetchData(
                        Instance.Geolocation.x,
                        Instance.Geolocation.y,
                        Instance.FetchTimeout,
                        (sunrise, sunset) =>
                        {
                            EditorUtility.ClearProgressBar();
                            Instance._sunrise = sunrise.ToString();
                            Instance._sunset = sunset.ToString();
                            EditorUtility.SetDirty(Instance);
                        },
                        EditorUtility.ClearProgressBar);
                }

                GUILayout.Label("Uses https://sunrise-sunset.org - thanks!");
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.PropertyField(settings.FindProperty(nameof(_sunrise)), new GUIContent("Sunrise"));
            EditorGUILayout.PropertyField(settings.FindProperty(nameof(_sunset)), new GUIContent("Sunset"));

            EditorGUILayout.PropertyField(settings.FindProperty(nameof(ShowExtraLogs)),
                new GUIContent("Show Extra Logs"));

            if (settings.hasModifiedProperties)
            {
                settings.ApplyModifiedProperties();
            }
        }
    }
}