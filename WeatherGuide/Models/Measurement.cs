using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Measurement
    {
        public int Id { get; set; }

        public int Temperature { get; set; }

        public int Humidity { get; set; }

        public int WindSpeed { get; set; }

        public DateTime? DateTime { get; set; } 

        public int SensorId { get; set; }

        public Sensor Sensor { get; set; }
    }
}
