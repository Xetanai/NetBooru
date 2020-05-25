using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBooru.Data;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly NetBooruContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            NetBooruContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult Index()
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
