using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherGuide.Models
{
    public class Claims
    {
        public List<Microsoft.AspNetCore.Identity.IdentityUserClaim<int>> ClaimsList { set; get; }
    }
}
