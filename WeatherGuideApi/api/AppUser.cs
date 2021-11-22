using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace WeatherGuideApi.api
{
    public class AppUser : IdentityUser<int>
    {  
        public int? CountryId { get; set; }

        public Country Country { get; set; }

        public int? StateId { get; set; }

        public State State { get; set; }

        public ICollection<Recommendation> Recommendations { get; set; }


    }
}
