using System;
using System.Net;

namespace WeatherGuide.Attributes
{
    [Serializable]
    public class CachedIP 
    {
        public string Value { get; set; }

        public int RequestCount { get; set; }

    }
}
