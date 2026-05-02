using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TermoAceitacao> TermosAceitacao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Garante que Email seja único na tabela
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Relacionamento 1:N entre Usuario e TermoAceitacao
            modelBuilder.Entity<TermoAceitacao>()
                .HasOne(t => t.Usuario)
                .WithMany(u => u.TermosAceitados)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
