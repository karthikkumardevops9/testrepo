using Leadtools.ImageProcessing.SpecialEffects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MSRecordsEngine.Services
{
    public class CommonControllersService<T>
    {
        public ILogger<T> Logger { get; }
        public IConfiguration Config { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public Microservices Microservices;

        public CommonControllersService(ILogger<T> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor, Microservices microservices)
        {
            Logger = logger;
            Config = config;
            HttpContextAccessor = httpContextAccessor;
            Microservices = microservices;
        }

        public string GetClientIpAddress()
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null) return "Unable to determine client IP address.";

            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unable to determine client IP address.";
        }
    }
}
