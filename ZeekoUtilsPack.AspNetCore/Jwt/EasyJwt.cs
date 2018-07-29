using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public class EasyJwt
    {
        private readonly EasyJwtOption _option;

        public EasyJwt(EasyJwtOption option)
        {
            _option = option;
        }

        public TokenValidationParameters ExportTokenParameters()
        {
            return new TokenValidationParameters()
            {
                IssuerSigningKey = _option.GenerateKey(),
                ValidAudience = _option.Audience,
                ValidIssuer = _option.Issuer,
                ValidateLifetime = true,
                RequireExpirationTime = true
            };
        }

        public string GenerateToken(string userName, IEnumerable<Claim> claims, DateTime expiratoin)
        {
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(userName));
            identity.AddClaims(claims);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Issuer = _option.Issuer,
                Audience = _option.Audience,
                SigningCredentials = _option.GenerateCredentials(),
                Subject = identity,
                Expires = expiratoin
            });
            return token;
        }

        public (ClaimsPrincipal, AuthenticationProperties) GenerateAuthTicket(string userName, IEnumerable<Claim> claims, DateTime expiratoin)
        {
            var principal = new ClaimsPrincipal();
            var authProps = new AuthenticationProperties();
            var token = GenerateToken(userName, claims, expiratoin);
            authProps.StoreTokens(new[]
            {
                new AuthenticationToken
                    {Name = JwtBearerDefaults.AuthenticationScheme ,Value = token}
            });
            return (principal, authProps);
        }
    }

    public abstract class EasyJwtOption
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public bool EnableCookie { get; set; }
        /// <summary>
        /// 自定义 Cookie 选项，可空
        /// </summary>
        public Action<CookieAuthenticationOptions> CookieOptions { get; set; }
        /// <summary>
        /// 自定义 jwt 选项，可空
        /// </summary>
        public Action<JwtBearerOptions> JwtOptions { get; set; }
        public abstract SecurityKey GenerateKey();
        public abstract SigningCredentials GenerateCredentials();
    }

    public class EasyRSAOptions : EasyJwtOption
    {
        public EasyRSAOptions(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path can not be null", nameof(path));
            }

            Path = path;
        }

        public string Path { get; set; }
        public override SecurityKey GenerateKey()
        {
            if (RsaUtils.TryGetKeyParameters(Path, true, out var rsaParams) == false)
            {
                rsaParams = RsaUtils.GenerateAndSaveKey(Path);
            }

            return new RsaSecurityKey(rsaParams);
        }

        public override SigningCredentials GenerateCredentials()
        {
            return new SigningCredentials(GenerateKey(), SecurityAlgorithms.RsaSha256);
        }
    }

    public class EasySymmetricOptions : EasyJwtOption
    {
        public EasySymmetricOptions(string secret)
        {
            Secret = secret ?? throw new ArgumentNullException(nameof(secret));
            Secret = Secret.GetMd5();
        }

        public string Secret { get; set; }
        public override SecurityKey GenerateKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }

        public override SigningCredentials GenerateCredentials()
        {
            return new SigningCredentials(GenerateKey(), SecurityAlgorithms.HmacSha256);
        }
    }
}
