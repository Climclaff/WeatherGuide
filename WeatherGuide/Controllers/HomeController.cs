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
using System.Globalization;
using WeatherGuide.Helpers.Geolocation;
using WeatherGuide.Models.Geolocation;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using WeatherGuide.Helpers;
using WeatherGuide.Models.ViewModels;
using WeatherGuide.Attributes;

namespace WeatherGuide.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IRecommendationService _recommendationService;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IDistributedCache _cache;
        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer, 
                              IStringLocalizer<SharedResource> sharedLocalizer, IRecommendationService recommendationService, 
                              UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                              IDistributedCache cache)
        {
            _logger = logger;
            _localizer = localizer;
            _sharedLocalizer = sharedLocalizer;
            _userManager = userManager;
            _recommendationService = recommendationService;
            _signInManager = signInManager;
            _cache = cache;
        }

        public IActionResult Index()
        {              
            ViewData["Title"] = _localizer["Header"];  
            return View();
        }


        [WebRequestLimit(Name = "Limit Recommendation Get", Seconds = 10, MaxRequestCount = 3)]
        public async Task<IActionResult> Recommendations()
        {
            if (User.Identity.IsAuthenticated)
            {
                CultureInfo culture = CultureInfo.CurrentCulture;
                var user = await _userManager.GetUserAsync(HttpContext.User);
                Recommendation recommendation;
                RecommendationViewModel model = new RecommendationViewModel();
                var distCacheRecommendation = await _cache.GetAsync(user.Id.ToString() + "recommendation"); 
                if(distCacheRecommendation == null)
                {
                    recommendation = await _recommendationService.GetRecommendation(user);
                    model.FirstClothingImage = recommendation.FirstClothing.ImageData;
                    model.SecondClothingImage = recommendation.SecondClothing.ImageData;
                    model.ThirdClothingImage = recommendation.ThirdClothing.ImageData;
                    model.FirstClothingNameEN = recommendation.FirstClothing.NameEN;
                    model.FirstClothingNameUA = recommendation.FirstClothing.NameUA;
                    model.SecondClothingNameEN = recommendation.SecondClothing.NameEN;
                    model.SecondClothingNameUA = recommendation.SecondClothing.NameUA;
                    model.ThirdClothingNameEN = recommendation.ThirdClothing.NameEN;
                    model.ThirdClothingNameUA = recommendation.ThirdClothing.NameUA;

                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(900));
                    await _cache.SetAsync(user.Id.ToString() + "recommendation", model.ToByteArray(), cacheEntryOptions);
                }
                else
                {
                    model = distCacheRecommendation.FromByteArray<RecommendationViewModel>();
                }

                Measurement measurement = await _recommendationService.FindUserMeasurement(user);


                if (culture.Name == "en-US")
                {
                    IEnumerable<string> itemNamesEN = new List<string>() {
                    model.FirstClothingNameEN,
                    model.SecondClothingNameEN,
                    model.ThirdClothingNameEN,
                    };
                    TempData["Items"] = itemNamesEN;
                    IEnumerable<string> measurementValues = new List<string>() {
                        Convert.ToString((int)(measurement.Temperature * 1.8) + 32)+"°F",
                        Convert.ToString(measurement.WindSpeed * 2)+" miles/hour",
                        Convert.ToString(measurement.Humidity) + "%"};
                        TempData["Measurement"] = measurementValues;
                }
                else
                {
                    IEnumerable<string> itemNamesUA = new List<string>()
                {
                    model.FirstClothingNameUA,
                    model.SecondClothingNameUA,
                    model.ThirdClothingNameUA,
                };
                    TempData["Items"] = itemNamesUA;
                    IEnumerable<string> measurementValues = new List<string>() {
                        Convert.ToString(measurement.Temperature)+"°C",
                        Convert.ToString(measurement.WindSpeed)+" м/с",
                        Convert.ToString(measurement.Humidity) + "%"};
                    TempData["Measurement"] = measurementValues;
                }                
                TempData.Keep("Items");
                TempData.Keep("Measurement");
                return View(model);
            }
            return LocalRedirect("/Identity/Account/Login"); ;
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
