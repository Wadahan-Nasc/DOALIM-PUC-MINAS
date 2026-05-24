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
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Produtos_IdProduto",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "IdProduto",
                table: "Reservas",
                newName: "IdLote");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_IdProduto",
                table: "Reservas",
                newName: "IX_Reservas_IdLote");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Lotes_IdLote",
                table: "Reservas",
                column: "IdLote",
                principalTable: "Lotes",
                principalColumn: "IdLote",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Lotes_IdLote",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "IdLote",
                table: "Reservas",
                newName: "IdProduto");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_IdLote",
                table: "Reservas",
                newName: "IX_Reservas_IdProduto");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Produtos_IdProduto",
                table: "Reservas",
                column: "IdProduto",
                principalTable: "Produtos",
                principalColumn: "IdProduto",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
