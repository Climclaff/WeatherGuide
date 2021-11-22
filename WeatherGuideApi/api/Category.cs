using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuideApi.api
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Clothing> Clothings { get; set; }
    }
}
