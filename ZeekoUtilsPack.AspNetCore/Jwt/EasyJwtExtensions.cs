using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public static class EasyJwtExtensions
    {
        public static IServiceCollection AddEazyJwt(this IServiceCollection services, EasyJwtOption option)
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
                services.AddDataProtection(dpOptions =>
                {
                    dpOptions.ApplicationDiscriminator = $"app-{option.Issuer}";
                });
                services.AddScoped<IDataSerializer<AuthenticationTicket>, TicketSerializer>();
                
                var tmpProvider = services.BuildServiceProvider();
                var protectionProvider = tmpProvider.GetService<IDataProtectionProvider>();
                var dataProtector = protectionProvider.CreateProtector("jwt-cookie");
                authBuilder.AddCookie(options =>
                {
                    options.TicketDataFormat =
                        new EasyJwtAuthTicketFormat(jwtParams,
                            tmpProvider.GetService<IDataSerializer<AuthenticationTicket>>(),
                            dataProtector);
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
    }
}