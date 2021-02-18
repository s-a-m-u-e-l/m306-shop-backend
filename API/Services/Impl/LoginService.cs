using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services.Impl
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _config;
        
        public LoginService(IConfiguration config)
        {
            _config = config;
        }
        public Guid GetLoggedInUserId()
        {
            throw new NotImplementedException();
        }

        public bool IsLoggedIn()
        {
            throw new NotImplementedException();
        }

        public string Login(UserEntity user)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("SECRET"));
            var role = user.IsAdmin ? "admin" : "user";
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim(ClaimTypes.Role, role)
                }),

                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public bool IsAdmin()
        {
            throw new NotImplementedException();
        }
    }
}