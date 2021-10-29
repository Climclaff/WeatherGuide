using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Sensor
    {
        public int Id { get; set; }

        public int? CountryId { get; set; }

        public Country Country { get; set; }

        public int? StateId { get; set; }

        public State State { get; set; }

    }
}
