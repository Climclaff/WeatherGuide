using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;

namespace WeatherGuide.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class WebRequestLimitAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; }     
        public int MaxRequestCount { get; set; }

      
        private static MemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions());

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isAdmin = false;
            if (context.HttpContext.Request.HttpContext.User.Identity.IsAuthenticated == true)
            {
                isAdmin = context.HttpContext.Request.HttpContext.User.FindFirst("IsAdmin").Value == "true" ? true : false;
            }
            if (!isAdmin)
            {
                var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
                var memoryCacheKey = $"{Name}-{ipAddress}";
                if (!Cache.TryGetValue(memoryCacheKey, out CachedIP ip))
                {
                    ip = new CachedIP();
                    ip.Value = ipAddress;
                    ip.RequestCount = 1;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(Seconds));

                    Cache.Set(memoryCacheKey, ip, cacheEntryOptions);
                }
                else
                {
                    ip.RequestCount++;
                }
                if (ip.RequestCount > MaxRequestCount)
                {
                    context.Result = new ViewResult
                    {
                        ViewName = "RequestLimit"
                    };
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                }
            }
        }
    }
}
