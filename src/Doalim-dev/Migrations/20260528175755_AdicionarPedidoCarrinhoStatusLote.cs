using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPedidoCarrinhoStatusLote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataRetiradaFim",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRetiradaInicio",
                table: "Reservas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdPedido",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TokenConfirmacao",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StatusLote",
                table: "Lotes",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "CarrinhoItens",
                columns: table => new
                {
                    IdCarrinhoItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataAdicao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdBeneficiario = table.Column<int>(type: "int", nullable: false),
                    IdProduto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrinhoItens", x => x.IdCarrinhoItem);
                    table.ForeignKey(
                        name: "FK_CarrinhoItens_Beneficiarios_IdBeneficiario",
                        column: x => x.IdBeneficiario,
                        principalTable: "Beneficiarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarrinhoItens_Produtos_IdProduto",
                        column: x => x.IdProduto,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusPedido = table.Column<int>(type: "int", nullable: false),
                    IdBeneficiario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_Beneficiarios_IdBeneficiario",
                        column: x => x.IdBeneficiario,
                        principalTable: "Beneficiarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_IdPedido",
                table: "Reservas",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItens_IdBeneficiario",
                table: "CarrinhoItens",
                column: "IdBeneficiario");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoItens_IdProduto",
                table: "CarrinhoItens",
                column: "IdProduto");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_IdBeneficiario",
                table: "Pedidos",
                column: "IdBeneficiario");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Pedidos_IdPedido",
                table: "Reservas",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Pedidos_IdPedido",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "CarrinhoItens");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_IdPedido",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "DataRetiradaFim",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "DataRetiradaInicio",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "IdPedido",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "TokenConfirmacao",
                table: "Reservas");

            migrationBuilder.AlterColumn<bool>(
                name: "StatusLote",
                table: "Lotes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
