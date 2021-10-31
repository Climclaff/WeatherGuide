using System.Threading.Tasks;
using WeatherGuide.Models;

namespace WeatherGuide.Repository
{
    public interface IRecommendationRepository
    {
        Task<Clothing> GenerateRandomClothing(int warmth, int category);
        Task<int> GenerateRecommendation(Recommendation model);
        Task<Measurement> GetMeasurementForCurrentUser(int userId);
        Task<AppUser> GetUser(int userId);
    }
}