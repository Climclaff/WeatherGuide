using System.Net;

namespace WeatherGuide.Attributes
{
    public class CachedIP
    {
        public IPAddress Value { get; set; }

        public int RequestCount { get; set; }
    }
}
