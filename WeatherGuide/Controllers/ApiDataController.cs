using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WeatherGuide.Attributes;
using WeatherGuide.Data;
using WeatherGuide.Models;
using WeatherGuide.Models.ApiModels;
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
        private readonly IMemoryCache _memoryCache;
        public ApiDataController(UserManager<AppUser> userManager,
            ApplicationDbContext context,
            IRecommendationService recommendationService,
            IMemoryCache cache)
        {
            _userManager = userManager;
            _context = context;
            _recommendationService = recommendationService;
            _memoryCache = cache;
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
        [ApiRequestLimit(Name = "Limit Change Location", Seconds = 5, MaxRequestCount = 2)]
        [Route("ChangeLocation")]
        public async Task<IActionResult> ChangeLocation([FromQuery(Name = "countryid")] int countryId, [FromQuery(Name = "stateid")] int stateId)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            var cacheEntry = _memoryCache.Get<Recommendation>(user.Id.ToString() + "recommendation");
            if (cacheEntry != null)
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
        [ApiRequestLimit(Name = "Limit Change Name", Seconds = 5, MaxRequestCount = 2)]
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
        [ApiRequestLimit(Name = "Limit Change Password", Seconds = 5, MaxRequestCount = 2)]
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
        [ApiRequestLimit(Name = "Limit Get Recommendation", Seconds = 10, MaxRequestCount = 1)]
        [HttpGet]
        [Route("Recommendation")]
        public async Task<IActionResult> Recommendation()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userManager.FindByNameAsync(username);

            if (!_memoryCache.TryGetValue(user.Id.ToString() + "recommendation", out Recommendation cacheRecommendation))
            {
                cacheRecommendation = await _recommendationService.GetRecommendation(user);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(900));

                _memoryCache.Set(user.Id.ToString() + "recommendation", cacheRecommendation, cacheEntryOptions);
            }
            Recommendation recommendation = cacheRecommendation;


            if (!_memoryCache.TryGetValue(user.Id.ToString() + "measurement", out Measurement cacheMeasurement))
            {
                cacheMeasurement = await _recommendationService.FindUserMeasurement(user);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(900));

                _memoryCache.Set(user.Id.ToString() + "measurement", cacheMeasurement, cacheEntryOptions);
            }
            Measurement measurement = cacheMeasurement;

            if (user == null || measurement == null)
            {
                return BadRequest();
            }
            return Ok(new
            {
                measurement = measurement,
                firstClothingNameEN = recommendation.FirstClothing.NameEN,
                secondClothingNameEN = recommendation.SecondClothing.NameEN,
                thirdClothingNameEN = recommendation.ThirdClothing.NameEN,
                firstClothingNameUA = recommendation.FirstClothing.NameUA,
                secondClothingNameUA = recommendation.SecondClothing.NameUA,
                thirdClothingNameUA = recommendation.ThirdClothing.NameUA,
                firstClothingImage = recommendation.FirstClothing.ImageData,
                secondClothingImage = recommendation.SecondClothing.ImageData,
                thirdClothingImage = recommendation.ThirdClothing.ImageData
            });
        }
    }
}
