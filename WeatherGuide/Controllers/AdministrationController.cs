using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WeatherGuide.Controllers
{
    public class AdministrationController : Controller
    {
        [Authorize("IsAdminPolicy")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
