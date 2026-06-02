using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class AddLoteNasReservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Reservas]', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'[FK_Reservas_Produtos_IdProduto]', N'F') IS NOT NULL
        ALTER TABLE [Reservas] DROP CONSTRAINT [FK_Reservas_Produtos_IdProduto];

    IF COL_LENGTH(N'Reservas', N'IdProduto') IS NOT NULL
       AND COL_LENGTH(N'Reservas', N'IdLote') IS NULL
    BEGIN
        EXEC sp_rename N'[Reservas].[IdProduto]', N'IdLote', N'COLUMN';
    END

    IF EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_Reservas_IdProduto'
          AND object_id = OBJECT_ID(N'[Reservas]')
    )
    BEGIN
        EXEC sp_rename N'[Reservas].[IX_Reservas_IdProduto]', N'IX_Reservas_IdLote', N'INDEX';
    END

    IF COL_LENGTH(N'Reservas', N'IdLote') IS NOT NULL
       AND OBJECT_ID(N'[Lotes]', N'U') IS NOT NULL
       AND OBJECT_ID(N'[Produtos]', N'U') IS NOT NULL
    BEGIN
        EXEC(N'
        INSERT INTO [Lotes] ([NumeroLote], [DataValidade], [Quantidade], [StatusLote], [IdProduto])
        SELECT CONCAT(N''MIGRADO-'', r.[IdLote]), SYSUTCDATETIME(), 0, 3, r.[IdLote]
        FROM [Reservas] r
        INNER JOIN [Produtos] p ON p.[IdProduto] = r.[IdLote]
        WHERE NOT EXISTS (SELECT 1 FROM [Lotes] l WHERE l.[IdProduto] = r.[IdLote])
          AND NOT EXISTS (SELECT 1 FROM [Lotes] l WHERE l.[IdLote] = r.[IdLote])
        GROUP BY r.[IdLote];

        UPDATE r
        SET [IdLote] = lote.[IdLote]
        FROM [Reservas] r
        CROSS APPLY (
            SELECT TOP 1 l.[IdLote]
            FROM [Lotes] l
            WHERE l.[IdProduto] = r.[IdLote]
            ORDER BY l.[DataValidade], l.[IdLote]
        ) lote
        WHERE NOT EXISTS (SELECT 1 FROM [Lotes] l WHERE l.[IdLote] = r.[IdLote]);
        ');
    END

    IF COL_LENGTH(N'Reservas', N'IdLote') IS NOT NULL
       AND OBJECT_ID(N'[FK_Reservas_Lotes_IdLote]', N'F') IS NULL
       AND OBJECT_ID(N'[Lotes]', N'U') IS NOT NULL
    BEGIN
        EXEC(N'
        IF NOT EXISTS (
            SELECT 1
            FROM [Reservas] r
            LEFT JOIN [Lotes] l ON l.[IdLote] = r.[IdLote]
            WHERE l.[IdLote] IS NULL
        )
        BEGIN
            ALTER TABLE [Reservas]
            ADD CONSTRAINT [FK_Reservas_Lotes_IdLote]
            FOREIGN KEY ([IdLote]) REFERENCES [Lotes] ([IdLote]) ON DELETE NO ACTION;
        END
        ');
    END
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Reservas]', N'U') IS NOT NULL
BEGIN
    IF OBJECT_ID(N'[FK_Reservas_Lotes_IdLote]', N'F') IS NOT NULL
        ALTER TABLE [Reservas] DROP CONSTRAINT [FK_Reservas_Lotes_IdLote];

    IF COL_LENGTH(N'Reservas', N'IdLote') IS NOT NULL
       AND COL_LENGTH(N'Reservas', N'IdProduto') IS NULL
       AND OBJECT_ID(N'[Lotes]', N'U') IS NOT NULL
    BEGIN
        EXEC(N'
        UPDATE r
        SET [IdLote] = l.[IdProduto]
        FROM [Reservas] r
        INNER JOIN [Lotes] l ON l.[IdLote] = r.[IdLote];
        ');

        EXEC sp_rename N'[Reservas].[IdLote]', N'IdProduto', N'COLUMN';
    END

    IF EXISTS (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_Reservas_IdLote'
          AND object_id = OBJECT_ID(N'[Reservas]')
    )
    BEGIN
        EXEC sp_rename N'[Reservas].[IX_Reservas_IdLote]', N'IX_Reservas_IdProduto', N'INDEX';
    END

    IF COL_LENGTH(N'Reservas', N'IdProduto') IS NOT NULL
       AND OBJECT_ID(N'[FK_Reservas_Produtos_IdProduto]', N'F') IS NULL
    BEGIN
        ALTER TABLE [Reservas]
        ADD CONSTRAINT [FK_Reservas_Produtos_IdProduto]
        FOREIGN KEY ([IdProduto]) REFERENCES [Produtos] ([IdProduto]) ON DELETE NO ACTION;
    END
END
");
        }
    }
}
