using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class CadastroProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    IdProduto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProduto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoBarras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MarcaProduto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoriaProduto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    TipoArmazenamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnidadeMedida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FotoProduto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    QuantidadePessoaFisica = table.Column<int>(type: "int", nullable: false),
                    QuantidadePessoaJuridica = table.Column<int>(type: "int", nullable: false),
                    StatusProduto = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.IdProduto);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
