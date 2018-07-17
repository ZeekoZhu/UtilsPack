using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ZeekoUtilsPack.AspNetCore.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.JwtTests.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly EasyJwt _jwt;

        public AccountController(EasyJwt jwt)
        {
            _jwt = jwt;
        }
        /// <summary>
        /// 演示性登录 API，返回新的 token 并设置 Cookie
        /// </summary>
        /// <param name="user"></param>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("application/json")]
        public async Task<IActionResult> Post([FromForm]string user)
        {
            // 假的用户信息
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user, ClaimValueTypes.String)
            };
            var token = _jwt.GenerateToken(user, claims, DateTime.Now.AddDays(1));
            var (principal, authProps) = _jwt.GenerateAuthTicket(user, claims, DateTime.Now.AddMinutes(30));
            await HttpContext.SignInAsync(principal, authProps);
            return Json(new {Token = token});
        }
    }
}
