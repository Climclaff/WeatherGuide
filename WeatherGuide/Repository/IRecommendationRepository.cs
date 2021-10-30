using System.Threading.Tasks;
using WeatherGuide.Models;

namespace WeatherGuide.Repository
{
    public interface IRecommendationRepository
    {
        int GenerateRandomClothing(int warmth, int category);
        Task<int> GenerateRecommendation(Recommendation model);
        Measurement GetMeasurementForCurrentUser(int userId);
        AppUser GetUser(int userId);
    }
}