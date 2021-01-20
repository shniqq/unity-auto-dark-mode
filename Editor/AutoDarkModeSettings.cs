using System;
using System.Globalization;
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
        public Vector2 Geolocation;
        [SerializeField, HideInInspector]
        public string Location;

        [SerializeField, HideInInspector]
        public bool AutoFetch;
        [SerializeField, HideInInspector]
        public int FetchTimeout = 10;
        [SerializeField, HideInInspector]
        private string _lastAutoFetchTime;
        public DateTime LastAutoFetchTime
        {
            get => DateTime.TryParse(_lastAutoFetchTime, out var value) ? value : default;
            set => _lastAutoFetchTime = value.ToString(CultureInfo.InvariantCulture);
        }

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
                new GUIContent("Enable Auto Dark Mode"));
            EditorGUILayout.Space();

            DrawAutoFetchUI(settings);

            EditorGUILayout.PropertyField(settings.FindProperty(nameof(_sunrise)), new GUIContent("Sunrise"));
            EditorGUILayout.PropertyField(settings.FindProperty(nameof(_sunset)), new GUIContent("Sunset"));

            EditorGUILayout.PropertyField(settings.FindProperty(nameof(ShowExtraLogs)),
                new GUIContent("Show Extra Logs"));

            if (settings.hasModifiedProperties)
            {
                settings.ApplyModifiedProperties();
            }
        }

        private static void DrawAutoFetchUI(SerializedObject settings)
        {
            EditorGUI.indentLevel++;
            GUILayout.BeginHorizontal();
            var refreshIcon = EditorGUIUtility.IconContent("d_Refresh");
            var fetchButtonContent = new GUIContent("Fetch now", refreshIcon.image,
                "Fetches first the long/lat based on your IP, and then the sunrise and sunset based on the long/lat.");
            if (GUILayout.Button(fetchButtonContent))
            {
                EditorUtility.DisplayProgressBar("Auto Dark Mode", "Fetching Location & Sunrise/Sunset from API..",
                    0.25f);
                Action onError = EditorUtility.ClearProgressBar;
                FetchAll(EditorUtility.ClearProgressBar, onError);
                AutoDarkMode.SetEditorTheme();
            }

            EditorGUILayout.PropertyField(settings.FindProperty(nameof(FetchTimeout)),
                new GUIContent("Fetch Timeout"));
            GUILayout.EndHorizontal();
            
            GUILayout.Box(
                $"Uses {IpApi.ApiUrl} to fetch your approximate longitude and latitude based on your IP and {SunriseSunsetApi.ApiUrl} to fetch sunrise and sunset based on longitude and latitude.");
            EditorGUILayout.Space();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                new GUIContent($"Rough Location: {settings.FindProperty(nameof(Location)).stringValue}"));
            var geoLocation = settings.FindProperty(nameof(Geolocation)).vector2Value;
            EditorGUILayout.LabelField(new GUIContent($"Longitude: {geoLocation.x} | Latitude: {geoLocation.y}"));
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            var autoFetch = settings.FindProperty(nameof(AutoFetch));
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Auto fetch once a day?", GUILayout.ExpandWidth(true));
            EditorGUILayout.PropertyField(autoFetch, GUIContent.none);
            GUILayout.EndHorizontal();
            
            EditorGUI.indentLevel--;
        }

        private static void FetchSunriseSunsetData(Action onComplete, Action onError)
        {
            SunriseSunsetApi.FetchData(
                Instance.Geolocation.x,
                Instance.Geolocation.y,
                Instance.FetchTimeout,
                (sunrise, sunset) =>
                {
                    Instance._sunrise = sunrise.ToString();
                    Instance._sunset = sunset.ToString();
                    EditorUtility.SetDirty(Instance);
                    onComplete?.Invoke();
                },
                onError);
        }

        private static void FetchLongLatData(Action onComplete, Action onError)
        {
            IpApi.FetchData(Instance.FetchTimeout, (longitude, latitude, location) =>
            {
                Instance.Geolocation = new Vector2(longitude, latitude);
                Instance.Location = location;
                EditorUtility.SetDirty(Instance);
                onComplete?.Invoke();
            }, onError);
        }

        public static void FetchAll(Action onComplete, Action onError)
        {
            FetchLongLatData(() => FetchSunriseSunsetData(onComplete, onError), onError);
        }
    }
}