using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;

namespace ApiGateway.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtGenerator _jwtGenerator;

        public AuthController(IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model.Username == "admin" && model.Password == "123")
            {
                var token = _jwtGenerator.GenerateToken(
                    userId: "12345",
                    userName: model.Username,
                    userRole: "Administrator"
                );

                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Credenciais inválidas." });
        }

        public class LoginModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
