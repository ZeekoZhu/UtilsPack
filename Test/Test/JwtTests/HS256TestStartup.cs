using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace Test.JwtTests
{
    public class HS256TestStartup : RS256TestStartup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddEasyJwt(
                new EasySymmetricOptions("test")
                {
                    Audience = "test",
                    Issuer = "test",
                    EnableCookie = true
                });
            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public HS256TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
