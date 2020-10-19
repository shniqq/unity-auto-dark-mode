using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Packages.AutoDarkMode
{
    public static class SunriseSunsetApi
    {
        private const string ApiUrl = "https://api.sunrise-sunset.org/json?lat={1}&lng={0}";

        public static void FetchData(float longitude, float latitude, int timeout,
            Action<TimeSpan, TimeSpan> onReceivedSunriseSunset,
            Action onError)
        {
            var uri = string.Format(ApiUrl, longitude, latitude);
            var request = UnityWebRequest.Get(uri);
            request.timeout = timeout;
            var requestOperation = request.SendWebRequest();
            requestOperation.completed += _ =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Sending request failed with error:\n{request.error}");
                    onError.Invoke();
                    return;
                }

                var resultText = request.downloadHandler.text;
                var apiResponse = JsonUtility.FromJson<SunsetSunriseApiResponse>(resultText);

                if (!apiResponse.IsValid())
                {
                    Debug.LogError($"Unable to parse result!\n{resultText}");
                    onError.Invoke();
                    return;
                }

                var sunrise = Convert.ToDateTime(apiResponse.results.sunrise).TimeOfDay;
                var sunset = Convert.ToDateTime(apiResponse.results.sunset).TimeOfDay;

                var prettyResultText = JsonUtility.ToJson(apiResponse, true);

                if (AutoDarkModeSettings.Instance.ShowExtraLogs)
                {
                    Debug.Log($"Server Response:\n{prettyResultText}");
                    Debug.Log($"Success. Found Sunrise {sunrise} and Sunset {sunset}.");
                }

                onReceivedSunriseSunset.Invoke(sunrise, sunset);
            };
        }
    }
}