using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjetoAvanade.Dominio.DTO;
using ProjetoAvanade.Dominio.Entidades;
using ProjetoAvanade.Infraestrutura.Db;

namespace ProjetoAvanade.Dominio.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EstoqueController : ControllerBase
    {

        private readonly EstoqueContexto _context;

        public EstoqueController(EstoqueContexto context)
        {
            _context = context;
        }

        [HttpPost("cadastrar")]
        public IActionResult Cadastrar(Produto produto)
        {
            _context.Add(produto);
            _context.SaveChanges();
            // return CreatedAction(nameof(ObterPorId), new { id = produto.Id}, produto)
            return Ok(produto);
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var produto = _context.Produtos.Find(id);

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var itens = _context.Produtos.ToList();
            return Ok(itens);
        }

        [HttpGet("ObterPorNome")]
        public IActionResult ObterPorNome(string nome)
        {
            var produtos = _context.Produtos.Where(x => x.Nome.Contains(nome));
            return Ok(produtos);
        }

        [HttpGet("validar/{id}/{quantidade}")]
        public IActionResult ValidarEstoque(int id, int quantidade)
        {
            var produto = _context.Produtos.Find(id);

            if (produto == null)
                return NotFound("Produto não encontrado.");

            if (produto.Quantidade < quantidade)
                return BadRequest("Estoque insuficiente.");

            return Ok(new { mensagem = "Estoque disponível", produto });
        }


        [HttpPut("atualizar")]
        public IActionResult Atualizar(int id, ProdutoDTO dto)
        {
            var produtoBanco = _context.Produtos.Find(id);

            if (produtoBanco == null)
            {
                return NotFound();
            }
            else
            {
                produtoBanco.Nome = dto.Nome;
                produtoBanco.Preco = dto.Preco;
                produtoBanco.Descricao = dto.Descricao;
                produtoBanco.Quantidade = dto.Quantidade;
                _context.SaveChanges();
            }

            return Ok(produtoBanco);
        }

        [HttpDelete("deletar {id}")]
        public IActionResult Excluir(int id)
        {
            var produtos = _context.Produtos.Find(id);

            if (produtos == null)
            {
                return NotFound();
            }
            else
            {
                _context.Produtos.Remove(produtos);
                _context.SaveChanges();

                return NoContent();
            }
        }

        [HttpDelete("baixar-estoque")]
        public IActionResult ExcluirPorNome(string nome)
        {
            var produto = _context.Produtos.FirstOrDefault(x => x.Nome == nome);

            if (produto == null)
            {
                return NotFound("Produto não encontrado.");
            }

            if (produto.Quantidade < 1)
            {
                return BadRequest("Estoque esgotado.");
            }

            produto.Quantidade -= 1;

            _context.Update(produto);
            _context.SaveChanges();

            return Ok(produto);
        }
    }
}