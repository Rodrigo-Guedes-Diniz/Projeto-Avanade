using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // **NOVO: Para resolver o IConfiguration**
using System.Collections.Generic;
using ApiGateway.Services;

namespace ApiGateway.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtGenerator _jwtGenerator;

        // O construtor agora pede o IConfiguration e o IJwtGenerator (que está no Services)
        public AuthController(IConfiguration configuration, IJwtGenerator jwtGenerator)
        {
            _configuration = configuration;
            _jwtGenerator = jwtGenerator;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // --- 1. Simulação de Autenticação ---
            if (model.Username == "admin" && model.Password == "123")
            {
                // Gera o token (usa a assinatura do método que definimos)
                // Adicionando um ID de usuário (simulado) e o primeiro item da lista de roles.
            var token = _jwtGenerator.GenerateToken(
                userId: "12345", // Argumento 1: userId (Simulado)
                userName: model.Username, // Argumento 2: userName
                userRole: "Administrator" // Argumento 3: userRole (Pegando o mais alto para simplificar)
            );

                return Ok(new { Token = token });
            }

            return Unauthorized(new { Message = "Credenciais inválidas." });
        }

        // Classe simples para receber o modelo de login
        public class LoginModel
        {
            // Propriedades são não-anuláveis no C# moderno (resolvendo o warning CS8618)
            public string Username { get; set; } = string.Empty; 
            public string Password { get; set; } = string.Empty;
        }
    }
    
    // **NOTA:** A interface duplicada que estava aqui FOI REMOVIDA.
}
