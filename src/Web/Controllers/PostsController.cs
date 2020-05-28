using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> List(string? query = null)
        {
            _logger.LogInformation(
                "Querying for posts by the query {query}",
                query);

            var tokens = query?.Split(' ') ?? Array.Empty<string>();

            var tags = _dbContext.Tags
                .Where(t => tokens.Contains(t.Name));

            var posts = await _dbContext.Posts
                .SelectMany(p => p.PostTags)
                .Join(tags,
                    pt => pt.Tag,
                    t => t,
                    (pt, t) => pt.Post)
                .Distinct()
                .Select(x => new PostListViewModel.PostListPost
                {
                    PostUrl = $"{x.Id} - {(x.Uploader == null ? 0 : x.Uploader.Id)}",
                    ThumbnailUrl = $"{x.Hash}"
                })
                .ToListAsync();


            ViewBag.Query = query;
            return View(new PostListViewModel
            {
                Posts = posts
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
