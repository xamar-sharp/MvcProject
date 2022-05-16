using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MvcProject.Models;
using Microsoft.Extensions.Logging;
using System.IO;
namespace MvcProject.Areas.Standard.Controllers
{
    [Authorize(Roles = "USER, ADMIN")]
    [Area("Standard")]
    public class HomeController : Controller
    {
        private readonly WebContext _ctx;
        private readonly ILogger _logger;
        public HomeController(WebContext ctx)
        {
            _ctx = ctx;
            _logger = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(new LoggerProvider("C:\\log.json"));
            }).CreateLogger(this.GetType().FullName);
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(model: _ctx.Users.Include(user=>user.References).FirstOrDefault(user => user.Email == User.Identity.Name));
        }

        [HttpGet]
        public IActionResult SendImage()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendImage([FromForm] IFormFile image)
        {
            
            string path = Path.Combine("C:\\MvcProjectData", $"{Guid.NewGuid().ToString()}.jpg");
            User currentUser = await _ctx.Users.FirstOrDefaultAsync(user => user.Email == User.Identity.Name);
            using (FileStream stream = System.IO.File.Create(path))
            {
                using (var reading = image.OpenReadStream())
                {
                    byte[] incomingData = new byte[reading.Length];
                    await reading.ReadAsync(incomingData);
                    await stream.WriteAsync(incomingData);
                }
            }
            await _ctx.FileReferences.AddAsync(new FileReference() { AbsolutePath = path, User = currentUser });
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
