using Doalim_dev.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260601000000_EnsureSenhaHashText")]
    public partial class EnsureSenhaHashText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Usuarios]', N'U') IS NOT NULL
AND EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'SenhaHash'
      AND t.name IN (N'binary', N'varbinary', N'image')
)
BEGIN
    IF COL_LENGTH(N'Usuarios', N'SenhaHashTexto') IS NULL
        ALTER TABLE [Usuarios] ADD [SenhaHashTexto] nvarchar(max) NULL;

    EXEC(N'
    UPDATE u
    SET [SenhaHashTexto] =
        CASE
            WHEN c.AsciiHash LIKE N''$2[aby]$%'' THEN c.AsciiHash
            WHEN c.UnicodeHash LIKE N''$2[aby]$%'' THEN c.UnicodeHash
            ELSE COALESCE(NULLIF(c.AsciiHash, N''''), NULLIF(c.UnicodeHash, N''''), N'''')
        END
    FROM [Usuarios] u
    CROSS APPLY (
        SELECT
            CONVERT(nvarchar(max), CONVERT(varchar(max), u.[SenhaHash])) AS AsciiHash,
            TRY_CONVERT(nvarchar(max), u.[SenhaHash]) AS UnicodeHash
    ) c;
    ');

    ALTER TABLE [Usuarios] DROP COLUMN [SenhaHash];
    EXEC sp_rename N'[Usuarios].[SenhaHashTexto]', N'SenhaHash', N'COLUMN';

    UPDATE [Usuarios]
    SET [SenhaHash] = N''
    WHERE [SenhaHash] IS NULL;

    ALTER TABLE [Usuarios] ALTER COLUMN [SenhaHash] nvarchar(max) NOT NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
