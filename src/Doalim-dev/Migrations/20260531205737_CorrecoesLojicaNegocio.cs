using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class CorrecoesLojicaNegocio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoRejeicao",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoRejeicao",
                table: "Reservas");
        }
    }
}
