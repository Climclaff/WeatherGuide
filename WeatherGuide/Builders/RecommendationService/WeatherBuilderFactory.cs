using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Repository;
using WeatherGuide.Models;
namespace WeatherGuide.Builders.RecommendationService
{
    public class WeatherBuilderFactory
    {     
            public static RecommendationBuilder createBuilder(IRecommendationRepository repository,Measurement measurement)
        {

            if (measurement.Temperature < -10 && measurement.WindSpeed > 15)
            {
                return new FreezingWeatherBuilder(repository);
            }
            if (measurement.Temperature < 0 && measurement.WindSpeed > 25 && measurement.Humidity > 40)
            {
                return new StormyWeatherBuilder(repository);
            }
            return new FreezingWeatherBuilder(repository);
        }
    }
}
