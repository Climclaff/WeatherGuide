using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace WeatherGuide.Models
{
    public class State
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? CountryId { get; set; }

        public Country Country { get; set; }

        public ICollection<Sensor> Sensors { get; set; }

        public ICollection<AppUser> Users { get; set; }
    }
}
