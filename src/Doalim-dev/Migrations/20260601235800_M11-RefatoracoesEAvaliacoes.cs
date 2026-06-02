using System;
using Doalim_dev.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260601235800_M11-RefatoracoesEAvaliacoes")]
    public partial class M11RefatoracoesEAvaliacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. Renomeia Arquivocomprovacao → ArquivoComprovacao ────────────
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'Usuarios', N'Arquivocomprovacao') IS NOT NULL
    AND COL_LENGTH(N'Usuarios', N'ArquivoComprovacao') IS NULL
BEGIN
    EXEC sp_rename N'[Usuarios].[Arquivocomprovacao]', N'ArquivoComprovacao', N'COLUMN';
END
");

            // ── 2a. Adiciona EhValorPadrao na tabela ValoresLookup ────────────
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'ValoresLookup', N'EhValorPadrao') IS NULL
    ALTER TABLE [ValoresLookup] ADD [EhValorPadrao] bit NOT NULL DEFAULT 0;
");

            // ── 2b. Marca os 16 valores seed como padrão (batch separado) ─────
            // Separado do ALTER TABLE para evitar erro de compilação:
            // SQL Server compila o batch inteiro antes de executar,
            // então o UPDATE falharia se estivesse no mesmo batch do ALTER.
            migrationBuilder.Sql(@"
UPDATE [ValoresLookup] SET [EhValorPadrao] = 1 WHERE [IdValor] BETWEEN 1 AND 16;
");

            // ── 3. Cria tabela Avaliacoes ────────────────────────────────────
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Avaliacoes]', N'U') IS NULL
BEGIN
    CREATE TABLE [Avaliacoes] (
        [IdAvaliacao]   int IDENTITY(1,1) NOT NULL,
        [IdAvaliador]   int NOT NULL,
        [IdAvaliado]    int NOT NULL,
        [Nota]          int NOT NULL,
        [Comentario]    nvarchar(500) NULL,
        [DataAvaliacao] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Avaliacoes] PRIMARY KEY ([IdAvaliacao]),
        CONSTRAINT [FK_Avaliacoes_Avaliador] FOREIGN KEY ([IdAvaliador])
            REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Avaliacoes_Avaliado] FOREIGN KEY ([IdAvaliado])
            REFERENCES [Usuarios] ([IdUsuario]) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX [IX_Avaliacoes_IdAvaliador_IdAvaliado]
        ON [Avaliacoes] ([IdAvaliador], [IdAvaliado]);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove tabela Avaliacoes
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Avaliacoes]', N'U') IS NOT NULL
    DROP TABLE [Avaliacoes];
");

            // Remove coluna EhValorPadrao
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'ValoresLookup', N'EhValorPadrao') IS NOT NULL
    ALTER TABLE [ValoresLookup] DROP COLUMN [EhValorPadrao];
");

            // Renomeia de volta ArquivoComprovacao → Arquivocomprovacao
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'Usuarios', N'ArquivoComprovacao') IS NOT NULL
    AND COL_LENGTH(N'Usuarios', N'Arquivocomprovacao') IS NULL
BEGIN
    EXEC sp_rename N'[Usuarios].[ArquivoComprovacao]', N'Arquivocomprovacao', N'COLUMN';
END
");
        }
    }
}
