using ApiGateway.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiGateway.Services
{
    // A classe agora implementa corretamente a interface IJwtGenerator
    public class JwtGenerator : IJwtGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // A assinatura do método corresponde EXATAMENTE à interface!
        public string GenerateToken(string userId, string userName, string userRole)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            // Nota: Este código está buscando a SecretKey.
            var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? 
                throw new InvalidOperationException("Chave JWT (SecretKey) não configurada no appsettings.json do Gateway."));

            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Define as Claims (informações dentro do token)
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId), // Identificador do usuário
                new Claim(ClaimTypes.Role, userRole), // Permissão/função do usuário
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // O token expira em 7 dias
                Expires = DateTime.UtcNow.AddDays(7),
                // Assina o token com a chave secreta
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}