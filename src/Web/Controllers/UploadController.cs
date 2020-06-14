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
using NetBooru.Web.Services;

namespace NetBooru.Web.Controllers
{
    public class UploadController : Controller
    {
        private readonly NetBooruContext _dbContext;
        private readonly ILogger<UploadController> _logger;
        private readonly FileUploadService _uploadService;
        private readonly UserManager<User> _userManager;

        public UploadController(ILogger<UploadController> logger,
            NetBooruContext dbContext,
            FileUploadService uploadService,
            UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _uploadService = uploadService;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize("CanPostFiles")]
        public async Task<IActionResult> Upload (
            IFormFile file,
            string[]? initialTags)
        {
            _logger.LogInformation("Handling file upload");

            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var upload = await _uploadService.UploadFileAsync(file);

            // TODO: Generate Metadata
            // TODO: Tokenize supplied tags and apply
            var newPost = new Post
            {
                Uploader = user,
                Hash = upload.Hash,
                Metadata = new PostMetadata
                {
                    MimeType = upload.MediaType
                }
            };

            _ = _dbContext.Posts.Add(newPost);
            _ = await _dbContext.SaveChangesAsync();

            // TODO: Less magic thx
            return Created("http://localhost:5000/Post/"+ newPost.Id, true);
        }

    }
}
