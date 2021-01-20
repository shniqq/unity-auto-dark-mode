using System;

namespace Packages.AutoDarkMode
{
    [Serializable]
    internal class IpApiResponse
    {
        public string status;
        public string continent;
        public string continentCode;
        public string country;
        public string countryCode;
        public string region;
        public string regionName;
        public string city;
        public string district;
        public string zip;
        public double lat;
        public double lon;
        public string timezone;
        public int offset;
        public string currency;
        public string isp;
        public string org;
        public string @as;
        public string asname;
        public bool mobile;
        public bool proxy;
        public bool hosting;
        public string query;

        public bool IsValid()
        {
            return status == "success" && !double.IsNaN(lat) && !double.IsNaN(lon);
        }
    }
}