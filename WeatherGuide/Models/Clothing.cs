using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Clothing
    {
        public int Id { get; set; }

        public string NameEN { get; set; }

        public string NameUA { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public byte[] ImageData { get; set; }

        public ICollection<Recommendation> Recommendations { get; set; }
    }
}
