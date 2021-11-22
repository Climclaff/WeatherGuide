using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuideApi.Repository;
using WeatherGuideApi.api;
namespace WeatherGuideApi.Builders.RecommendationService
{
    public class WeatherBuilderFactory
    {     
            public static RecommendationBuilder createBuilder(IRecommendationRepository repository,Measurement measurement)
        {           
            if (measurement.Temperature < 17 && measurement.WindSpeed > 25)
            {   
                return new StormyWeatherBuilder(repository);
            }
            if (measurement.Temperature >= 27)
            {
                return new HotWeatherBuilder(repository);
            }
            if (measurement.Temperature <= 0)
            {
                return new FreezingWeatherBuilder(repository);
            }
            if (measurement.Humidity > 70 && measurement.Temperature < 10)
            {
                return new ColdHumidWeatherBuilder(repository);
            }
            if (measurement.Humidity > 70 && measurement.Temperature >= 10)
            {
                return new WarmHumidWeatherBuilder(repository);
            }                     
            if (measurement.Temperature >= 17)
            {
                return new WarmWeatherBuilder(repository);
            }
            return new ShiftyWeatherBuilder(repository);
        }
    }
}
