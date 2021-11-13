using System.Threading.Tasks;
using WeatherGuide.Models;

namespace WeatherGuide.Services
{
    public interface IRecommendationService
    {
        Task<Measurement> FindUserMeasurement(AppUser appUser);
        Task<Recommendation> GetRecommendation(AppUser appUser);
    }
}