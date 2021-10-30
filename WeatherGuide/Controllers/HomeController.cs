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
using WeatherGuide.Models.domain;
using Microsoft.AspNetCore.Identity;

namespace WeatherGuide.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, 
                              IStringLocalizer<SharedResource> sharedLocalizer, IRecommendationRepository recommendationRepository,
                              UserManager<AppUser> userManager)
        {
            _logger = logger;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
            _userManager = userManager;
            _recommendationRepository = recommendationRepository;
        }

        public IActionResult Index()
        {
            var userId = Convert.ToInt32(_userManager.GetUserId(HttpContext.User));
            var user = _recommendationRepository.GetUser(userId);
            Recommender recommender = new Recommender(_recommendationRepository, user);
            recommender.Recommend();
            ViewData["Title"] = _localizer["Header"];
            return View();
        }

        public IActionResult Privacy()
        {
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
