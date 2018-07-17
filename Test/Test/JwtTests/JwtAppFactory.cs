using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace Test.JwtTests
{
    public class JwtAppFactory<T> : WebApplicationFactory<T> where T : class
    {
        public EasyJwt EasyJwt { get; set; }

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseStartup<T>();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var provider = services.BuildServiceProvider();
                EasyJwt = provider.GetService<EasyJwt>();
            });
        }
    }
}
