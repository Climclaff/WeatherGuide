using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherGuide.Builders.RecommendationService;
using WeatherGuide.Models;
using WeatherGuide.Repository;
namespace WeatherGuide.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IRecommendationRepository _recommendationRepository = null;
        StringBuilder recommendationStringBuilder = new StringBuilder(255);
        AppUser currentUser = new AppUser();
        Measurement currentMeasurement = new Measurement();
        public RecommendationService(IRecommendationRepository recommendationRepository)
        {
            _recommendationRepository = recommendationRepository;
        }
        public async Task<Recommendation> GetRecommendation(AppUser appUser)
        {
            currentUser = appUser;
            currentMeasurement = await _recommendationRepository.GetMeasurementForCurrentUser(currentUser.Id);
            Recommender recommender = new Recommender();
            RecommendationBuilder builder = WeatherBuilderFactory.createBuilder(_recommendationRepository, currentMeasurement);
            Recommendation recommendation = await recommender.Build(builder);
            recommendation.DateTime = DateTime.UtcNow;
            recommendation.AppUserId = currentUser.Id;
            await _recommendationRepository.GenerateRecommendation(recommendation);
            recommendationStringBuilder.Append(recommendation.FirstClothing.NameEN+" ")
                .Append(recommendation.SecondClothing.NameEN + " ")
                .Append(recommendation.ThirdClothing.NameEN); 
            return recommendation;

        }
    }
}
