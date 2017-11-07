using Microsoft.IdentityModel.Tokens;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public RsaSecurityKey Key { get; set; }
        public SigningCredentials Credentials { get; set; }
        public string Issuer { get; set; }
    }
}
