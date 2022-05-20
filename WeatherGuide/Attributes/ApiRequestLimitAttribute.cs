using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Threading.Tasks;
using WeatherGuide.Helpers;

namespace WeatherGuide.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiRequestLimitAttribute : ActionFilterAttribute
    {
        public string Name { get; set; }
        public int Seconds { get; set; }
        public int MaxRequestCount { get; set; }


        public override async Task OnActionExecutionAsync(ActionExecutingContext context,
                                         ActionExecutionDelegate next)
        {
                bool error = false;
                IDistributedCache cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                       .SetAbsoluteExpiration(DateTime.Now.AddSeconds(Seconds));

                var ipAddress = context.HttpContext.Request.HttpContext.Connection.Id;
                var memoryCacheKey = $"{Name}-{ipAddress}";
                var distCacheVal = await cache.GetAsync(memoryCacheKey);
                CachedIP ip;
                if (distCacheVal == null)
                {
                    ip = new CachedIP();
                    ip.Value = ipAddress.ToString();
                    ip.RequestCount = 1;
                    await cache.SetAsync(memoryCacheKey, ip.ToByteArray(), cacheEntryOptions);
                }
                else
                {
                    ip = distCacheVal.FromByteArray<CachedIP>();
                    if (ip.RequestCount > MaxRequestCount)
                    {
                        error = true;
                        context.Result = new ContentResult
                        {
                            Content = "Too many requests."
                        };
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    }

                }
                if (!error)
                {
                    ip.RequestCount++;
                    await cache.SetAsync(memoryCacheKey, ip.ToByteArray(), cacheEntryOptions);
                    await next();
                }
            }
        }

        /*
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            {
                var ipAddress = context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
                var memoryCacheKey = $"{Name}-{ipAddress}";
                if (!Cache.TryGetValue(memoryCacheKey, out CachedIP ip))
                {
                    ip = new CachedIP();
                    ip.Value = ipAddress.ToString();
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
                    context.Result = new ContentResult
                    {
                        Content = "Too many requests."
                    };
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                }
            }
        }*/
    } 

