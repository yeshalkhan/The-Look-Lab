using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthorizationController(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"] ?? "";
            _issuer = configuration["Jwt:Issuer"] ?? "";
            _audience = configuration["Jwt:Audience"] ?? "";
        }

        // POST: api/authorization/token
        [HttpPost("token")]
        public async Task<IActionResult> GenerateToken([FromForm] LoginRequest loginRequest)
        {
            if (loginRequest.Username == "admin@thelooklab.com" && loginRequest.Password == "Admin123#")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_key);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, loginRequest.Username), new Claim(ClaimTypes.Role, "Admin")]),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                    Issuer = _issuer,
                    Audience = _audience
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new { Token = tokenHandler.WriteToken(token) });
            }
            return Unauthorized();
        }

        // Request Model for Login
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }

}
