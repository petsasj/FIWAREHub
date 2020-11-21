using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FIWAREHub.Parsers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FIWAREHub.Web.Models;
using FIWAREHub.Web.Services;

namespace FIWAREHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CachingService _cachingService;

        public HomeController(ILogger<HomeController> logger, CachingService cachingService)
        {
            _logger = logger;
            this._cachingService = cachingService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
