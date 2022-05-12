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
    public class VendaController : ControllerBase
    {
        private readonly ProdutoContext _context;

        public VendaController(ProdutoContext context)
        {
            _context = context;
        }

        // Ultimas 10 vendas do estoque
        // GET: api/Venda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venda>>> GetUltimasVendas()
        {

            return await _context.Venda.Include(b => b.Produto).OrderByDescending(c => c.ID)
                .Take(10).ToListAsync();
        }

        // Ultima saida do estoque
        // GET: api/Venda/Ultima-Saida
        [HttpGet("Ultima-saida")]
        public async Task<ActionResult<int>> GetUltimaSaida()
        {

            var estoqueQtde = _context.Venda.Sum(a => a.quantidade);

            return estoqueQtde;
        }

        // 10 Produtos mais vendidos
        // GET: api/Venda/Mais-Vendidos
        [HttpGet("Mais-Vendidos")]
        public async Task<ActionResult<List<Produto>>> GetMaisVendidos()
        {
            var maisVendidos = await _context.Venda.Include(d => d.Produto).GroupBy
                (e => e.Produto.ID).Select(b => new { ID = b.Key, count = b.Count() })
                .OrderByDescending(a => a.count).Take(10).ToListAsync();

            List<Produto> produtos = new List<Produto>();

            foreach (var item in maisVendidos)
            {
                var produto = await _context.Produto.Where(c => c.ID == item.ID)
                    .FirstOrDefaultAsync();
                produtos.Add(produto);
            }

            return produtos;
        }

        // Ultimo produtos vendido
        // GET: api/Venda/Melhor-Venda
        [HttpGet("Melhor-venda")]
        public async Task<ActionResult<List<Produto>>> GetMelhorVenda()
        {
            var maisVendidos = await _context.Venda.Include(d => d.Produto).GroupBy
                (e => e.Produto.ID).Select(b => new { ID = b.Key, count = b.Count() })
                .OrderByDescending(a => a.count).Take(1).ToListAsync();

            List<Produto> produtos = new List<Produto>();

            foreach (var item in maisVendidos)
            {
                var produto = await _context.Produto.Where(c => c.ID == item.ID)
                    .FirstOrDefaultAsync();
                produtos.Add(produto);
            }

            return produtos;
        }

        // GET: api/Venda/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Venda>> GetVenda(int id)
        {
            var venda = await _context.Venda.FindAsync(id);

            if (venda == null)
            {
                return NotFound();
            }

            return venda;
        }

        // PUT: api/Venda/5
        [HttpPut]
        public async Task<IActionResult> PutVenda(Venda venda)
        {
            _context.Entry(venda).State = EntityState.Modified;
            _context.Entry(venda.Produto).State = EntityState.Unchanged;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Produto vendido e subtraindo a quantidade em estoque
        // POST: api/Venda
        [HttpPost]
        public async Task<ActionResult<Venda>> PostVenda(Venda venda)
        {
            var produto = await _context.Produto.AsNoTracking().Where(c => c.ID ==
            venda.Produto.ID && c.quantidade >= venda.quantidade).FirstOrDefaultAsync();

            if (produto == null || produto.ativo == false)
            {
                return BadRequest("Você não possui estoque suficiente");
            }
            produto.quantidade = produto.quantidade - venda.quantidade;

            _context.Entry(produto).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _context.Entry(produto).State = EntityState.Detached;

            _context.Venda.Add(venda);
            _context.Entry(venda.Produto).State = EntityState.Unchanged;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVenda", new { id = venda.ID }, venda);
        }

        // DELETE: api/Venda/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(int id)
        {
            var venda = await _context.Venda.FindAsync(id);
            if (venda == null)
            {
                return NotFound();
            }

            _context.Venda.Remove(venda);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendaExists(int id)
        {
            return _context.Venda.Any(e => e.ID == id);
        }
    }
}
