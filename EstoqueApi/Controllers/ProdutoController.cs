using EstoqueApi.Data;
using EstoqueApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstoqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoContext _context;

        public ProdutoController(ProdutoContext context)
        {
            _context = context;
        }
        // consultando todos os produtos e suas respectivas categorias
        // GET: api/Produto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProduto()
        {
            var produtos = await _context.Produto.Include(a => a.Categoria)
                .Where(b => b.ativo == true).OrderByDescending(e => e.ID).ToListAsync();

            return produtos;
        }

        //Consulta quantidade de produto em estoque
        [HttpGet("Quantidade-Estoque")]

        public async Task<ActionResult<int>> GetEstoque()
        {

            var estoque = _context.Produto.Where(c => c.ativo == true).Sum(a => a.quantidade);

            return estoque;
        }

        //Consulta do Produto por ID
        // GET: api/Produto/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProdutoPorCodigo(int id)
        {
            var produto = await _context.Produto.Where(c => c.ID == id).FirstOrDefaultAsync();

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        //Edita o Produto e a categoria por ID
        // PUT: api/Produto/1
        [HttpPut]
        public async Task<IActionResult> PutProduto(Produto produto)
        {
            _context.Entry(produto).State = EntityState.Modified;
            _context.Entry(produto.Categoria).State = EntityState.Unchanged;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT Selecionando o ID e adicionando a quantidade do produto, somando a quantidade em estoque
        // PUT: api/Produto/Produto/{id}/Compra/quantidade/{quantidade
        //
        [HttpPatch("Produto/{id}/Compra/quantidade/{quantidade}")]
        public async Task<IActionResult> PutQtdProduto([FromRoute] int id, [FromRoute] int quantidade)
        {
            var Compra = new Produto() { ID = id, quantidade = quantidade };

            var meuProduto = await _context.Produto.AsNoTracking().Where(c => c.ID == Compra.ID).FirstOrDefaultAsync();
            meuProduto.quantidade = meuProduto.quantidade + Compra.quantidade;
            _context.Produto.Attach(meuProduto);
            _context.Entry(meuProduto).Property(c => c.quantidade).IsModified = true;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Produto
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {

            _context.Produto.Add(produto);
            _context.Entry(produto.Categoria).State = EntityState.Unchanged;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduto", new { id = produto.ID }, produto);
        }

        // DELETE: api/Produto/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produto.AsNoTracking().Include(e => e.Categoria).Where(c => c.ID == id).FirstOrDefaultAsync();
            produto.ativo = false;
            _context.Entry(produto).State = EntityState.Modified;
            _context.Entry(produto.Categoria).State = EntityState.Unchanged;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.ID == id);
        }
    }
}
