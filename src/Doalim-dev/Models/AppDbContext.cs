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
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<DocumentoVerificacao> DocumentosVerificacao { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }
        public DbSet<ValorLookup> ValoresLookup { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Garante que Email seja único na tabela
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Cpf)
                .IsUnique()
                .HasFilter("[Cpf] IS NOT NULL AND [Cpf] <> ''");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Cnpj)
                .IsUnique()
                .HasFilter("[Cnpj] IS NOT NULL AND [Cnpj] <> ''");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Telefone)
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

            // Endereço é 1:1 com Usuario
            modelBuilder.Entity<Endereco>()
                .HasOne(e => e.Usuario)
                .WithOne(u => u.Endereco)
                .HasForeignKey<Endereco>(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // DocumentoVerificacao é N:1 com Usuario
            modelBuilder.Entity<DocumentoVerificacao>()
                .HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.IdUsuario)
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
                .OnDelete(DeleteBehavior.Restrict); // Evita exclusão de lote com reservas associadas

            // Relacionamento 1:N entre Beneficiario e Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Beneficiario)
                .WithMany()
                .HasForeignKey(r => r.IdBeneficiario)
                .OnDelete(DeleteBehavior.Restrict); // Mantém histórico mesmo se beneficiário for desativado

            // Relacionamento 1:N entre Pedido e Reserva
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Pedido)
                .WithMany(p => p.Reservas)
                .HasForeignKey(r => r.IdPedido)
                .OnDelete(DeleteBehavior.Restrict); // Evita exclusão de pedido com reservas associadas

            // Relacionamento 1:N entre Beneficiario e Pedido
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Beneficiario)
                .WithMany()
                .HasForeignKey(p => p.IdBeneficiario)
                .OnDelete(DeleteBehavior.Restrict); // Mantém histórico de pedidos mesmo se beneficiário for desativado

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

            // Garante que um beneficiário não adicione o mesmo produto duas vezes no carrinho — controlado via índice único
            modelBuilder.Entity<CarrinhoItem>()
                .HasIndex(c => new { c.IdBeneficiario, c.IdProduto })
                .IsUnique();

            // Índice de unicidade: mesmo tipo não pode ter dois valores com o mesmo nome
            modelBuilder.Entity<ValorLookup>()
                .HasIndex(v => new { v.Tipo, v.Nome })
                .IsUnique();

            // Seed dos valores padrão de domínio — EhValorPadrao = true protege da exclusão
            modelBuilder.Entity<ValorLookup>().HasData(
                // Categorias
                new ValorLookup { IdValor = 1,  Tipo = TipoLookup.Categoria,          Nome = "Grão",                Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 2,  Tipo = TipoLookup.Categoria,          Nome = "Bebida",              Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 3,  Tipo = TipoLookup.Categoria,          Nome = "Carne",               Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 4,  Tipo = TipoLookup.Categoria,          Nome = "Produtos de Limpeza", Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 5,  Tipo = TipoLookup.Categoria,          Nome = "Higiene Pessoal",     Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 6,  Tipo = TipoLookup.Categoria,          Nome = "Laticínios",          Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 7,  Tipo = TipoLookup.Categoria,          Nome = "Verdura",             Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 8,  Tipo = TipoLookup.Categoria,          Nome = "Legume",              Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 9,  Tipo = TipoLookup.Categoria,          Nome = "Fruta",               Ativo = true, EhValorPadrao = true },
                // Unidades de medida
                new ValorLookup { IdValor = 10, Tipo = TipoLookup.UnidadeMedida,      Nome = "Kg",                  Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 11, Tipo = TipoLookup.UnidadeMedida,      Nome = "mg",                  Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 12, Tipo = TipoLookup.UnidadeMedida,      Nome = "L",                   Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 13, Tipo = TipoLookup.UnidadeMedida,      Nome = "ml",                  Ativo = true, EhValorPadrao = true },
                // Tipos de armazenamento
                new ValorLookup { IdValor = 14, Tipo = TipoLookup.TipoArmazenamento,  Nome = "Ambiente",            Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 15, Tipo = TipoLookup.TipoArmazenamento,  Nome = "Congelado",           Ativo = true, EhValorPadrao = true },
                new ValorLookup { IdValor = 16, Tipo = TipoLookup.TipoArmazenamento,  Nome = "Local fechado",       Ativo = true, EhValorPadrao = true }
            );

            // Avaliações: duas FKs para Usuario sem cascade duplo (evita ciclo de cascade delete)
            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Avaliador)
                .WithMany()
                .HasForeignKey(a => a.IdAvaliador)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Avaliado)
                .WithMany()
                .HasForeignKey(a => a.IdAvaliado)
                .OnDelete(DeleteBehavior.Cascade);

            // Cada reserva permite no maximo uma avaliacao por avaliador
            modelBuilder.Entity<Avaliacao>()
                .HasIndex(a => new { a.IdAvaliador, a.IdReserva })
                .IsUnique();

            modelBuilder.Entity<Avaliacao>()
                .HasOne(a => a.Reserva)
                .WithMany()
                .HasForeignKey(a => a.IdReserva)
                .OnDelete(DeleteBehavior.SetNull);

            // Notificações: FK para Usuario com cascade delete
            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            // ChaveDuplicacao é único por usuário (índice parcial filtrando nulos)
            modelBuilder.Entity<Notificacao>()
                .HasIndex(n => new { n.IdUsuario, n.ChaveDuplicacao })
                .IsUnique()
                .HasFilter("[ChaveDuplicacao] IS NOT NULL");
        }
    }
}
