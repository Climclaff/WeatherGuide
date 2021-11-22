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
        public async Task<IActionResult> Recommendation([FromBody] UserIdModel model)
        {
            var user = await _userManager.FindByIdAsync(model.userId);
            Measurement measurement = await _recommendationService.FindUserMeasurement(user);
            Recommendation recommendation = await _recommendationService.GetRecommendation(user);
            return Ok(new
            {
                firstClothingName = recommendation.FirstClothing.NameEN,
                secondClothingName = recommendation.SecondClothing.NameEN,
                thirdClothingName = recommendation.ThirdClothing.NameEN,
            });
        }
    }
    public class UserIdModel
    {
        public string userId { get; set; }
    }
}
