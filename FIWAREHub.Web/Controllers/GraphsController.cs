using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Xpo;
using FIWAREHub.Models.Sql;
using FIWAREHub.Models.WebModels.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FIWAREHub.Web.Controllers
{
    public class GraphsController : Controller
    {

        private readonly UnitOfWork _unitOfWork;

        public GraphsController(UnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

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

        public IActionResult GetClusterData(int year, int quarter, string state)
        {
            var clusters = _unitOfWork.Query<QuarterlyPeriod>()
                .Where(qp => qp.Year == year)
                .Where(qp => qp.Quarter == quarter)
                .Where(qp => qp.State.ToLower() == state.ToLower())
                .SingleOrDefault()
                .ClusterCentroids
                .Select(cc => new Position {Latitude = cc.Longitude, Longitude = cc.Latitude})
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
    }
}
