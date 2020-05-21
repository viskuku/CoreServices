using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreServices.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CoreServices.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ClientConfigurationMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientConfigurationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IClientConfiguration clientConfiguration)
        {
            if (httpContext.Request.Headers.TryGetValue("CLIENTNAME", out StringValues clientName))
            {
                clientConfiguration.ClientName = clientName.SingleOrDefault();
            }
            else
            {
                //Here you can throw exception to force client to send the header
            }

            //Move to next delegate/middleware in the pipleline
            await _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ClientConfigurationMiddlewareExtensions
    {
        public static IApplicationBuilder UseClientConfigurationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientConfigurationMiddleware>();
        }
    }
}
