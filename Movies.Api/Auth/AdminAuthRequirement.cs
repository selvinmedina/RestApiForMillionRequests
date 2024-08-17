using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Movies.Api.Auth
{
    public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
    {
        private readonly string _apiKey;

        public AdminAuthRequirement(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
            {
                context.Succeed(this);
                return Task.CompletedTask;

            }

            var httpContext = context.Resource as HttpContext;

            if (httpContext is null)
            {
                return Task.CompletedTask;
            }

            if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiKey))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (!apiKey.Equals(_apiKey))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var identity = (ClaimsIdentity)context.User.Identity!;
            identity.AddClaim(new Claim("userid", Guid.NewGuid().ToString()));

            context.Succeed(this);

            return Task.CompletedTask;
        }
    }
}
