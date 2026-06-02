using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoLogicaAvaliacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Avaliacoes_IdAvaliador_IdAvaliado",
                table: "Avaliacoes");

            migrationBuilder.AddColumn<int>(
                name: "IdReserva",
                table: "Avaliacoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_IdAvaliador_IdReserva",
                table: "Avaliacoes",
                columns: new[] { "IdAvaliador", "IdReserva" },
                unique: true,
                filter: "[IdReserva] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_IdReserva",
                table: "Avaliacoes",
                column: "IdReserva");

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Reservas_IdReserva",
                table: "Avaliacoes",
                column: "IdReserva",
                principalTable: "Reservas",
                principalColumn: "IdReserva",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Reservas_IdReserva",
                table: "Avaliacoes");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacoes_IdAvaliador_IdReserva",
                table: "Avaliacoes");

            migrationBuilder.DropIndex(
                name: "IX_Avaliacoes_IdReserva",
                table: "Avaliacoes");

            migrationBuilder.DropColumn(
                name: "IdReserva",
                table: "Avaliacoes");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_IdAvaliador_IdAvaliado",
                table: "Avaliacoes",
                columns: new[] { "IdAvaliador", "IdAvaliado" },
                unique: true);
        }
    }
}
