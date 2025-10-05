using Venda_Servico.Infraestrutura.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.OpenApi.Models; // Necessário para a configuração do Swagger/JWT

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Configuração do JWT (Bearer Authentication)
// ----------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
// Atenção: Use a mesma chave que você definiu no APIGateway
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Chave JWT não configurada."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

// ----------------------------------------------------
// Serviços do Projeto
// ----------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Seu serviço RabbitMQ e HttpClient
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHttpClient();


// ----------------------------------------------------
// Configuração do SWAGGER/OPENAPI
// ----------------------------------------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vendas Service API", Version = "v1" });

    // 1. Define o esquema de segurança (Bearer Token)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Insira o token JWT no formato 'Bearer {token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    // 2. Aplica o requisito de segurança a todos os endpoints
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});


var app = builder.Build();

// ----------------------------------------------------
// Pipeline de Requisições
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vendas Service API V1");
    });
}

app.UseHttpsRedirection();

// Ordem correta para segurança
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
