using System;
using System.Security.Claims;
using RimDev.Stuntman.Core;

namespace Impersonation.API
{
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