using Microsoft.Extensions.DependencyInjection;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public static class JwtConfigOptionsExt
    {
        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services, JwtConfigOptions options)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", options.JwtAuthorizationPolicyBuilder.Build());
            });
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            JwtConfigOptions options)
        {
            services.AddAuthentication().AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = options.JwTokenValidationParameters;
            });
            return services;
        }
    }
}