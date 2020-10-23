using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FIWAREHub.Parsers.Models;
using FIWAREHub.Web.Models;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIWAREHub.Web.Controllers.Api
{
    public class IoTMeasurementController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> PostTestWeatherMeasurement()
        {
            var weatherEvent = new POSTWeatherReport
            {
                Temperature = 12,
                WindChill = 0,
                Humidity = 02,
                Pressure = (decimal)33.33,
                Visibility = 100,
                WindDirection = "NWD",
                WindSpeed = (decimal)120.1,
                Precipitation = (decimal)30.5,
                ReportTime = DateTime.UtcNow
            };

            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendJson(HttpMethod.Post, 
                FIWAREUrls.JsonMeasurementUrl(FIWAREUrls.WeatherDeviceIds.FirstOrDefault()), weatherEvent);
            var success = response.IsSuccessStatusCode;

            return Ok(success);
        }

        [HttpGet]
        public async Task<IActionResult> PostTestRoadTrafficReport()
        {
            var roadTraffic = new POSTTrafficReport
            {
                StartTime = DateTime.UtcNow,
                City = "Random",
                AddressNumber = "AddrNr",
                StartLatitude = "37.9600965",
                StartLongitude = "23.8576042"
            };

            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendUltraLight(HttpMethod.Post, 
                FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.FirstOrDefault()), roadTraffic.ToUltraLightSyntax());
            var success = response.IsSuccessStatusCode;

            return Ok(success);
        }
    }
}
