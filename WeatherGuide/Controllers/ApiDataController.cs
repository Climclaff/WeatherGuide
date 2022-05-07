using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public ApiDataController(UserManager<AppUser> userManager,
            ApplicationDbContext context,
            IRecommendationService recommendationService)
        {
            _userManager = userManager;
            _context = context;
            _recommendationService = recommendationService;
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

            LoginModel model = new LoginModel();
            model.UserName = "admin@gmail.com";
            model.Password = "121212qqQQ_";

            return Ok(new
            {
                data = countries,
                model = model
            }); ;

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
        public async Task<IActionResult> ProfileCountry([FromQuery(Name = "username")] string username)
        {
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
        public async Task<IActionResult> ProfileState([FromQuery(Name = "username")] string username)
        {
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
        public async Task<IActionResult> UserInfo([FromQuery(Name = "username")] string username)
        {
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
        [Route("ChangeLocation")]
        public async Task<IActionResult> ChangeLocation([FromQuery(Name = "username")] string username,
            [FromQuery(Name = "countryid")] int countryId, [FromQuery(Name = "stateid")] int stateId)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest();
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
        [Route("ChangeName")]
        public async Task<IActionResult> ChangeName([FromQuery(Name = "username")] string username, [FromBody] ChangeNameModel model)
        {
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
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromQuery(Name = "username")] string username, [FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

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
        [HttpGet]
        [Route("Recommendation")]
        public async Task<IActionResult> Recommendation([FromQuery(Name = "username")] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            Measurement measurement = await _recommendationService.FindUserMeasurement(user);
            if (user == null || measurement == null)
            {
                return BadRequest();
            }
            Recommendation recommendation = await _recommendationService.GetRecommendation(user);
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
