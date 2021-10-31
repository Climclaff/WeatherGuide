using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Repository;
using WeatherGuide.Models;
namespace WeatherGuide.Builders.RecommendationService
{
    public class StormyWeatherBuilder : RecommendationBuilder
    {
        public StormyWeatherBuilder(IRecommendationRepository recommendationRepository)
           : base(recommendationRepository) { }
        public override async Task<bool> GenerateFirstItem()
        {
            this.Recommendation.FirstClothing = await _recommendationRepository.GenerateRandomClothing(75, 1);
            return true;
        }
        public override async Task<bool> GenerateSecondItem()
        {
            this.Recommendation.SecondClothing = await _recommendationRepository.GenerateRandomClothing(75, 2);
            return true;
        }
        public override async Task<bool> GenerateThirdItem()
        {
            this.Recommendation.ThirdClothing = await _recommendationRepository.GenerateRandomClothing(75, 3);
            return true;
        }
    }
}
