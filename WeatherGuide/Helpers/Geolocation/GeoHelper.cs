using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherGuide.Helpers.Geolocation
{
    public class GeoHelper
    {
        private readonly HttpClient _httpClient;
        public GeoHelper()
        {
            _httpClient = new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            };          
        }


        public async Task<string> GetGeoInfo()
        {
            var ipAddress = await GetIPAdress();
            var response = await _httpClient.GetAsync($"http://api.ipstack.com/" + ipAddress + "?access_key=e8df4857d152697f22c925f8ec49d84b");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return json;
            }
            return null;
        }
        private async Task<string> GetIPAdress()
        {
            var ipAdress = await _httpClient.GetAsync($"http://ipinfo.io/ip");
            if (ipAdress.IsSuccessStatusCode)
            {
                var json = await ipAdress.Content.ReadAsStringAsync();
                return json.ToString();
            }
            return "";
        }
    }
}
