using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspNetExample.Models;
using AspNetExample.Collectors;

namespace AspNetExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeCollectors _collectors;
        public HomeController(IHomeCollectors collectors)
        {
            _collectors = collectors;
        }
        public IActionResult Index()
        {
            _collectors.PageViewsCount.Inc();
            return View();
        }

        public IActionResult Privacy()
        {
            _collectors.PageViewsCount.Inc();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _collectors.PageErrorsCount.Inc();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
