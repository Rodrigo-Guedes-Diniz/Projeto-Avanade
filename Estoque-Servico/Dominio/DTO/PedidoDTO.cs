using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstoqueServico.Dominio.DTO
{
    public class PedidoDTO
    {
        public int ProdutoId { get; set; }
        public int QuantidadeVendida { get; set; }
    }
}