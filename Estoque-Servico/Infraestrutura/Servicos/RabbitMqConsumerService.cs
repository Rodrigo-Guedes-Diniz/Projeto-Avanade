using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EstoqueServico.Dominio.DTO;
using Microsoft.EntityFrameworkCore;
using ProjetoAvanade.Infraestrutura.Db;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EstoqueServico.Infraestrutura.Servicos
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConnectionFactory _factory;

        public RabbitMqConsumerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var connection = await _factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("fila_pedidos", durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var venda = JsonSerializer.Deserialize<VendaMessageDTO>(message);

                    if (venda != null)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<EstoqueContexto>();

                            var produto = db.Produtos.FirstOrDefault(p => p.Id == venda.ProdutoId);
                            if (produto != null)
                            {
                                produto.Quantidade -= venda.Quantidade;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
                }

                await channel.BasicAckAsync(args.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync("fila_pedidos", autoAck: false, consumer: consumer);

            await Task.Delay(-1, stoppingToken);
        }
    }
}