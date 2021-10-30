using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Data;
using WeatherGuide.Models;
using WeatherGuide.Repository;
namespace WeatherGuide.Models.domain
{
    public class Recommender
    {
        private readonly IRecommendationRepository _recommendationRepository = null;       
        Recommendation recommendation = new Recommendation();
        Measurement currentMeasurement = new Measurement();
        AppUser currentUser = new AppUser();
        public Recommender(IRecommendationRepository recommendationRepository, AppUser appUser)
        {
            _recommendationRepository = recommendationRepository;
            currentUser = appUser;
            currentMeasurement = _recommendationRepository.GetMeasurementForCurrentUser(appUser.Id);
        }
        public string Recommend()
        {
            if (currentMeasurement.Temperature < 10)
            {
                recommendation.FirstClothingId = _recommendationRepository.GenerateRandomClothing(60, 1);
                recommendation.SecondClothingId = _recommendationRepository.GenerateRandomClothing(60, 2);
                recommendation.ThirdClothingId = _recommendationRepository.GenerateRandomClothing(60, 3);
                recommendation.DateTime = DateTime.UtcNow;
                recommendation.AppUserId = currentUser.Id;
                _recommendationRepository.GenerateRecommendation(recommendation);
                return "You should wear " + recommendation.FirstClothingId;
            }
            return "You should wear " + recommendation.FirstClothingId;
        }

    }
}
