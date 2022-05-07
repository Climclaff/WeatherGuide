using System.Threading.Tasks;
using WeatherGuide.Models.Geolocation;

namespace WeatherGuide.Repository
{
    public interface IGeolocationRepository
    {
        Task<bool> DBSupportsLocation(GeoInfoModel model);
        Task ApplyGeolocationToUser(int userId, GeoInfoModel model);

    }
}
