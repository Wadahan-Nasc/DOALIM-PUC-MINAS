using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class AlinhaVitrineProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Doacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Doacoes",
                columns: table => new
                {
                    IdProduto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDoador = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FotoProduto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MarcaProduto = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    QuantidadeDisponivel = table.Column<int>(type: "int", nullable: false),
                    StatusProduto = table.Column<bool>(type: "bit", nullable: false),
                    TipoArmazenamento = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
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
    }
}
