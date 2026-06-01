using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarrinhoPedidosRelacionados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarrinhoItens_IdBeneficiario",
                table: "CarrinhoItens");

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEncerramento",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoteIdLote",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantidade",
                table: "CarrinhoItens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_LoteIdLote",
                table: "Reservas",
                column: "LoteIdLote");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItens_IdBeneficiario_IdProduto",
                table: "CarrinhoItens",
                columns: new[] { "IdBeneficiario", "IdProduto" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Lotes_LoteIdLote",
                table: "Reservas",
                column: "LoteIdLote",
                principalTable: "Lotes",
                principalColumn: "IdLote");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Lotes_LoteIdLote",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_LoteIdLote",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_CarrinhoItens_IdBeneficiario_IdProduto",
                table: "CarrinhoItens");

            migrationBuilder.DropColumn(
                name: "DataEncerramento",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "LoteIdLote",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "Quantidade",
                table: "CarrinhoItens");

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "Reservas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItens_IdBeneficiario",
                table: "CarrinhoItens",
                column: "IdBeneficiario");
        }
    }
}
