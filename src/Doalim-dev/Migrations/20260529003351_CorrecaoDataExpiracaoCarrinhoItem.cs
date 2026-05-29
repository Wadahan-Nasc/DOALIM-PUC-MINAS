using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoDataExpiracaoCarrinhoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "CarrinhoItens",
                newName: "QuantidadeDesejada");

            migrationBuilder.RenameColumn(
                name: "Expiracao",
                table: "CarrinhoItens",
                newName: "DataExpiracao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeDesejada",
                table: "CarrinhoItens",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "DataExpiracao",
                table: "CarrinhoItens",
                newName: "Expiracao");
        }
    }
}
