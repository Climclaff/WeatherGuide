using System.ComponentModel.DataAnnotations;

namespace WeatherGuide.Models.ApiModels
{
    public class ChangeNameModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$",
               ErrorMessage = "The Name field must contain alphabetical characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$",
            ErrorMessage = "The Surname field must contain alphabetical characters")]
        public string Surname { get; set; }
    }
}
