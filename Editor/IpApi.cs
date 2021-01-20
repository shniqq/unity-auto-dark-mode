using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Packages.AutoDarkMode
{
    internal static class IpApi
    {
        public const string ApiUrl = "http://ip-api.com/json/";

        public static void FetchData(int timeout, Action<float, float> onFetchedLongLat, Action onError)
        {
            var request = UnityWebRequest.Get(ApiUrl);
            request.timeout = timeout;
            var requestOperation = request.SendWebRequest();
            requestOperation.completed += _ => OnResponseReceived(onFetchedLongLat, onError, request);
        }

        private static void OnResponseReceived(
            Action<float, float> onFetchedLongLat,
            Action onError,
            UnityWebRequest request)
        {
            try
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Sending request failed with error:\n{request.error}");
                    onError?.Invoke();
                    return;
                }

                var resultText = request.downloadHandler.text;
                var apiResponse = JsonUtility.FromJson<IpApiResponse>(resultText);

                if (!apiResponse.IsValid())
                {
                    Debug.LogError($"Invalid response from API!\n{resultText}");
                    onError?.Invoke();
                    return;
                }

                var prettyResultText = JsonUtility.ToJson(apiResponse, true);

                if (AutoDarkModeSettings.Instance.ShowExtraLogs)
                {
                    Debug.Log($"Server Response:\n{prettyResultText}");
                    Debug.Log($"Success. Found Longitude {apiResponse.lon} and Latitude {apiResponse.lat}.");
                }

                onFetchedLongLat?.Invoke((float) apiResponse.lon, (float) apiResponse.lat);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                onError?.Invoke();
            }
        }
    }
}