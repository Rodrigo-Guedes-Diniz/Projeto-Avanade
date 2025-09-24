using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venda_Servico.Dominio.DTO
{
    public class PedidoDTO
    {
        public string Produto { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
    }
}