using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetBooru.Data;

namespace NetBooru.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            ILogger<AuthController> logger,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register (string username, string password)
        {
            _logger.LogInformation("Attempting to resgister user " + username);

            var user = new User()
            {
                UserName = username
            };
            var res = await _userManager.CreateAsync(user, password);

            if(!res.Succeeded)
            {
                foreach(var err in res.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }

                return RedirectToAction(nameof(Login));
            }

            await _signInManager.SignInAsync(user, true);

            return LocalRedirect("/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login (string username, string password)
        {
            _logger.LogInformation("Attempting to login user " + username);

            // TODO: Do like, 3 chances or something idk
            var res = await _signInManager.PasswordSignInAsync(username, password, true, false);

            if(!res.Succeeded)
            {
                ModelState.AddModelError("err", "Login unsuccessful.");
                return View();
            }
            if (res.IsLockedOut)
            {
                ModelState.AddModelError("err", "Account locked.");
                return View();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect("/");
        }
    }
}
