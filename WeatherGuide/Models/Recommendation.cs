using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Recommendation
    {
        public int Id { get; set; }

        public int? AppUserId { get; set; }

        public AppUser User { get; set; }

        [ForeignKey(nameof(FirstClothing))]
        public int? FirstClothingId { get; set; }

        public Clothing FirstClothing { get; set; }

        [ForeignKey(nameof(SecondClothing))]
        public int? SecondClothingId { get; set; }

        public Clothing SecondClothing { get; set; }

        [ForeignKey(nameof(ThirdClothing))]
        public int? ThirdClothingId { get; set; }

        public Clothing ThirdClothing { get; set; }

        public DateTime? DateTime { get; set; }


    }
}
