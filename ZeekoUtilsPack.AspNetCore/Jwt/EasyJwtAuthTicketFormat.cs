using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    /// <summary>
    /// user info |> jwt |> store in ticket |> serialize |> data protection |> base64 encode
    /// https://amanagrawal.blog/2017/09/18/jwt-token-authentication-with-cookies-in-asp-net-core/
    /// </summary>
    public class EasyJwtAuthTicketFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly TokenValidationParameters _validationParameters;

        /// <summary>
        /// Create a new instance of the <see cref="EasyJwtAuthTicketFormat"/>
        /// </summary>
        /// <param name="validationParameters">
        /// instance of <see cref="TokenValidationParameters"/> containing the parameters you
        /// configured for your application
        /// </param>
        public EasyJwtAuthTicketFormat(TokenValidationParameters validationParameters)
        {
            _validationParameters = validationParameters ??
                                        throw new ArgumentNullException($"{nameof(validationParameters)} cannot be null");
        }

        /// <summary>
        /// Does the exact opposite of the Protect methods i.e. converts an encrypted string back to
        /// the original <see cref="AuthenticationTicket"/> instance containing the JWT and claims.
        /// </summary>
        /// <param name="protectedText"></param>
        /// <returns></returns>
        public AuthenticationTicket Unprotect(string protectedText)
            => Unprotect(protectedText, null);

        /// <summary>
        /// Does the exact opposite of the Protect methods i.e. converts an encrypted string back to
        /// the original <see cref="AuthenticationTicket"/> instance containing the JWT and claims.
        /// Additionally, optionally pass in a purpose string.
        /// </summary>
        /// <param name="protectedText"></param>
        /// <param name="purpose"></param>
        /// <returns></returns>
        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            try
            {
                // 校验并读取 jwt 中的用户信息（Claims）
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(protectedText, _validationParameters, out var token);

                if (!(token is JwtSecurityToken))
                {
                    throw new SecurityTokenValidationException("JWT token was found to be invalid");
                }
                // todo: 此处还可以校验 token 是否被吊销
                // 将 jwt 中的用户信息与 Cookie 中的包含的用户信息合并起来
                var authTicket = new AuthenticationTicket(principal, CookieAuthenticationDefaults.AuthenticationScheme);
                authTicket.Principal.AddIdentities(principal.Identities);
                return authTicket;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Protect the authentication ticket and convert it to an encrypted string before sending
        /// out to the users.
        /// </summary>
        /// <param name="data">an instance of <see cref="AuthenticationTicket"/></param>
        /// <returns>encrypted string representing the <see cref="AuthenticationTicket"/></returns>
        public string Protect(AuthenticationTicket data) => Protect(data, null);

        /// <summary>
        /// Protect the authentication ticket and convert it to an encrypted string before sending
        /// out to the users. Additionally, specify the purpose of encryption, default is null.
        /// </summary>
        /// <param name="data">an instance of <see cref="AuthenticationTicket"/></param>
        /// <param name="purpose">a purpose string</param>
        /// <returns>encrypted string representing the <see cref="AuthenticationTicket"/></returns>
        public string Protect(AuthenticationTicket data, string purpose)
        {
            var token = data
                .Properties?
                .GetTokenValue(JwtBearerDefaults.AuthenticationScheme);
            return token;
        }
    }
}