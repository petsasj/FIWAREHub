using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FIWAREHub.Models.ParserModels;
using FIWAREHub.Parsers;
using FIWAREHub.Web.Extensions;
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
            var weatherEvent = new FiwareWeatherReport
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
            var roadTraffic = new FiwareTrafficReport
            {
                StartTime = DateTime.UtcNow,
                City = "Random",
                AddressNumber = "AddrNr",
                StartLatitude = "37.9600965",
                StartLongitude = "23.8576043"
            };

            var testStr = "sv|3|sdt|01-Jan-17 03:29:36|geo|37.318039, -121.940125|dis|0.01|desc|1 lane blocked due to accident on I-880 Northbound at CA-17.|addrnr|5198.0|str|I-880 N|sd|R|ct|San Jose|cn|Santa Clara|st|CA|zc|95128|c|US";
            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendUltraLight(HttpMethod.Post, 
                FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.FirstOrDefault()), testStr);
            var success = response.IsSuccessStatusCode;

            return Ok(success);
        }

        [HttpGet]
        public async Task<IActionResult> SubmitCsvReadings()
        {
            var fileParser = new FileParser();
            var accidentDataset = fileParser.ParseAccidentsDataset().ToList();

            var chunks = accidentDataset.Chunk(100);

            foreach (var chunk in chunks)
            {
                using var weatherClient = new FIWAREClient();
                using var trafficClient = new FIWAREClient();
                foreach (var report in chunk)
                {
                    var weatherResponseMessage = await weatherClient.SendJson(HttpMethod.Post, 
                        FIWAREUrls.JsonMeasurementUrl(FIWAREUrls.WeatherDeviceIds.FirstOrDefault()), report.FiwareWeatherReport);
                    System.Threading.Thread.Sleep(80);
                    var trafficResponseMessage = await trafficClient.SendUltraLight(HttpMethod.Post, 
                        FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.FirstOrDefault()), report.FiwareTrafficDataReport.ToUltraLightSyntax());
                    if (!trafficResponseMessage.IsSuccessStatusCode)
                    {
                        var messageSent = report.FiwareTrafficDataReport.ToUltraLightSyntax();
                    }
                }
            }

            return Ok();
        }
    }
}
