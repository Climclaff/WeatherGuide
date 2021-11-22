using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace WeatherGuideApi.api
{
    public class Country
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public ICollection<State> States { get; set; }

        public ICollection<Measurement> Measurements { get; set; }

        public ICollection<AppUser> Users { get; set; }
    }
    
}
