using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAvanade.Dominio.DTO
{
    public class ProdutoDTO
    {
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
    }
}