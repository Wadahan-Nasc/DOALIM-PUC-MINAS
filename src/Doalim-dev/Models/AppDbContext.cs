using Microsoft.EntityFrameworkCore;

namespace Doalim_dev.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TermoAceitacao> TermosAceitacao { get; set; }
        public DbSet<Doador> Doadores { get; set; }
        public DbSet<Beneficiario> Beneficiarios { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }

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

            // Doador é uma extensão de Usuario (1:1)
            modelBuilder.Entity<Doador>()
                .HasOne(d => d.Usuario)
                .WithOne()
                .HasForeignKey<Doador>(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Beneficiario é uma extensão de Usuario (1:1)
            modelBuilder.Entity<Beneficiario>()
                .HasOne(b => b.Usuario)
                .WithOne()
                .HasForeignKey<Beneficiario>(b => b.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Administrador é uma extensão de Usuario (1:1)
            modelBuilder.Entity<Administrador>()
                .HasOne(a => a.Usuario)
                .WithOne()
                .HasForeignKey<Administrador>(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre Doador e Produto
            modelBuilder.Entity<Produto>()
                .HasOne(d => d.Doador)
                .WithMany(dc => dc.Produtos)
                .HasForeignKey(d => d.IdDoador)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre Produto e Lote
            modelBuilder.Entity<Lote>()
                .HasOne(l => l.Produto)
                .WithMany(p => p.Lotes)
                .HasForeignKey(l => l.IdProduto)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre Lote e Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Lote)
                .WithMany()
                .HasForeignKey(r => r.IdLote)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Beneficiario e Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Beneficiario)
                .WithMany()
                .HasForeignKey(r => r.IdBeneficiario)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Pedido e Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Pedido)
                .WithMany(p => p.Reservas)
                .HasForeignKey(r => r.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Beneficiario e Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Beneficiario)
                .WithMany()
                .HasForeignKey(p => p.IdBeneficiario)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento 1:N entre Beneficiario e CarrinhoItem
            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(c => c.Beneficiario)
                .WithMany()
                .HasForeignKey(c => c.IdBeneficiario)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento 1:N entre Produto e CarrinhoItem
            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(c => c.Produto)
                .WithMany()
                .HasForeignKey(c => c.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}