using Newtonsoft.Json;

namespace WeatherGuide.Models.Geolocation
{
    public class GeoInfoModel
    {
        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region_name")]
        public string Region { get; set; }


    }
}
