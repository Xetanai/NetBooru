using System;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBooru.Data;

namespace NetBooru.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly NetBooruContext _dbContext;
        private readonly ILogger<UploadController> _logger;
        private readonly UserManager<User> _userManager;

        public UploadController(ILogger<UploadController> logger,
            NetBooruContext dbContext,
            UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize("CanPostFile")]
        public async Task<IActionResult> Upload (
            IFormFile file,
            string[]? initialTags)
        {
            _logger.LogInformation("Handling file upload");

            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            using var hashSlingingSlasher = SHA256.Create();
            var hash = await hashSlingingSlasher.ComputeHashAsync(file.OpenReadStream());

            var finalFileHash = BitConverter.ToString(hash).Replace("-","");
            var hashPre = finalFileHash.Substring(0, 3);

            using var destFile = System.IO.File.Create(Path.Combine(hashPre, finalFileHash));
            await file.OpenReadStream().CopyToAsync(destFile);

            // TODO: Generate Metadata
            // TODO: Tokenize supplied tags and apply
            var newPost = new Post()
            {
                Uploader = user,
                Hash = finalFileHash,
                Metadata = new PostMetadata()
            };

            _dbContext.Posts.Add(newPost);
            await _dbContext.SaveChangesAsync();

            // TODO: Less magic thx
            return Created("http://localhost:5000/Post/"+ newPost.Id, true);
        }

    }
}
