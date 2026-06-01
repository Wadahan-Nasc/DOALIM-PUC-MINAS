using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class RestringeIdentificadoresUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cnpj",
                table: "Usuarios",
                column: "Cnpj",
                unique: true,
                filter: "[Cnpj] IS NOT NULL AND [Cnpj] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cpf",
                table: "Usuarios",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL AND [Cpf] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Telefone",
                table: "Usuarios",
                column: "Telefone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Cnpj",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Cpf",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Telefone",
                table: "Usuarios");
        }
    }
}
