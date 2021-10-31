﻿using System;
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
        public override async Task GenerateFirstItem()
        {
            this.Recommendation.FirstClothing = await _recommendationRepository.GenerateRandomClothing(75,75, 1);
        }
        public override async Task GenerateSecondItem()
        {
            this.Recommendation.SecondClothing = await _recommendationRepository.GenerateRandomClothing(75,75, 2);
        }
        public override async Task GenerateThirdItem()
        {
            this.Recommendation.ThirdClothing = await _recommendationRepository.GenerateRandomClothing(75,75, 3);
        }
    }
}
