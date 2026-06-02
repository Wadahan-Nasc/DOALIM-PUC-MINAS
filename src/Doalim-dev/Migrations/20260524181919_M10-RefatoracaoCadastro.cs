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
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Usuarios");

            migrationBuilder.Sql("ALTER TABLE [Usuarios] DROP COLUMN [FotoPerfil];");
            migrationBuilder.Sql("ALTER TABLE [Usuarios] ADD [FotoPerfil] varbinary(max) NULL;");

            migrationBuilder.Sql("ALTER TABLE [Usuarios] DROP COLUMN [Arquivocomprovacao];");
            migrationBuilder.Sql("ALTER TABLE [Usuarios] ADD [Arquivocomprovacao] varbinary(max) NULL;");

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

            migrationBuilder.Sql("ALTER TABLE [Usuarios] DROP COLUMN [FotoPerfil];");
            migrationBuilder.Sql("ALTER TABLE [Usuarios] ADD [FotoPerfil] nvarchar(500) NULL;");

            migrationBuilder.Sql("ALTER TABLE [Usuarios] DROP COLUMN [Arquivocomprovacao];");
            migrationBuilder.Sql("ALTER TABLE [Usuarios] ADD [Arquivocomprovacao] nvarchar(500) NULL;");

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Usuarios",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
