using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherGuide.Attributes;
using WeatherGuide.Helpers.Geolocation;
using WeatherGuide.Models;
using WeatherGuide.Models.Geolocation;
using WeatherGuide.Repository;

namespace WeatherGuide.Areas.Identity.Pages.Account.Manage
{
    [WebRequestLimitRazor(Name = "Limit Profile Web", Seconds = 10, MaxRequestCount = 30)]
    public class GeolocationModel : PageModel
    {
        private readonly UserManager<Models.AppUser> _userManager;
        private readonly SignInManager<Models.AppUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly IGeolocationRepository _geolocationRepository = null;
        private readonly IDistributedCache _cache;
        public GeolocationModel(
            UserManager<Models.AppUser> userManager,
            SignInManager<Models.AppUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            IStringLocalizer<SharedResource> sharedLocalizer,
            IGeolocationRepository geolocationRepository,
            IDistributedCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _geolocationRepository = geolocationRepository;
            _cache = cache; 
        }

        [TempData]
        public string StatusMessage { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {   
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var cacheEntry = _cache.GetAsync(user.Id.ToString() + "recommendation");
            if (cacheEntry.Result != null)
            {
                StatusMessage = _sharedLocalizer["Recommendation cooldown"];
                return RedirectToPage();
            }

            GeoInfoModel geoInfo = new GeoInfoModel();
            GeoHelper geoHelper = new GeoHelper();
            var result = await geoHelper.GetGeoInfo();
            if (result != null)
            {
                geoInfo = JsonConvert.DeserializeObject<GeoInfoModel>(result);
                bool isSupported = await _geolocationRepository.DBSupportsLocation(geoInfo);
                if (isSupported)
                {
                    await _geolocationRepository.ApplyGeolocationToUser(user.Id, geoInfo);
                    await _signInManager.RefreshSignInAsync(user);
                    _logger.LogInformation("User changed their location with geolocation service successfully.");
                    StatusMessage = _sharedLocalizer["Geolocation service changed your location"];
                    return RedirectToPage();
                }
                StatusMessage = _sharedLocalizer["Geolocation service could not locate you, it means that your country or region is not supported by our application."];
            }

            return RedirectToPage();
        }
    }
}
