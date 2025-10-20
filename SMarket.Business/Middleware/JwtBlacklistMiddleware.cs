using Microsoft.AspNetCore.Http;
using SMarket.Business.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace SMarket.Business.Middleware
{
    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITokenBlacklistService blacklist)
        {
            string? token = null;

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }
            else if (context.Request.Cookies.ContainsKey("access_token"))
            {
                token = context.Request.Cookies["access_token"];
            }

            if (!string.IsNullOrEmpty(token) && blacklist.IsBlacklisted(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has been revoked");
                return;
            }

            await _next(context);
        }
    }
}

