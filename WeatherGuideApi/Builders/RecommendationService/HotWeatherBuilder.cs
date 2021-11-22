﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuideApi.Repository;
using WeatherGuideApi.api;
namespace WeatherGuideApi.Builders.RecommendationService
{
    public class HotWeatherBuilder : RecommendationBuilder
    {
        public HotWeatherBuilder(IRecommendationRepository recommendationRepository)
           : base(recommendationRepository) { }
        public override async Task GenerateFirstItem()
        {
            this.Recommendation.FirstClothing = await _recommendationRepository.GenerateRandomClothing(0, 20, 0, 1);
        }
        public override async Task GenerateSecondItem()
        {
                this.Recommendation.SecondClothing = await _recommendationRepository.GenerateRandomClothing(0,20, 0, 2);
        }
        public override async Task GenerateThirdItem()
        {
            this.Recommendation.ThirdClothing = await _recommendationRepository.GenerateRandomClothing(0, 20, 0, 3);
        }
    }
}
