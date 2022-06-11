using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Data;
using WeatherGuide.Models;
using WeatherGuide.Models.Geolocation;

namespace WeatherGuide.Repository
{
    public class GeolocationRepository : IGeolocationRepository
    {
        private readonly ApplicationDbContext _context = null;
        public GeolocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> DBSupportsLocation(GeoInfoModel model)
        {
            var country = await CheckCountryByName(model.CountryName);
            if (country != null)
            {
                var region = await CheckStateByName(model.Region);
                var city = await CheckStateByName(model.City);
                if (region != null || city != null)
                {
                    return true;
                }             
            }
            return false;
        }
        public async Task ApplyGeolocationToUser(int userId, GeoInfoModel model)
        {
            AppUser currentUser = await _context.Users.FindAsync(userId);
            var country = await CheckCountryByName(model.CountryName);
            var state = await CheckStateByName(model.Region);
            var city = await CheckStateByName(model.City);
            if (country != null)
            {
                currentUser.CountryId = country.Id;
                if (state != null)
                {                 
                    currentUser.StateId = state.Id;                  
                }
                if (city != null)
                {
                    currentUser.StateId = city.Id;
                }
                _context.Update(currentUser);
                await _context.SaveChangesAsync();
            }
        }
        private async Task<Country> CheckCountryByName(string countryName)
        {
            var country = await
           _context
               .Set<Country>()
               .Where(x => x.Name == countryName)
               .SingleOrDefaultAsync();
            return country;
        }
        private async Task<State> CheckStateByName(string stateName)
        {
            var state = await
            _context
               .Set<State>()
               .Where(x => x.Name == stateName  )
               .SingleOrDefaultAsync();

            return state;

        }
    }
}
