using EstoqueApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EstoqueApi.Data
{
    public class ProdutoContext : DbContext
    {

        public ProdutoContext(DbContextOptions<ProdutoContext> options) : base(options)
        {

        }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Venda> Venda { get; set; }


    }
}
