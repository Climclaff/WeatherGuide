using System.ComponentModel.DataAnnotations;

namespace WeatherGuide.Models.ApiModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "{0} is not valid")]
        [Display(Name = "EmailAddress")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress(ErrorMessage = "{0} is not valid")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$",
               ErrorMessage = "The field must contain alphabetical characters")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^(([A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+[\s]{1}[A-za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+)|([A-Za-zА-ЩЬЮЯҐЄІЇа-щьюяґєії]+))$",
            ErrorMessage = "The field must contain alphabetical characters")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int StateId { get; set; }
    }
}
