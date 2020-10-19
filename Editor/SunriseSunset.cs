using System;

namespace Packages.AutoDarkMode
{
    public class SunriseSunset
    {
        public DateTime Sunrise { get; }
        public DateTime Sunset { get; }

        public SunriseSunset(DateTime sunrise, DateTime sunset)
        {
            Sunset = sunset;
            Sunrise = sunrise;
        }
    }
}