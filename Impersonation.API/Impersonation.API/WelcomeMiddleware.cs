using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RimDev.Stuntman.Core;

namespace Impersonation.API
{
    public class WelcomeApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StuntmanOptions _stuntmanOptions;

        private const string ApiName = "Sample API"; // TODO: Need to be injected in

        public WelcomeApiMiddleware(RequestDelegate next, StuntmanOptions stuntmanOptions)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _stuntmanOptions = stuntmanOptions ?? throw new ArgumentNullException(nameof(stuntmanOptions));
        }

        public Task Invoke(HttpContext httpContext)
        {
            var userName = httpContext.User?.Identity.Name;
            if (string.IsNullOrEmpty(userName))
                userName = "Anonymous User";

            httpContext.Response.ContentType = "text/html";
            httpContext.Response.WriteAsync(
                $@"<!DOCTYPE html>
                   <html>
                      <head>
                         <meta charset=""utf-8"">
                         <title>{ApiName}</title>
                      </head>
                      <body style=""font-family: Segoe UI"">
                        <div><strong>{ApiName}</strong></div>
                        <div>Hello {userName}, welcome. See the API documentation <a href=""/swagger"">here</a></div>");

            httpContext.Response.WriteAsync(_stuntmanOptions.UserPicker(httpContext.User ?? new ClaimsPrincipal()));
            httpContext.Response.WriteAsync(@"</body></html>");

            return Task.FromResult(true);
        }
    }

    public static class WelcomeApiMiddlewareExtensions
    {
        public static IApplicationBuilder UseWelcomeApi(this IApplicationBuilder builder, StuntmanOptions stuntmanOptions)
        {
            return builder.UseMiddleware<WelcomeApiMiddleware>(stuntmanOptions);
        }
    }
}
