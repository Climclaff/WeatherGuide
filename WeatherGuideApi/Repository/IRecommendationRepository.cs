using System.Threading.Tasks;

using WeatherGuideApi.api;

namespace WeatherGuideApi.Repository
{
    public interface IRecommendationRepository
    {
        Task<Clothing> GenerateRandomClothing(int minWarmth, int maxWarmth, int moistureResistance, int category);
        Task<int> GenerateRecommendation(Recommendation model);
        Task<Measurement> GetMeasurementForCurrentUser(int userId);
        Task<AppUser> GetUser(int userId);
    }
}