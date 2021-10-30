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
        public AppUser GetUser(int userId)
        {
            var currentUser = (from user in _context.Users
                                  where (user.Id == userId)
                                  select user).SingleOrDefault();
            return currentUser;
        }
        public int GenerateRandomClothing(int warmth, int category)
        {
            var clothingList =
            _context
                .Set<Clothing>()
                .Where(x => x.Warmth >= warmth)
                .Where(x => x.CategoryId == category)
                .Take(3).ToList();

            var rand = new Random();
            var clothing = clothingList.ElementAt(rand.Next(clothingList.Count()));
            return clothing.Id;
        }
        public Measurement GetMeasurementForCurrentUser(int userId)
        {
            AppUser currentUser = (from user in _context.Users
                                           where (user.Id == userId)
                                           select user).SingleOrDefault();
            var measurementList =
                from meas in _context.Measurements
                where (meas.CountryId == currentUser.CountryId && meas.StateId == currentUser.StateId)
                orderby meas.DateTime descending
                select meas;
            Measurement measurement = measurementList.First();
            return measurement;
        }

        public async Task<int> GenerateRecommendation(Recommendation model)
        {
            var generatedRecommendation = new Recommendation()
            {
                DateTime = DateTime.UtcNow,
                FirstClothingId = model.FirstClothingId,
                SecondClothingId = model.SecondClothingId,
                ThirdClothingId = model.ThirdClothingId,
                AppUserId = model.AppUserId
            };
            await _context.Recommendations.AddAsync(generatedRecommendation);
            await _context.SaveChangesAsync();

            return generatedRecommendation.Id;
        }
    }
}
