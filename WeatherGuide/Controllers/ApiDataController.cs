using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WeatherGuide.Attributes;
using WeatherGuide.Data;
using WeatherGuide.Helpers;
using WeatherGuide.Helpers.Geolocation;
using WeatherGuide.Models;
using WeatherGuide.Models.ApiModels;
using WeatherGuide.Models.Geolocation;
using WeatherGuide.Models.ViewModels;
using WeatherGuide.Repository;
using WeatherGuide.Services;

namespace WeatherGuide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRecommendationService _recommendationService;
        private readonly IDistributedCache _cache;
        private readonly IGeolocationRepository _geolocationRepository;
        public ApiDataController(UserManager<AppUser> userManager,
            ApplicationDbContext context,
            IRecommendationService recommendationService,
            IDistributedCache cache,
            IGeolocationRepository geolocationRepository)
        {
            _userManager = userManager;
            _context = context;
            _recommendationService = recommendationService;
            _cache = cache;
            _geolocationRepository = geolocationRepository;
        }


        [HttpGet]
        [Route("Country")]
        public async Task<IActionResult> Country()
        {
            List<CountryModel> countries = new List<CountryModel>();
            List<Country> dbCountries = await _context.Set<Country>().ToListAsync();
            for (int i = 0; i < dbCountries.Count; ++i)
            {
                countries.Add(new CountryModel() { Id = dbCountries[i].Id, Name = dbCountries[i].Name });
            }


            return Ok(new
            {
                data = countries
            }); 
        }

        [HttpGet]
        [Route("State/{id?}")]
        public async Task<IActionResult> State([FromQuery(Name = "id")] int id)
        {
            var dbStates = await _context.Set<State>().Where(x => x.CountryId == id)
                .ToListAsync();
            List<StateModel> states = new List<StateModel>();
            for (int i = 0; i < dbStates.Count; ++i)
            {
                states.Add(new StateModel() { Id = dbStates[i].Id, Name = dbStates[i].Name });
            }
            if (states.Count == 0)
            {
                var defaultStates = await _context.Set<State>().Where(x => x.CountryId == 1)
               .ToListAsync();
                for (int i = 0; i < dbStates.Count; ++i)
                {
                    states.Add(new StateModel() { Id = defaultStates[i].Id, Name = defaultStates[i].Name });
                }
                return Ok(new
                {
                    data = states
                });
            }
            return Ok(new
            {
                data = states
            });

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("ProfileCountry")]
        public async Task<IActionResult> ProfileCountry()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var curUser = await _userManager.FindByNameAsync(username);
            if (curUser == null)
            {
                return BadRequest();
            }
            CountryModel country = new CountryModel();
            country.Id = (int)curUser.CountryId;
            Country dbCountry = await _context.Set<Country>().Where(x => x.Id == country.Id).FirstOrDefaultAsync();
            country.Name = dbCountry.Name;
            List<CountryModel> countries = new List<CountryModel>();
            List<Country> dbCountries = await _context.Set<Country>().ToListAsync();

            for (int i = 0; i < dbCountries.Count; ++i)
            {
                countries.Add(new CountryModel() { Id = dbCountries[i].Id, Name = dbCountries[i].Name });
            }


            return Ok(new
            {
                data = countries,
                country = country
            });

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("ProfileState")]
        public async Task<IActionResult> ProfileState()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var curUser = await _userManager.FindByNameAsync(username);
            if (curUser == null)
            {
                return BadRequest();
            }
            var dbStates = await _context.Set<State>().Where(x => x.CountryId == curUser.CountryId)
                .ToListAsync();
            State dbState = await _context.Set<State>().Where(x => x.Id == curUser.StateId).FirstOrDefaultAsync();
            StateModel state = new StateModel();
            state.Id = dbState.Id;
            state.Name = dbState.Name;
            List<StateModel> states = new List<StateModel>();
            for (int i = 0; i < dbStates.Count; ++i)
            {
                states.Add(new StateModel() { Id = dbStates[i].Id, Name = dbStates[i].Name });
            }
            if (states.Count == 0)
            {
                var defaultStates = await _context.Set<State>().Where(x => x.CountryId == 1)
               .ToListAsync();
                for (int i = 0; i < dbStates.Count; ++i)
                {
                    states.Add(new StateModel() { Id = defaultStates[i].Id, Name = defaultStates[i].Name });
                }
                return Ok(new
                {
                    data = states,
                    state = state
                });
            }
            return Ok(new
            {
                data = states,
                state = state
            });

        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("UserInfo")]
        public async Task<IActionResult> UserInfo()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest();
            }
            var nameClaim = await _context.UserClaims.Where(x => x.ClaimType == "Name").Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            var surnameClaim = await _context.UserClaims.Where(x => x.ClaimType == "Surname").Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
            string name = nameClaim.ClaimValue.ToString();
            string surname = surnameClaim.ClaimValue.ToString();
            return Ok(new
            {
                name = name,
                surname = surname
            });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ApiRequestLimit(Name = "Limit Change Location", Seconds = 10, MaxRequestCount = 3)]
        [Route("ChangeLocation")]
        public async Task<IActionResult> ChangeLocation([FromQuery(Name = "countryid")] int countryId, [FromQuery(Name = "stateid")] int stateId)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            var cacheEntry = _cache.GetAsync(user.Id.ToString() + "recommendation");
            if (cacheEntry.Result != null)
            {               
                return StatusCode(420);
            }
            if (countryId != user.CountryId || stateId != user.StateId)
            {
                user.CountryId = countryId;
                user.StateId = stateId;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return StatusCode(400);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ApiRequestLimit(Name = "Limit Change GeolocationService", Seconds = 10, MaxRequestCount = 3)]
        [Route("GeolocationService")]
        public async Task<IActionResult> GeolocationService()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            var cacheEntry = _cache.GetAsync(user.Id.ToString() + "recommendation");
            if (cacheEntry.Result != null)
            {
                return StatusCode(420);
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
                    return Ok();
                }              
            }
            return StatusCode(400);
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ApiRequestLimit(Name = "Limit Change Name", Seconds = 10, MaxRequestCount = 3)]
        [HttpPost]
        [Route("ChangeName")]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameModel model)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(
                new ValidationProblemDetails(ModelState));
            }
            var claims = await _userManager.GetClaimsAsync(user);
            if (model.Name != claims[0].Value)
            {
                var claim = await _context.UserClaims.Where(x => x.ClaimType == "Name").Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
                claim.ClaimValue = model.Name;
                _context.UserClaims.Update(claim);
                await _context.SaveChangesAsync();
            }
            if (model.Surname != claims[1].Value)
            {
                var claim = await _context.UserClaims.Where(x => x.ClaimType == "Surname").Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
                claim.ClaimValue = model.Surname;
                _context.UserClaims.Update(claim);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ApiRequestLimit(Name = "Limit Change Password", Seconds = 10, MaxRequestCount = 3)]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest();
            }

            return Ok();
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ApiRequestLimit(Name = "Limit Get Recommendation", Seconds = 10, MaxRequestCount = 3)]
        [HttpGet]
        [Route("Recommendation")]
        public async Task<IActionResult> Recommendation()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);

            Recommendation recommendation;
            RecommendationViewModel model = new RecommendationViewModel();
            var distCacheRecommendation = await _cache.GetAsync(user.Id.ToString() + "recommendation");
            if (distCacheRecommendation == null)
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

            if (user == null || measurement == null)
            {
                return BadRequest();
            }
            return Ok(new
            {
                measurement = measurement,
                firstClothingNameEN = model.FirstClothingNameEN,
                secondClothingNameEN = model.SecondClothingNameEN,
                thirdClothingNameEN = model.ThirdClothingNameEN,
                firstClothingNameUA = model.FirstClothingNameUA,
                secondClothingNameUA = model.SecondClothingNameUA,
                thirdClothingNameUA = model.ThirdClothingNameUA,
                firstClothingImage = model.FirstClothingImage,
                secondClothingImage = model.SecondClothingImage,
                thirdClothingImage = model.ThirdClothingImage
            });
        }
    }
}
