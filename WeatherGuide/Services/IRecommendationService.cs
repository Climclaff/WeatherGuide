using System.Threading.Tasks;
using WeatherGuide.Models;

namespace WeatherGuide.Services
{
    public interface IRecommendationService
    {
        Task<Recommendation> GetRecommendation(AppUser appUser);
    }
}