using System;

namespace WeatherGuide.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public int? AppUserId { get; set; }

        public AppUser User { get; set; }

        public DateTime? DateTime { get; set; }
    }
}
