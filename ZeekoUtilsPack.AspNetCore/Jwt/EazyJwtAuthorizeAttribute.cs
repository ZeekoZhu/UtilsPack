using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    /// <summary>
    /// Jwt 验证
    /// </summary>
    public class EazyJwtAuthorizeAttribute : AuthorizeAttribute
    {
        public EazyJwtAuthorizeAttribute()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," +
                                    JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
