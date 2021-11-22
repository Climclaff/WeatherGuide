using System.Threading.Tasks;
using WeatherGuideApi.api;

namespace WeatherGuideApi.Services
{
    public interface IRecommendationService
    {
        Task<Measurement> FindUserMeasurement(AppUser appUser);
        Task<Recommendation> GetRecommendation(AppUser appUser);
    }
}