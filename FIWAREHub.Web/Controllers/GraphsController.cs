using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using FIWAREHub.Models.Sql;
using FIWAREHub.Models.WebModels.ViewModels;
using FIWAREHub.Web.Services;
using Microsoft.AspNetCore.Mvc;

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
                .Select(rtr => new Position {Longitude = rtr.Longitude.Value, Latitude = rtr.Latitude.Value})
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
                .SingleOrDefault()
                .ClusterCentroids
                .Select(cc => new Position {Latitude = cc.Latitude, Longitude = cc.Longitude})
                .ToList();

            return Json(clusters);
        }

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

        public async Task<IActionResult> Probabilities()
        {
            return View(new ProbabilitiesViewModel
            {
                States = _cachingService.States,
                WeatherEvents = _cachingService.WeatherEvents,
                WeatherSeverities = _cachingService.WeatherSeverities
            });
        }

        public async Task<IActionResult> ProbabilitiesQuery()
        {
            var test = _unitOfWork.Query<RoadTrafficReport>()
                .Join(_unitOfWork.Query<WeatherReport>(),
                    rtr => rtr.UID,
                    wr => wr.UID,
                    (rtr, wr) => new
                    {
                        RoadTrafficReport = rtr,
                        WeatherReport = wr
                    })
                .Count(fr => fr.RoadTrafficReport.State == "CA" && fr.WeatherReport.Severity == "Heavy");



            //var mleo = test.ToList();
            
            return View();
        }

        public IActionResult GetStateCities(string state, string search, int page)
        {
            var objectsPerPage = 100;
            var skip = objectsPerPage * page - 1;

            // Initialize search term if null
            search ??= string.Empty;

            var results = _cachingService.StateCities
                .Where(sc => sc.state == state)
                .Where(sc => sc.city.Contains(search))
                .Skip(skip)
                .Take(objectsPerPage);

            var obj = new {results = results, pagination = new {more = true}};

            return Json(obj);
        }
    }
}
