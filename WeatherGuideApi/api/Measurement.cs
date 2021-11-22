using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuideApi.api
{
    public class Measurement
    {
        public int Id { get; set; }

        public int Temperature { get; set; }

        public int Humidity { get; set; }

        public int WindSpeed { get; set; }

        public DateTime? DateTime { get; set; }

        public int? CountryId { get; set; }

        public Country Country { get; set; }

        public int? StateId { get; set; }

        public State State { get; set; }
    }
}
