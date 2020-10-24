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

        [HttpPost]
        public async Task<IActionResult> PostTestWeatherMeasurement(FiwareWeatherReport model)
        {
            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendJson(HttpMethod.Post, 
                FIWAREUrls.JsonMeasurementUrl(FIWAREUrls.WeatherDeviceIds.FirstOrDefault()), model);
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

            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendUltraLight(HttpMethod.Post, 
                FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.FirstOrDefault()), roadTraffic.ToUltraLightSyntax());
            var success = response.IsSuccessStatusCode;

            return Ok(success);
        }

        [HttpPost]
        public async Task<IActionResult> PostTestRoadTrafficReport([FromBody] string payLoad)
        {
            using var fiwareClient = new FIWAREClient();
            var response = await fiwareClient.SendUltraLight(HttpMethod.Post, 
                FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.FirstOrDefault()), payLoad);
            var success = response.IsSuccessStatusCode;

            return Ok(success);
        }

        [HttpGet]
        public async Task<IActionResult> SubmitCsvReadings()
        {
            var fileParser = new FileParser();
            var accidentDataset = fileParser.ParseAccidentsDataset().ToList();

            var any = accidentDataset.Where(ar => ar.FiwareTrafficDataReport.ToUltraLightSyntax() == null);

            var chunks = accidentDataset.Chunk(20);


            var progress = 0;
            var percentage = (decimal) 0 / accidentDataset.Count;
            var taskList = new List<Task>();

            foreach (var chunk in chunks)
            {
                using var fiwareClient = new FIWAREClient();
                // index of iteration to select correct device
                var index = 0;

                foreach (var report in chunk)
                {
                    taskList.Add(fiwareClient.SendJson(HttpMethod.Post, 
                        FIWAREUrls.JsonMeasurementUrl(FIWAREUrls.WeatherDeviceIds.Skip(index).FirstOrDefault()), report.FiwareWeatherReport));
                    taskList.Add(Task.Delay(600));
                    taskList.Add(fiwareClient.SendUltraLight(HttpMethod.Post, 
                        FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.Skip(index).FirstOrDefault()), report.FiwareTrafficDataReport.ToUltraLightSyntax()));

                    await Task.WhenAll(taskList);

                    //var weatherResponseMessage = await weatherClient.SendJson(HttpMethod.Post, 
                    //    FIWAREUrls.JsonMeasurementUrl(FIWAREUrls.WeatherDeviceIds.Skip(index).FirstOrDefault()), report.FiwareWeatherReport);
                    //await Task.Delay(80);
                    //var trafficResponseMessage = await trafficClient.SendUltraLight(HttpMethod.Post, 
                    //    FIWAREUrls.UltraLightMeasurementUrl(FIWAREUrls.RoadTrafficDeviceIds.Skip(index).FirstOrDefault()), report.FiwareTrafficDataReport.ToUltraLightSyntax());

                    //if (!trafficResponseMessage.IsSuccessStatusCode)
                    //{
                    //    var messageSent = report.FiwareTrafficDataReport.ToUltraLightSyntax();
                    //}


                    index++;
                    progress++;
                }
            }

            return Ok();
        }
    }
}
