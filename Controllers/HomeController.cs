using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkillSwapApp.Models;
using SkillSwapApp.Data;

namespace SkillSwapApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            // Fetch a few sample users to show on the public landing page
            // Only show users who have filled their profile fields
            var featuredUsers = _db.Users
                .Where(u => !string.IsNullOrEmpty(u.FullName)
                         && !string.IsNullOrEmpty(u.OfferedSkill)
                         && !string.IsNullOrEmpty(u.NeededSkill))
                .OrderBy(u => u.FullName)
                .Take(3)
                .ToList();

            return View(featuredUsers);
        }

        public IActionResult Privacy()
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