using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Clothing
    {
        public int Id { get; set; }

        public string NameEN { get; set; }

        public string NameUA { get; set; }

        public int Warmth { get; set; }

        public int MoistureResistance { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public byte[] ImageData { get; set; }

        [InverseProperty(nameof(Recommendation.FirstClothing))]
        public ICollection<Recommendation> FirstRecommendation { get; set; }

        [InverseProperty(nameof(Recommendation.SecondClothing))]
        public ICollection<Recommendation> SecondRecommendation { get; set; }

        [InverseProperty(nameof(Recommendation.ThirdClothing))]
        public ICollection<Recommendation> ThirdRecommendation { get; set; }
    }
}
