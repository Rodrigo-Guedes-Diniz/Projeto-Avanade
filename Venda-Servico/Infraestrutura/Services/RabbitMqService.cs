using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Venda_Servico.Infraestrutura.Services
{
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RABBITMQ__HOST") ?? "localhost",
                Port = 5672,
                UserName = Environment.GetEnvironmentVariable("RABBITMQ__USERNAME") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RABBITMQ__PASSWORD") ?? "guest"
            };
        }

        public async Task PublicarMensagem<T>(string fila, T mensagem)
        {
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "fila_pedidos",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

            var json = JsonSerializer.Serialize(mensagem);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "fila_pedidos",
                mandatory: false,
                body: Encoding.UTF8.GetBytes(json)
            );
        }
    }
}