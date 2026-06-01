using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class M10RefatoracaoCadastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'Usuarios', N'Endereco') IS NOT NULL
BEGIN
    DECLARE @constraint_Endereco sysname;
    SELECT @constraint_Endereco = d.name
    FROM sys.default_constraints d
    INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
    WHERE d.parent_object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'Endereco';

    IF @constraint_Endereco IS NOT NULL
        EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @constraint_Endereco + N'];');

    ALTER TABLE [Usuarios] DROP COLUMN [Endereco];
END
");

            ConverterColunaArquivoUsuarioParaVarbinary(migrationBuilder, "FotoPerfil");
            ConverterColunaArquivoUsuarioParaVarbinary(migrationBuilder, "Arquivocomprovacao");

            migrationBuilder.CreateTable(
                name: "DocumentosVerificacao",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Arquivo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusValidacao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosVerificacao", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_DocumentosVerificacao_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enderecos",
                columns: table => new
                {
                    IdEndereco = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Logradouro = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enderecos", x => x.IdEndereco);
                    table.ForeignKey(
                        name: "FK_Enderecos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosVerificacao_IdUsuario",
                table: "DocumentosVerificacao",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Enderecos_IdUsuario",
                table: "Enderecos",
                column: "IdUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosVerificacao");

            migrationBuilder.DropTable(
                name: "Enderecos");

            ConverterColunaArquivoUsuarioParaTexto(migrationBuilder, "FotoPerfil");
            ConverterColunaArquivoUsuarioParaTexto(migrationBuilder, "Arquivocomprovacao");

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Usuarios",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }

        private static void ConverterColunaArquivoUsuarioParaVarbinary(MigrationBuilder migrationBuilder, string coluna)
        {
            migrationBuilder.Sql($@"
IF COL_LENGTH(N'Usuarios', N'{coluna}') IS NOT NULL
BEGIN
    DECLARE @tipo_{coluna} sysname;
    SELECT @tipo_{coluna} = t.name
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'{coluna}';

    DECLARE @constraint_{coluna} sysname;
    SELECT @constraint_{coluna} = d.name
    FROM sys.default_constraints d
    INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
    WHERE d.parent_object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'{coluna}';

    IF @constraint_{coluna} IS NOT NULL
        EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @constraint_{coluna} + N'];');

    IF @tipo_{coluna} IN (N'binary', N'varbinary', N'image')
    BEGIN
        ALTER TABLE [Usuarios] ALTER COLUMN [{coluna}] varbinary(max) NULL;
    END
    ELSE
    BEGIN
        EXEC sp_rename N'[Usuarios].[{coluna}]', N'{coluna}TextoAntigo', N'COLUMN';
        ALTER TABLE [Usuarios] ADD [{coluna}] varbinary(max) NULL;
        ALTER TABLE [Usuarios] DROP COLUMN [{coluna}TextoAntigo];
    END
END
");
        }

        private static void ConverterColunaArquivoUsuarioParaTexto(MigrationBuilder migrationBuilder, string coluna)
        {
            migrationBuilder.Sql($@"
IF COL_LENGTH(N'Usuarios', N'{coluna}') IS NOT NULL
BEGIN
    DECLARE @tipo_down_{coluna} sysname;
    SELECT @tipo_down_{coluna} = t.name
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'{coluna}';

    DECLARE @constraint_down_{coluna} sysname;
    SELECT @constraint_down_{coluna} = d.name
    FROM sys.default_constraints d
    INNER JOIN sys.columns c ON d.parent_column_id = c.column_id AND d.parent_object_id = c.object_id
    WHERE d.parent_object_id = OBJECT_ID(N'[Usuarios]')
      AND c.name = N'{coluna}';

    IF @constraint_down_{coluna} IS NOT NULL
        EXEC(N'ALTER TABLE [Usuarios] DROP CONSTRAINT [' + @constraint_down_{coluna} + N'];');

    IF @tipo_down_{coluna} IN (N'nvarchar', N'varchar', N'nchar', N'char')
    BEGIN
        ALTER TABLE [Usuarios] ALTER COLUMN [{coluna}] nvarchar(500) NULL;
    END
    ELSE
    BEGIN
        EXEC sp_rename N'[Usuarios].[{coluna}]', N'{coluna}BinarioAntigo', N'COLUMN';
        ALTER TABLE [Usuarios] ADD [{coluna}] nvarchar(500) NULL;
        ALTER TABLE [Usuarios] DROP COLUMN [{coluna}BinarioAntigo];
    END
END
");
        }
    }
}
