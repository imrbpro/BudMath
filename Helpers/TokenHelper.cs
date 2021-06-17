using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BudMath.Helpers
{
    public class TokenHelper
    {
        public static object GenerateToken(IConfiguration config, DateTime expiry, List<Claim> authclaims)
        {
            var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( config["JWT:Secret"]));
            var token = new JwtSecurityToken(
                    issuer: config["JWT:ValidIssuer"],
                    audience: config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authclaims,
                    signingCredentials: new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256)
                    );
            return (new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
        }
    }
}
