using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace ZeekoUtilsPack.EFKata
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddEfKata<T>(this IServiceCollection services) where T : DbContext
        {
            services.AddScoped(provider =>
            {
                var dbContext = provider.GetRequiredService<T>();
                return new EfKataContext(dbContext.Model);
            });
            return services;
        }
    }
}