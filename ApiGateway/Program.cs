using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApiGateway.Services; // Usado para IJwtGenerator e JwtGenerator
using Microsoft.AspNetCore.Mvc; // Necessário para AddControllers

// Este arquivo assume que você tem um arquivo 'ocelot.json' configurado na raiz do projeto

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// 1. Configuração do Ocelot
// --------------------------------------------------------
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// --------------------------------------------------------
// 2. Configuração do JWT (Bearer Authentication)
// --------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Chave JWT não configurada."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Mudar para true em produção
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// --------------------------------------------------------
// 3. Registro de DI e Controllers
// --------------------------------------------------------

// Registra a Injeção de Dependência do Gerador JWT
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// ADICIONA O SUPORTE A CONTROLLERS (ISSO RESOLVE O "NO OPERATIONS DEFINED")
builder.Services.AddControllers(); 

// --------------------------------------------------------
// 4. Configuração do Swagger/OpenAPI
// --------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Gateway API", Version = "v1" });
    
    // Configuração para incluir o botão de Autorização JWT
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT no formato 'Bearer {token}'",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

// --------------------------------------------------------
// 5. Inicialização do App
// --------------------------------------------------------
var app = builder.Build();

// --------------------------------------------------------
// 6. Habilitar Middleware
// --------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");
        // Seus serviços downstream (Estoque, Vendas) devem ser adicionados aqui posteriormente
        c.SwaggerEndpoint("/swagger/vendas/swagger.json", "Serviço de Vendas");
        c.SwaggerEndpoint("/swagger/estoque/swagger.json", "Serviço de Estoque");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapeia os Controllers (IMPORTANTE!)
app.MapControllers(); 

// O Ocelot DEVE ser o último middleware de roteamento a ser executado
await app.UseOcelot(); 

app.Run();