using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBooru.Data;
using NetBooru.Web.Models;

namespace NetBooru.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly NetBooruContext _dbContext;
        private readonly ILogger<PostsController> _logger;

        public PostsController(ILogger<PostsController> logger,
            NetBooruContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IActionResult List(string? query = null)
        {
            _logger.LogInformation(
                "Querying for posts by the query {query}",
                query);
            ViewBag.Query = query;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
