using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public static class EasyJwtExtensions
    {
        public static IServiceCollection AddEasyJwt(this IServiceCollection services, EasyJwtOption option)
        {
            var easyJwt = new EasyJwt(option);
            var jwtParams = easyJwt.ExportTokenParameters();
            services.AddDataProtection();
            services.AddSingleton(easyJwt);

            var authBuilder = services.AddAuthentication(authOptions =>
                {
                    authOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Audience = option.Audience;
                    jwtOptions.ClaimsIssuer = option.Issuer;
                    jwtOptions.TokenValidationParameters = jwtParams;
                    option.JwtOptions?.Invoke(jwtOptions);
                });
            if (option.EnableCookie)
            {
                authBuilder.AddCookie(options =>
                {
                    options.TicketDataFormat =
                        new EasyJwtAuthTicketFormat(jwtParams);
                    options.ClaimsIssuer = option.Issuer;
                    options.LoginPath = "/Login";
                    options.AccessDeniedPath = "/Login";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "tk";
                    option.CookieOptions?.Invoke(options);
                });
            }

            return services;
        }

        public static async Task SignInAsync(this HttpContext context, string userName, IEnumerable<Claim> claims,
            DateTime expiratoin)
        {
            var jwt = context.RequestServices.GetService<EasyJwt>();
            var (principal, authProps) = jwt.GenerateAuthTicket(userName, claims, expiratoin);
            await context.SignInAsync(principal, authProps);
        }
    }
}