using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.Math.Distances;
using Accord.Statistics;
using DevExpress.Xpo;
using FIWAREHub.Models.Extensions;
using FIWAREHub.Models.Sql;
using FIWAREHub.Models.WebModels.ViewModels;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Device.Location;
using System.Globalization;

namespace FIWAREHub.Web.Controllers
{
    public class GraphsController : Controller
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly CachingService _cachingService;

        public GraphsController(UnitOfWork unitOfWork, CachingService cachingService)
        {
            this._unitOfWork = unitOfWork;
            this._cachingService = cachingService;
        }

        #region Clusters

        [Route("{controller}/{action}/{state}/{year}/{quarter}")]
        public IActionResult GeoCluster(int year, int quarter, string state)
        {
            var model = new GeoClusterViewModel
            {
                Year = year,
                Quarter = quarter,
                State = state
            };

            return View(model);
        }

        [Route("{controller}/{action}/{state}/{year}/{quarter}")]
        public IActionResult GetAccidentData(int year, int quarter, string state)
        {
            var qt = this.quarterTuple(quarter);
            var roadAccidents = _unitOfWork.Query<RoadTrafficReport>()
                .Where(rtr => rtr.StartTime != null)
                .Where(rtr => rtr.State.ToLower() == state.ToLower())
                .Where(rtr =>
                    rtr.StartTime.Value.Year == year &&
                    rtr.StartTime.Value.Month >= qt.startMonth &&
                    rtr.StartTime.Value.Month <= qt.endMonth)
                .Select(rtr => new Position { Longitude = rtr.Longitude.Value, Latitude = rtr.Latitude.Value })
                .ToList();

            return Json(roadAccidents);
        }

        [Route("{controller}/{action}/{state}/{year}/{quarter}")]
        public IActionResult GetClusterData(int year, int quarter, string state)
        {
            var clusters = _unitOfWork.Query<QuarterlyPeriod>()
                .Where(qp => qp.Year == year)
                .Where(qp => qp.Quarter == quarter)
                .Where(qp => qp.State.ToLower() == state.ToLower())
                .SingleOrDefault()?
                .ClusterCentroids
                .Select(cc => new Position { Latitude = cc.Latitude, Longitude = cc.Longitude })
                .ToList();

            return Json(clusters);
        }

        #endregion

        #region Probabilities

        public async Task<IActionResult> Probabilities()
        {
            return View(new ProbabilitiesViewModel
            {
                States = _cachingService.States,
                WeatherEvents = _cachingService.WeatherEvents,
                WeatherSeverities = _cachingService.WeatherSeverities
            });
        }

