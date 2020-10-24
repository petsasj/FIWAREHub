using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DevExpress.Xpo;
using FIWAREHub.Models.ParserModels;
using FIWAREHub.Models.Sql;
using FIWAREHub.Parsers;
using FIWAREHub.Web.Extensions;
using FIWAREHub.Web.Models;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIWAREHub.Web.Controllers.Api
{
    public class IoTMeasurementController : ControllerBase
    {
        // Unit of Work from Dependency Injection
        private readonly UnitOfWork _unitOfWork;

        public IoTMeasurementController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Test action for testing a single measurement post to JSON Devices
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Test action for testing a single measurement post to UL device
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Main Controller function
        /// 1. Gets typed files from dataset
        /// 2. Splits into chunks of 20
        /// 3. Submits each part to appropriate IoT agent
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SubmitCsvReadings(int delay = 60)
        {
            // Instance of File parser 
            var fileParser = new FileParser();

            // Get predefined dataset results
            var accidentDataset = fileParser.ParseAccidentsDataset().Take(100000).ToList();

            // Split into chunks of 20 for easier use with weather devices
            var chunks = accidentDataset.Chunk(20);

            // Diagnostics
            var progress = 0;
            var percentage = (decimal) progress / accidentDataset.Count;
            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            // Adds async Tasks to list for execution
            var taskList = new List<Task>();

            // POST measurements to IoT devices
            foreach (var chunk in chunks)
            {
                using var fiwareClient = new FIWAREClient();
                // index of iteration to select correct device
                var index = 0;

                foreach (var report in chunk)
                {
                    // POST to JSON
                    taskList.Add(fiwareClient.SendJson(HttpMethod.Post,
                        FIWAREUrls.JsonMeasurementUrl(
                            FIWAREUrls.WeatherDeviceIds.Skip(index).FirstOrDefault()),
                        report.FiwareWeatherReport));
                    
                    // Delay Task for operation completion
                    taskList.Add(Task.Delay(delay));

                    // POST to UL
                    taskList.Add(fiwareClient.SendUltraLight(HttpMethod.Post,
                        FIWAREUrls.UltraLightMeasurementUrl(
                            FIWAREUrls.RoadTrafficDeviceIds.Skip(index).FirstOrDefault()),
                        report.FiwareTrafficDataReport.ToUltraLightSyntax()));

                    // await Task execution
                    await Task.WhenAll(taskList);
                    index++;
                    progress++;
                }
            }

            // Diagnostics continued
            stopWatch.Stop();
            var elapsedTime = stopWatch.ElapsedMilliseconds / 1000 / 60;

            return Ok($"Successfully sent {progress} within {elapsedTime} minutes.");
        }


        /// <summary>
        /// Action was used for diagnostic purposes due to Count mismatch between sent
        /// measurements and actually stored devices
        /// Was only happening in UltraLight Devices
        /// Further investigated that was down to reserved characters
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public async Task<IActionResult> FindDifferences()
        {
            var fileParser = new FileParser();
            var accidentDataset = fileParser.ParseAccidentsDataset().Take(2000).ToList();

            var roadTrafficReports = await _unitOfWork.Query<RoadTrafficReport>().ToListAsync();

            var missing = accidentDataset
                .Where(ad => !roadTrafficReports.Any(rtr =>
                    rtr.Country == ad.FiwareTrafficDataReport.Country &&
                    rtr.City == ad.FiwareTrafficDataReport.City &&
                    rtr.State == ad.FiwareTrafficDataReport.State &&
                    rtr.StartTime == ad.FiwareTrafficDataReport.StartTime &&
                    rtr.Street == ad.FiwareTrafficDataReport.Street))
                .ToList();

            return Ok();
        }
    }
}
