using Doalim_dev.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260602120000_M12-AvaliacaoPorReserva")]
    public partial class M12AvaliacaoPorReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Remove avaliacoes existentes (incompativeis com o novo modelo por-reserva)
            migrationBuilder.Sql("DELETE FROM [Avaliacoes];");

            // 2. Remove o indice unico antigo (IdAvaliador, IdAvaliado)
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Avaliacoes_IdAvaliador_IdAvaliado'
      AND object_id = OBJECT_ID(N'[Avaliacoes]')
)
    DROP INDEX [IX_Avaliacoes_IdAvaliador_IdAvaliado] ON [Avaliacoes];
");

            // 3. Adiciona coluna IdReserva
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[Avaliacoes]', N'IdReserva') IS NULL
    ALTER TABLE [Avaliacoes] ADD [IdReserva] int NULL;
");

            // 4. FK: Avaliacoes.IdReserva -> Reservas.IdReserva (SET NULL ao excluir reserva)
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Avaliacoes_Reservas_IdReserva'
)
    ALTER TABLE [Avaliacoes]
        ADD CONSTRAINT [FK_Avaliacoes_Reservas_IdReserva]
        FOREIGN KEY ([IdReserva]) REFERENCES [Reservas] ([IdReserva])
        ON DELETE SET NULL;
");

            // 5. Novo indice unico: um avaliador pode avaliar cada reserva apenas uma vez
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Avaliacoes_IdAvaliador_IdReserva'
      AND object_id = OBJECT_ID(N'[Avaliacoes]')
)
    CREATE UNIQUE INDEX [IX_Avaliacoes_IdAvaliador_IdReserva]
        ON [Avaliacoes] ([IdAvaliador], [IdReserva])
        WHERE [IdReserva] IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove indice novo e FK
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Avaliacoes_IdAvaliador_IdReserva'
      AND object_id = OBJECT_ID(N'[Avaliacoes]')
)
    DROP INDEX [IX_Avaliacoes_IdAvaliador_IdReserva] ON [Avaliacoes];
");

            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Avaliacoes_Reservas_IdReserva'
)
    ALTER TABLE [Avaliacoes] DROP CONSTRAINT [FK_Avaliacoes_Reservas_IdReserva];
");

            // Remove coluna IdReserva
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[Avaliacoes]', N'IdReserva') IS NOT NULL
    ALTER TABLE [Avaliacoes] DROP COLUMN [IdReserva];
");

            // Restaura indice unico antigo
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Avaliacoes_IdAvaliador_IdAvaliado'
      AND object_id = OBJECT_ID(N'[Avaliacoes]')
)
    CREATE UNIQUE INDEX [IX_Avaliacoes_IdAvaliador_IdAvaliado]
        ON [Avaliacoes] ([IdAvaliador], [IdAvaliado]);
");
        }
    }
}
