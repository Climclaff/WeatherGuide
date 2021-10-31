using System.Threading.Tasks;
using WeatherGuide.Models;

namespace WeatherGuide.Services
{
    public interface IRecommendationService
    {
        Task<string> GetRecommendation(AppUser appUser);
    }
}