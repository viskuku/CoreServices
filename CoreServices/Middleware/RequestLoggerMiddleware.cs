using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoreServices.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger logger;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next;
            //logger = loggerFactory.CreateLogger<LoggerMiddleware>();
            this.logger = logger;

        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            //Read body from the request and log it
            using (var reader = new StreamReader(httpContext.Request.Body))
            {
                var requestbody = reader.ReadToEnd();
                //As this is a middleware below line will make sure it will log each and every request body
                logger.LogInformation(requestbody);

            }
            //Move to next delegate/middleware in the pipleline
            return _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}
