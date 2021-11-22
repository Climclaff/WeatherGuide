using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuideApi.api.ActionModels
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
