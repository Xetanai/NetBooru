using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetBooru.Web.Models;
using NetBooru.Web.Options;

namespace NetBooru.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOptionsMonitor<LandingOptions> _options;

        private readonly Random _rng;

        public HomeController(
            ILogger<HomeController> logger,
            IOptionsMonitor<LandingOptions> options)
        {
            _logger = logger;
            _options = options;

            _rng = new Random();
        }

        public IActionResult Index()
        {
            var options = _options.CurrentValue;

            return options.DisplayLandingPage
                ? View(new PostCountViewModel
                {
                    PostCount = _rng.Next()
                }) as IActionResult
                : RedirectToActionPermanent(
                    nameof(PostsController.List),
                    nameof(PostsController));
        }
    }
}
