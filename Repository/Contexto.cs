using Microsoft.EntityFrameworkCore;
using Domain;

namespace Repository
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Pagador> Pagador { get; set; }
        public DbSet<Configuracao> Configuracao { get; set; }
        public DbSet<Interacao> Interacao { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<Visita> Visita { get; set; }

    }
}