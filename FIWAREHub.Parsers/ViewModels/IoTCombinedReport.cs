using System;
using System.Collections.Generic;
using System.Text;
using FIWAREHub.Parsers.Models;

namespace FIWAREHub.Parsers.ViewModels
{
    public class IoTCombinedReport
    {
        public POSTTrafficReport TrafficData { get; set; }

        public POSTWeatherReport WeatherReport { get; set; }
    }
}
