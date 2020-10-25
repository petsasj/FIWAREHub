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
            var weatherEvent = new FiwareWeatherReport(1)
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
            var roadTraffic = new FiwareTrafficReport(1)
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
        public async Task<IActionResult> SynchronizeReadings(int delay = 60)
        {
            var guid = Guid.NewGuid();
            var measurementsSubmitter = new FIWAREMeasurementsSubmitter();

            var task = Task.Factory.StartNew(async () => await measurementsSubmitter.SubmitToAgentsAsyncTask(guid, delay),
                TaskCreationOptions.LongRunning);

            return AcceptedAtAction(nameof(QueryTaskProgress), "IoTMeasurement", new { guid = guid.ToString() });
        }

        /// <summary>
        /// Queries Sync Operation Progress
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> QueryTaskProgress(string guid)
        {
            var successful = Guid.TryParse(guid, out var uniqueId);

            if (!successful)
                return BadRequest("Malformed GUID.");

            var syncOperation = await _unitOfWork.Query<SyncOperation>()
                .Where(so => so.UniqueId == uniqueId)
                .SingleOrDefaultAsync();

            if (syncOperation == null)
                return BadRequest("No progress found for GUID.");

            var json = new
            {
                DateStarted = syncOperation.DateStarted.ToString("yyyy-MM-dd HH:mm"),
                DateLastModified = syncOperation.DateModified.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm"),
                DateFinished = syncOperation.DateFinished.GetValueOrDefault().ToString("yyyy-MM-dd HH:mm"),
                Percentage = syncOperation.ProgressPercentage.GetValueOrDefault(),
                IsRunning = syncOperation.IsRunning
            };

            return Ok(json);
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
