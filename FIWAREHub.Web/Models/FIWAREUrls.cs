using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIWAREHub.Web.Models
{
    public static class FIWAREUrls
    {
        private static string baseUrl => "http://192.168.10.254";

        public static string DeviceProvisionPath => "/iot/devices";

        public static string ServiceGroupPath => "/iot/services";

        public static string OrionUrl => $"{baseUrl}:1026";

        public static string IoTAJsonNorth => $"{baseUrl}:4041";

        public static string IoTAJsonSouth => $"{baseUrl}:7896";

        public static string JsonKey => $"JSONKEY.EPU.NTUA";

        public static string JsonServiceGroupResource => $"/iot/json";

        public static string JsonMeasurementUrl(string deviceId) => $"{IoTAJsonSouth}{JsonServiceGroupResource}?k={JsonKey}&i={deviceId}";

        public static string IoTUltraLightNorth => $"{baseUrl}:4061";

        public static string IoTUltraLightSouth => $"{baseUrl}:7897";

        public static string UltraLightKey => $"ULKEY.EPU.NTUA";

        public static string UltraLightServiceGroupResource => $"/iot/d";

        public static string UltraLightMeasurementUrl(string deviceId) => $"{IoTUltraLightSouth}{UltraLightServiceGroupResource}?k={UltraLightKey}&i={deviceId}";

        public static List<string> WeatherDeviceIds = new List<string>
        {
            "Weather01",
            "Weather02",
            "Weather03",
            "Weather04",
            "Weather05",
            "Weather06",
            "Weather07",
            "Weather08",
            "Weather09",
            "Weather10",
            "Weather11",
            "Weather12",
            "Weather13",
            "Weather14",
            "Weather15",
            "Weather16",
            "Weather17",
            "Weather18",
            "Weather19",
            "Weather20"
        };

        public static List<string> RoadTrafficDeviceIds = new List<string>
        {
            "RoadTraffic01",
            "RoadTraffic02",
            "RoadTraffic03",
            "RoadTraffic04",
            "RoadTraffic05",
            "RoadTraffic06",
            "RoadTraffic07",
            "RoadTraffic08",
            "RoadTraffic09",
            "RoadTraffic10",
            "RoadTraffic11",
            "RoadTraffic12",
            "RoadTraffic13",
            "RoadTraffic14",
            "RoadTraffic15",
            "RoadTraffic16",
            "RoadTraffic17",
            "RoadTraffic18",
            "RoadTraffic19",
            "RoadTraffic20"
        };
    }
}
