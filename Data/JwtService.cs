using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorApp1.Data
{
    public class JwtService(string issuer, string audience, string key)
    {
        private readonly string _issuer = issuer;
        private readonly string _audience = audience;
        private readonly string _key = key;

        public string GenerateToken(string userId, string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // 设置认证类型为 Bearer
            token.Payload["AuthenticationType"] = "Bearer";

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