        /// <summary>
        /// API Action that returns statistic of probabilities for select query filters
        /// </summary>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="quarter"></param>
        /// <param name="accidentSeverity"></param>
        /// <param name="weatherEvent"></param>
        /// <param name="weatherSeverity"></param>
        /// <param name="matchingWeatherConditions"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ProbabilitiesQuery(string state, string city, string street, int? quarter,
            int? accidentSeverity, string weatherEvent, string weatherSeverity, bool matchingWeatherConditions)
        {
            var statistics = await GetStatistics(state, city, street, quarter,
                accidentSeverity, weatherEvent, weatherSeverity, matchingWeatherConditions);

            return Json(statistics);
        }

        /// <summary>
        /// Portal Action that displays Probabilities results (top 15) in graphical format
        /// </summary>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="quarter"></param>
        /// <param name="accidentSeverity"></param>
        /// <param name="weatherEvent"></param>
        /// <param name="weatherSeverity"></param>
        /// <param name="matchingWeatherConditions"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ProbabilitiesResults(string state, string city, string street, int? quarter,
            int? accidentSeverity, string weatherEvent, string weatherSeverity, bool matchingWeatherConditions)
        {
            var statistics = await GetStatistics(state, city, street, quarter, 
                accidentSeverity, weatherEvent, weatherSeverity, matchingWeatherConditions);
            var occurrencesCount = statistics.Take(15);

            return View(new ProbabilitiesResultsViewModel
            {
                Labels = occurrencesCount.Select(o => o.Street).ToList(),
                Percentages = occurrencesCount.Select(o => o.Percentage).ToList()
            });
        }

        /// <summary>
        /// Common query for API Action and Portal View
        /// Calculates probability of accident based on passed parameters
        /// </summary>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="street"></param>
        /// <param name="quarter"></param>
        /// <param name="accidentSeverity"></param>
        /// <param name="weatherEvent"></param>
        /// <param name="weatherSeverity"></param>
        /// <param name="matchingWeatherConditions"></param>
        /// <returns></returns>
        private async Task<IQueryable<ProbabilityStatistic>> GetStatistics(string state, string city, string street, int? quarter,
            int? accidentSeverity, string weatherEvent, string weatherSeverity, bool matchingWeatherConditions)
        {
            // Join two entities on UID
            var queryable = _unitOfWork.Query<RoadTrafficReport>()
                .Join(_unitOfWork.Query<WeatherReport>(),
                    rtr => rtr.UID,
                    wr => wr.UID,
                    (rtr, wr) => new
                    {
                        RoadTrafficReport = rtr,
                        WeatherReport = wr
                    })
                .Where(fr => fr.RoadTrafficReport.State == state);

            // if city exists, populate query
            if (!string.IsNullOrWhiteSpace(city))
            {
                queryable = queryable.Where(fr => fr.RoadTrafficReport.City == city);
            }

            // if street exists, populate query
            if (!string.IsNullOrWhiteSpace(street))
            {
                queryable = queryable.Where(fr => fr.RoadTrafficReport.Street == street);
            }

            // if weather event exists, populate query
            if (!string.IsNullOrWhiteSpace(weatherEvent))
            {
                queryable = queryable.Where(fr => fr.WeatherReport.WeatherEvent == weatherEvent);
            }

            // if weather severity exists, populate query
            if (!string.IsNullOrWhiteSpace(weatherSeverity))
            {
                queryable = queryable.Where(fr => fr.WeatherReport.Severity == weatherSeverity);
            }

            // if accident severity exists, populate query
            if (accidentSeverity.HasValue && accidentSeverity.Value > 0)
            {
                queryable = queryable.Where(fr => fr.RoadTrafficReport.Severity == accidentSeverity);
            }

            // if quarter exists, get start month and end month from helper method
            // then populate query
            if (quarter.HasValue && quarter.Value > 0)
            {
                var qt = quarterTuple(quarter.Value);
                queryable = queryable.Where(fr =>
                    fr.RoadTrafficReport.StartTime.Value.Month >= qt.startMonth &&
                    fr.RoadTrafficReport.StartTime.Value.Month <= qt.endMonth);
            }

            // Query for counting total accidents
            var totalAccidentsQueryable = _unitOfWork.Query<RoadTrafficReport>()
                .Join(_unitOfWork.Query<WeatherReport>(),
                    rtr => rtr.UID,
                    wr => wr.UID,
                    (rtr, wr) => new
                    {
                        RoadTrafficReport = rtr,
                        WeatherReport = wr
                    })
                .Where(fr => fr.RoadTrafficReport.State == state);


            // if person inquiring wants the total accidents or just the accidents
            // whose weather conditions match the weather conditions of query
            if (matchingWeatherConditions)
            {
                if (!string.IsNullOrWhiteSpace(weatherEvent))
                {
                    totalAccidentsQueryable =
                        totalAccidentsQueryable.Where(ta => ta.WeatherReport.WeatherEvent == weatherEvent);
                }

                if (!string.IsNullOrWhiteSpace(weatherSeverity))
                {
                    totalAccidentsQueryable =
                        totalAccidentsQueryable.Where(ta => ta.WeatherReport.Severity == weatherSeverity);
                }
            }

            // count of accidents
            var totalAccidentsCount = await totalAccidentsQueryable.CountAsync();

            // SELECT statement into Probability Statistic class
            var occurrencesCount = queryable
                .GroupBy(q => new {q.RoadTrafficReport.City, q.RoadTrafficReport.Street, q.RoadTrafficReport.County})
                .Select(g => new ProbabilityStatistic
                {
                    Street = $"{g.Key.Street}, {g.Key.City}" ,
                    Count = g.Count(),
                    Percentage = ((double) g.Count() / totalAccidentsCount) * 100
                }).OrderByDescending(o => o.Count);

            return occurrencesCount;
        }

        /// <summary>
        /// AJAX Action
        /// Helps populate the select dropdown of Probabilities Web Page
        /// </summary>
        /// <param name="state"></param>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IActionResult GetStateCities(string state, string search, int page)
        {
            var objectsPerPage = 100;
            var skip = objectsPerPage * page - 1;

            // Initialize search term if null
            search ??= string.Empty;

            var results = _cachingService.StateCities
                .Where(sc => sc.state == state)
                .Where(sc => sc.city?.ToLower().Contains(search.ToLower()) == true)
                .OrderBy(sc => sc.city)
                .Select(sc => new { id = sc.city, text = sc.city })
                .Skip(skip)
                .Take(objectsPerPage + 1)
                .ToList();

            var obj = new
            {
                results = results.Take(objectsPerPage),
                pagination = new { more = results.Count == objectsPerPage + 1 }
            };

            return Json(obj);
        }

        /// <summary>
        /// AJAX Action
        /// Helps populate the select dropdown of Probabilities web page
        /// </summary>
        /// <param name="state"></param>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public IActionResult GetStateStreets(string state, string search, int page)
        {
            var objectsPerPage = 100;
            var skip = objectsPerPage * page - 1;

            // Initialize search term if null
            search ??= string.Empty;

            var results = _cachingService.StateStreets
                .Where(sc => sc.state == state)
                .Where(sc => sc.street?.ToLower().Contains(search.ToLower()) == true)
                .OrderBy(sc => sc.street)
                .Select(sc => new { id = sc.street, text = sc.street })
                .Skip(skip)
                .Take(objectsPerPage + 1)
                .ToList();

            var obj = new
            {
                results = results.Take(objectsPerPage),
                pagination = new { more = results.Count == objectsPerPage + 1 }
            };

            return Json(obj);
        }

        #endregion

        #region ClusterFinder

        [HttpGet]
        public async Task<IActionResult> ClusterFinder()
        {
            var model = new GeoClusterViewModel
            {
                Year = 2017,
                Quarter = 1,
                State = "CA"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetNearestCluster(DateTime incidentDay, double? latitude, double? longitude)
        {
            if (incidentDay.Equals(DateTime.MinValue) || !latitude.HasValue || !longitude.HasValue)
                return null;

            var clusterCentroids = await _unitOfWork.Query<QuarterlyPeriod>()
                .Where(qp => qp.Quarter == incidentDay.GetQuarter())
                .SelectMany(qp => qp.ClusterCentroids)
                .ToListAsync();

            var coordinate = new GeoCoordinate(latitude.Value, longitude.Value);
            var orderedCoordinates = clusterCentroids.Select(x => new GeoCoordinate(x.Latitude, x.Longitude))
                .OrderBy(x => x.GetDistanceTo(coordinate));
            var nearest = orderedCoordinates.First();

            var result = clusterCentroids.FirstOrDefault(cc =>
                cc.Latitude == nearest.Latitude && cc.Longitude == nearest.Longitude);

            if (result == null)
                throw new ArgumentException("Could not calculate the nearest coordinate");

            var red = 178;
            var green = 34;
            var blue = 34;
            return Json(new {result.Latitude, result.Longitude, red, green, blue, tooltip = "Nearest Ambulance"});
        }

        #endregion

        #region Helpers

        private (int startMonth, int endMonth) quarterTuple(int quarter)
        {
            return quarter switch
            {
                1 => (1, 3),
                2 => (4, 6),
                3 => (7, 9),
                4 => (10, 12),
                _ => (0, 0),
            };
        }

        #endregion
    }
}
