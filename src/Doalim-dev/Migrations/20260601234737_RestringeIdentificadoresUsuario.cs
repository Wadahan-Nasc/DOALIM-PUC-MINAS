using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class RestringeIdentificadoresUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Usuarios]', N'U') IS NOT NULL
BEGIN
    ;WITH CpfDuplicado AS (
        SELECT [IdUsuario],
               ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM([Cpf])) ORDER BY [IdUsuario]) AS Ordem
        FROM [Usuarios]
        WHERE NULLIF(LTRIM(RTRIM([Cpf])), N'') IS NOT NULL
    )
    UPDATE u
    SET [Cpf] = NULL
    FROM [Usuarios] u
    INNER JOIN CpfDuplicado d ON d.[IdUsuario] = u.[IdUsuario]
    WHERE d.Ordem > 1;

    ;WITH CnpjDuplicado AS (
        SELECT [IdUsuario],
               ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM([Cnpj])) ORDER BY [IdUsuario]) AS Ordem
        FROM [Usuarios]
        WHERE NULLIF(LTRIM(RTRIM([Cnpj])), N'') IS NOT NULL
    )
    UPDATE u
    SET [Cnpj] = NULL
    FROM [Usuarios] u
    INNER JOIN CnpjDuplicado d ON d.[IdUsuario] = u.[IdUsuario]
    WHERE d.Ordem > 1;

    UPDATE [Usuarios]
    SET [Telefone] = CONCAT(N'MIGRADO-', [IdUsuario])
    WHERE NULLIF(LTRIM(RTRIM([Telefone])), N'') IS NULL;

    ;WITH TelefoneDuplicado AS (
        SELECT [IdUsuario],
               ROW_NUMBER() OVER (PARTITION BY LTRIM(RTRIM([Telefone])) ORDER BY [IdUsuario]) AS Ordem
        FROM [Usuarios]
        WHERE NULLIF(LTRIM(RTRIM([Telefone])), N'') IS NOT NULL
    )
    UPDATE u
    SET [Telefone] = CONCAT(N'MIGRADO-', u.[IdUsuario])
    FROM [Usuarios] u
    INNER JOIN TelefoneDuplicado d ON d.[IdUsuario] = u.[IdUsuario]
    WHERE d.Ordem > 1;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Cnpj' AND object_id = OBJECT_ID(N'[Usuarios]'))
        CREATE UNIQUE INDEX [IX_Usuarios_Cnpj] ON [Usuarios] ([Cnpj])
        WHERE [Cnpj] IS NOT NULL AND [Cnpj] <> '';

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Cpf' AND object_id = OBJECT_ID(N'[Usuarios]'))
        CREATE UNIQUE INDEX [IX_Usuarios_Cpf] ON [Usuarios] ([Cpf])
        WHERE [Cpf] IS NOT NULL AND [Cpf] <> '';

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Telefone' AND object_id = OBJECT_ID(N'[Usuarios]'))
        CREATE UNIQUE INDEX [IX_Usuarios_Telefone] ON [Usuarios] ([Telefone]);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Usuarios]', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Cnpj' AND object_id = OBJECT_ID(N'[Usuarios]'))
        DROP INDEX [IX_Usuarios_Cnpj] ON [Usuarios];

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Cpf' AND object_id = OBJECT_ID(N'[Usuarios]'))
        DROP INDEX [IX_Usuarios_Cpf] ON [Usuarios];

    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Usuarios_Telefone' AND object_id = OBJECT_ID(N'[Usuarios]'))
        DROP INDEX [IX_Usuarios_Telefone] ON [Usuarios];
END
");
        }
    }
}
