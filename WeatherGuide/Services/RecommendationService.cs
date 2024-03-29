﻿using System;
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
        Measurement currentMeasurement = new Measurement();
        public RecommendationService(IRecommendationRepository recommendationRepository)
        {
            _recommendationRepository = recommendationRepository;
        }
        public async Task<Measurement> FindUserMeasurement(AppUser appUser)
        {
            Measurement meas = await _recommendationRepository.GetMeasurementForCurrentUser(appUser.Id);
            return meas;
        }
            public async Task<Recommendation> GetRecommendation(AppUser appUser)
        {
            currentMeasurement = await _recommendationRepository.GetMeasurementForCurrentUser(appUser.Id);
            Recommender recommender = new Recommender();
            RecommendationBuilder builder = WeatherBuilderFactory.createBuilder(_recommendationRepository, currentMeasurement);
            Recommendation recommendation = await recommender.Build(builder);
            recommendation.DateTime = DateTime.UtcNow;
            recommendation.AppUserId = appUser.Id;
            await _recommendationRepository.GenerateRecommendation(recommendation);
            return recommendation;

        }
    }
}
