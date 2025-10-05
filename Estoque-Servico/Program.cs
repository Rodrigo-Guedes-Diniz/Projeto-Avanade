using Microsoft.EntityFrameworkCore;
using ProjetoAvanade.Infraestrutura.Db;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Estas são necessárias para BackgroundService:
using Microsoft.Extensions.Hosting; 
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddDbContext<EstoqueContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHostedService<EstoqueServico.Infraestrutura.Servicos.RabbitMqConsumerService>();

// Deve vir depoi de todos os serviços declarados
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
