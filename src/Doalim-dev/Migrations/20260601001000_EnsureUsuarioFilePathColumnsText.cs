using Doalim_dev.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260601001000_EnsureUsuarioFilePathColumnsText")]
    public partial class EnsureUsuarioFilePathColumnsText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ConverterColunaBinariaParaTexto(migrationBuilder, "FotoPerfil");
            ConverterColunaBinariaParaTexto(migrationBuilder, "Arquivocomprovacao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }

        private static void ConverterColunaBinariaParaTexto(MigrationBuilder migrationBuilder, string coluna)
        {
            migrationBuilder.Sql($@"
IF OBJECT_ID(N'[Usuarios]', N'U') IS NOT NULL
AND EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'{coluna}'
      AND t.name IN (N'binary', N'varbinary', N'image')
)
BEGIN
    IF COL_LENGTH(N'Usuarios', N'{coluna}Texto') IS NULL
        ALTER TABLE [Usuarios] ADD [{coluna}Texto] nvarchar(500) NULL;

    EXEC(N'
    UPDATE u
    SET [{coluna}Texto] =
        CASE
            WHEN u.[{coluna}] IS NULL THEN NULL
            ELSE COALESCE(
                NULLIF(CONVERT(nvarchar(500), CONVERT(varchar(max), u.[{coluna}])), N''''),
                NULLIF(TRY_CONVERT(nvarchar(500), u.[{coluna}]), N'''')
            )
        END
    FROM [Usuarios] u;
    ');

    ALTER TABLE [Usuarios] DROP COLUMN [{coluna}];
    EXEC sp_rename N'[Usuarios].[{coluna}Texto]', N'{coluna}', N'COLUMN';
END
");
        }
    }
}
