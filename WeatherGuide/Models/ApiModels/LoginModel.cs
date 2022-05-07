using System.ComponentModel.DataAnnotations;

namespace WeatherGuide.Models.ApiModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
