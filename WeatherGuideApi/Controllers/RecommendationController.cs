using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuideApi.api;
using WeatherGuideApi.Services;

namespace WeatherGuideApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRecommendationService _recommendationService;
        public RecommendationController(UserManager<AppUser> userManager, IConfiguration configuration,
            IRecommendationService recommendationService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _recommendationService = recommendationService;
        }
       [Authorize]
       [HttpGet]
        [Route("Recommendation")]
        public async Task<IActionResult> Recommendation([FromQuery(Name="username")] string username)
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
    public class UserIdModel
    {
        public string userId { get; set; }
    }
}
