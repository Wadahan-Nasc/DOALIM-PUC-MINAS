using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesUsuariosAddTableDoacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradores",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradores", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Administradores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Beneficiarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    CadastroUnico = table.Column<int>(type: "int", nullable: false),
                    Eong = table.Column<bool>(type: "bit", nullable: false),
                    QuantidadeAlimentosRecebidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Beneficiarios_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doadores",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    QtdAlimentosDoados = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doadores", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Doadores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doacoes",
                columns: table => new
                {
                    IdProduto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StatusProduto = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TipoArmazenamento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    MarcaProduto = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    FotoProduto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false),
                    IdDoador = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doacoes", x => x.IdProduto);
                    table.ForeignKey(
                        name: "FK_Doacoes_Doadores_IdDoador",
                        column: x => x.IdDoador,
                        principalTable: "Doadores",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doacoes_IdDoador",
                table: "Doacoes",
                column: "IdDoador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administradores");

            migrationBuilder.DropTable(
                name: "Beneficiarios");

            migrationBuilder.DropTable(
                name: "Doacoes");

            migrationBuilder.DropTable(
                name: "Doadores");
        }
    }
}
