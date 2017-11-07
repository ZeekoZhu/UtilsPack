using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public class JwtConfigOptions
    {
        public string KeyStorePath { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public JwtOptions TokenOptions { get; set; }

        public JwtConfigOptions(string keyStorePath, string issuer, string audience)
        {
            KeyStorePath = keyStorePath ?? throw new ArgumentNullException(nameof(keyStorePath));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            if (RsaUtils.TryGetKeyParameters(keyStorePath, true, out RSAParameters keyParams) == false)
            {
                keyParams = RsaUtils.GenerateAndSaveKey(keyStorePath);
            }
            TokenOptions = new JwtOptions
            {
                Key = new RsaSecurityKey(keyParams),
                Issuer = issuer,
                Audience = audience
            };
            TokenOptions.Credentials =
                new SigningCredentials(TokenOptions.Key, SecurityAlgorithms.RsaSha256Signature);
        }



        /// <summary>
        /// 获取默认的 AuthorizationPolicyBuilder
        /// </summary>
        /// <example>
        /// services.AddAuthorization(auth =>
        /// {
        ///     auth.AddPolicy("Bearer", jwtConfigOptions.JwtAuthorizationPolicyBuilder.Build());
        /// }
        /// </example>
        public AuthorizationPolicyBuilder JwtAuthorizationPolicyBuilder
            => new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser();

        /// <summary>
        /// 获取默认的 TokenValidationParameters
        /// </summary>
        /// <example>
        /// services.AddAuthentication().AddJwtBearer(jwtOptions =>
        /// {
        ///     jwtOptions.TokenValidationParameters = options.JwTokenValidationParameters;
        /// }
        /// </example>
        public TokenValidationParameters JwTokenValidationParameters
            => new TokenValidationParameters
            {
                IssuerSigningKey = TokenOptions.Key,
                ValidAudience = TokenOptions.Audience,
                ValidIssuer = TokenOptions.Issuer,
                ValidateLifetime = true,
                RequireExpirationTime = true
            };
    }
}
