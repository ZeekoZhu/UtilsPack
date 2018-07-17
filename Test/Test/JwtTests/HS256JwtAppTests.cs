using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Test.JwtTests
{
    public class HS256JwtAppTests : IClassFixture<JwtAppFactory<HS256TestStartup>>
    {
        private readonly JwtAppFactory<HS256TestStartup> _factory;

        public HS256JwtAppTests(JwtAppFactory<HS256TestStartup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/values")]
        [InlineData("/api/values/123")]
        public async Task GetTest(string url)
        {
            var client = _factory.CreateClient();

            var resp = await client.GetAsync(url);

            resp.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task PostDenied()
        {
            var client = _factory.CreateClient();

            var resp = await client.PostAsync("/api/values", new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("value","value")
            }));

            resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task FakeLogin()
        {
            var loginClient = _factory.CreateClient();
            var content = new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("user","test")
            });

            // 获取 token
            var resp = await loginClient.PostAsync("/api/account", content);

            // assert
            resp.IsSuccessStatusCode.Should().BeTrue();
            var respBody = await resp.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeAnonymousType(respBody, new {Token = ""})?.Token;
            token.Should().NotBeNullOrEmpty();
            var cookieTokenResp =  await loginClient.PostAsync("/api/values", new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("value","value")
            }));
            cookieTokenResp.IsSuccessStatusCode.Should().BeTrue();

            var apiClient = _factory.CreateClient();
            apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiTokenResp = await apiClient.PostAsync("/api/values", new FormUrlEncodedContent(new[]
            {
                KeyValuePair.Create("value","value")
            }));
            apiTokenResp.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}
