using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Data;
using WeatherGuide.Models;
namespace WeatherGuide.Repository
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ApplicationDbContext _context = null;
        public RecommendationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<AppUser> GetUser(int userId)
        {
            var currentUser = await (from user in _context.Users
                                     where (user.Id == userId)
                                     select user).SingleOrDefaultAsync();
            return currentUser;
        }
        public async Task<Clothing> GenerateRandomClothing(int warmth, int category)
        {
            var clothingList = await
            _context
                .Set<Clothing>()
                .Where(x => x.Warmth >= warmth)
                .Where(x => x.CategoryId == category)
                .Take(3).ToListAsync();

            var rand = new Random();
            var clothing = clothingList.ElementAt(rand.Next(clothingList.Count()));
            return clothing;
        }
        public async Task<Measurement> GetMeasurementForCurrentUser(int userId)
        {
            AppUser currentUser = await _context.Users.FindAsync(userId);
            return await (from meas in _context.Measurements
                                         where (meas.CountryId == currentUser.CountryId && meas.StateId == currentUser.StateId)
                                         orderby meas.DateTime descending
                                         select meas).FirstOrDefaultAsync();
        }

        public async Task<int> GenerateRecommendation(Recommendation model)
        {
            var generatedRecommendation = new Recommendation()
            {
                DateTime = DateTime.UtcNow,
                FirstClothingId = model.FirstClothing.Id,
                SecondClothingId = model.SecondClothing.Id,
                ThirdClothingId = model.ThirdClothing.Id,
                AppUserId = model.AppUserId
            };
            await _context.Recommendations.AddAsync(generatedRecommendation);
            await _context.SaveChangesAsync();

            return generatedRecommendation.Id;
        }
    }
}
