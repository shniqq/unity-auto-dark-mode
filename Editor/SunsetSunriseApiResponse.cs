using System;

namespace Packages.AutoDarkMode
{
    /// <summary>
    /// Response for https://sunrise-sunset.org/api
    /// </summary>
    [Serializable]
    internal class SunsetSunriseApiResponse
    {
        [Serializable]
        public class Results
        {
            public string sunrise;
            public string sunset;
            public string solar_noon;
            public string day_length;
            public string civil_twilight_begin;
            public string civil_twilight_end;
            public string nautical_twilight_begin;
            public string nautical_twilight_end;
            public string astronomical_twilight_begin;
            public string astronomical_twilight_end;
        }
            
        public Results results;
        public string status;

        public bool IsValid()
        {
            return status == "OK" && results != null;
        }
    }
}