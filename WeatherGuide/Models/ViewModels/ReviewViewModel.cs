using System.Collections.Generic;

namespace WeatherGuide.Models.ViewModels
{
    public class ReviewViewModel
    {
        public IEnumerable<Review> Reviews { get; set; }
        public Review Review { get; set; }
    }
}
