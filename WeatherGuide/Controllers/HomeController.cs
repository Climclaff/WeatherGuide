using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using WeatherGuide.Repository;
using Microsoft.AspNetCore.Identity;
using WeatherGuide.Services;
namespace WeatherGuide.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IRecommendationService _recommendationService;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, 
                              IStringLocalizer<SharedResource> sharedLocalizer, IRecommendationService recommendationService, 
                              UserManager<AppUser> userManager)
        {
            _logger = logger;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
            _userManager = userManager;
            _recommendationService = recommendationService;
        }

        public IActionResult Index()
        {              
            ViewData["Title"] = _localizer["Header"];
            return View();
        }

        public async Task<IActionResult> Recommendations()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string recommendation = await _recommendationService.GetRecommendation(user);
            return View();
        }

        [Authorize("IsAdminPolicy")]
        public IActionResult Administration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
