using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Venda_Servico.Dominio.DTO;
using Venda_Servico.Infraestrutura.Services;

namespace Venda_Servico.Dominio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private static readonly List<PedidoDTO> _pedidos = new();
        private readonly RabbitMqService _rabbitMqService;
        private readonly HttpClient _httpClient;

        public PedidoController(RabbitMqService rabbitMqService, IHttpClientFactory httpClientFactory)
        {
            _rabbitMqService = rabbitMqService;
            _httpClient = httpClientFactory.CreateClient();
        }


        [HttpPost("Venda")]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoDTO pedido)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5001/api/estoque/validar/{pedido.ProdutoId}/{pedido.QuantidadeVendida}");

            if (!response.IsSuccessStatusCode)
                return BadRequest("Estoque insuficiente");

            _pedidos.Add(pedido);
            await _rabbitMqService.PublicarMensagem("fila_pedidos", pedido);

            return Ok("Pedido criado com sucesso");
        }

        [HttpGet("ObterVendas")]
        public IActionResult ObterVendas()
        {
            return Ok(_pedidos);
        }
    }
}