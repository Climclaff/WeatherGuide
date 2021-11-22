using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuideApi.Repository;
using WeatherGuideApi.api;
namespace WeatherGuideApi.Builders.RecommendationService
{
    public abstract class RecommendationBuilder
    {
        protected readonly IRecommendationRepository _recommendationRepository = null;
        public RecommendationBuilder(IRecommendationRepository recommendationRepository)
        {
            _recommendationRepository = recommendationRepository;
        }
        public Recommendation Recommendation { get; private set; }
        public void CreateRecommendation()
        {
            Recommendation = new Recommendation();
        }
        public abstract Task GenerateFirstItem();
        public abstract Task GenerateSecondItem();
        public abstract Task GenerateThirdItem();

    }
}
