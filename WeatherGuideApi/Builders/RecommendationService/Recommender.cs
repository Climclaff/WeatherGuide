﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuideApi.Repository;
using WeatherGuideApi.api;
namespace WeatherGuideApi.Builders.RecommendationService
{
    public class Recommender
    {
        public async Task<Recommendation> Build(RecommendationBuilder recommendationBuilder)
        {
            recommendationBuilder.CreateRecommendation();
            await recommendationBuilder.GenerateFirstItem();
            await recommendationBuilder.GenerateSecondItem();
            await recommendationBuilder .GenerateThirdItem();
            return recommendationBuilder.Recommendation;
        }
    }
}
