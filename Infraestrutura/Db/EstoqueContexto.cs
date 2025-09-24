using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjetoAvanade.Dominio.Entidades;

namespace ProjetoAvanade.Infraestrutura.Db
{
    public class EstoqueContexto : DbContext
    {
        public EstoqueContexto(DbContextOptions<EstoqueContexto> options) : base(options)
        {

        }

        public DbSet<Produto> Produtos { get; set; }
        
        public DbSet<Usuario> Usuarios { get; set; }
    }
}