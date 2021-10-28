using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Recommendation
    {
        public int Id { get; set; }

        public int? AppUserId { get; set; }

        public AppUser User { get; set; }

        public int? ClothingId { get; set; }

        public Clothing Clothing { get; set; }

        public DateTime? DateTime { get; set; }


    }
}
