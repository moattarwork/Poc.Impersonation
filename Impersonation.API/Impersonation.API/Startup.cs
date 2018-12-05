using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RimDev.Stuntman.Core;

namespace Impersonation.API
{
    public class Startup
    {
        public static readonly StuntmanOptions StuntmanOptions = new StuntmanOptions();

        public Startup(IConfiguration configuration)
        {
            StuntmanOptions
                .AddUser("John", "Doe")
                .AddUser("Mary", "Smith");

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStuntman(StuntmanOptions);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStuntman(StuntmanOptions);
            app.UseMvc();

            app.Map("", nonSecure =>
            {
                nonSecure.Run(context =>
                {
                    var userName = context.User?.Identity.Name;
                    if (string.IsNullOrEmpty(userName))
                        userName = "Anonymous / Unknown";

                    context.Response.ContentType = "text/html";
                    context.Response.WriteAsync(
                        $@"<!DOCTYPE html>
                            <html>
                                <head>
                                    <meta charset=""utf-8"">
                                    <title>Stuntman - UsageSample.AspNetCore</title>
                                </head>
                            <body>Hello, {userName}.");

                    context.Response.WriteAsync(StuntmanOptions.UserPicker(context.User ?? new ClaimsPrincipal()));
                    context.Response.WriteAsync(@"</body></html>");

                    return Task.FromResult(true);
                });
            });
        }


    }

    public static class SecurityExtensions
    {
        public static StuntmanOptions AddUser(this StuntmanOptions options, string name, string surname)
        {
            var user = new StuntmanUser($"{name}.{surname}", $"{name} {surname}", ClaimTypes.Name, ClaimTypes.Role)
                .SetAccessToken(Guid.NewGuid().ToString())
                .AddClaim("given_name", name)
                .AddClaim("family_name", surname)
                .AddClaim("role", "MDA")
                .AddClaim("role", "MPIR");

            options.AddUser(user);
            return options;
        }
    }
}
