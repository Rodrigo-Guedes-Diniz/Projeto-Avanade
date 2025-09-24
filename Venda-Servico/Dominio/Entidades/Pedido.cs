using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venda_Servico.Dominio.Entidades
{
    public class Pedido
    {
        public int Id { get; set; }
        public string Produto { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
    }
}