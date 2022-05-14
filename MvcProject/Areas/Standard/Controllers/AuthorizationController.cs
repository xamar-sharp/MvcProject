using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvcProject.ViewModels;
using MvcProject.Models;
namespace MvcProject.Areas.Standard.Controllers
{
    [Authorize(Roles ="USER, ADMIN",Policy ="default")]
    [Area("Standard")]
    public class AuthorizationController:Controller
    {
        private readonly WebContext _ctx;
        private readonly ILogger _logger;
        public AuthorizationController(WebContext ctx)
        {
            _ctx = ctx;
            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new LoggerProvider("C:\\log.json"));
            }).CreateLogger(this.GetType().FullName);
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            (await _ctx.Users.FirstOrDefaultAsync(user => user.Email == User.Identity.Name)).IsAlive = false;
            await _ctx.SaveChangesAsync();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [AcceptVerbs("GET")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm]AuthorizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(_ctx.Users.FirstOrDefault(user=>user.Email == model.Email) is User user && user.PasswordHash==SecureData.HashPassword(model.Password))
                {
                    await Authenticate(user.Email, model.Password);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model: model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm]AuthorizationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var passwordHash = SecureData.HashPassword(model.Password);
                if (_ctx.Users.FirstOrDefault(user => user.Email == model.Email && user.PasswordHash==passwordHash) is null)
                {
                    await _ctx.Users.AddAsync(new Models.User() { IsAlive = true, Email = model.Email, PasswordHash = passwordHash });
                    await _ctx.SaveChangesAsync();
                    await Authenticate(model.Email, model.Password);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model: model);
        }
        public async Task Authenticate(string email,string password)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType,email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType,password ==  SecureData.AdminPassword?"ADMIN":"USER")
            };
            await HttpContext.SignInAsync("Cookies",new ClaimsPrincipal(new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType)));
        }
    }
}
