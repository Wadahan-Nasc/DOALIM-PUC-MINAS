using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaReservaProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataReserva",
                table: "Produtos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdBeneficiario",
                table: "Produtos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataReserva",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "IdBeneficiario",
                table: "Produtos");
        }
    }
}
