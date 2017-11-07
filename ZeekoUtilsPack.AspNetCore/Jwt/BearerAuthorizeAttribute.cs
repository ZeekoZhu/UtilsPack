using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    /// <summary>
    /// Jwt 验证
    /// </summary>
    public class BearerAuthorizeAttribute : AuthorizeAttribute
    {
        public BearerAuthorizeAttribute(params int[] roles) : base("Bearer")
        {
        }
    }
}
