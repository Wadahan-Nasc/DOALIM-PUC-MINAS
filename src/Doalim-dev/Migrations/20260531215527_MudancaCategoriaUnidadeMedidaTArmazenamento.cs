using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Doalim_dev.Migrations
{
    /// <inheritdoc />
    public partial class MudancaCategoriaUnidadeMedidaTArmazenamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValoresLookup",
                columns: table => new
                {
                    IdValor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoresLookup", x => x.IdValor);
                });

            migrationBuilder.InsertData(
                table: "ValoresLookup",
                columns: new[] { "IdValor", "Ativo", "Nome", "Tipo" },
                values: new object[,]
                {
                    { 1, true, "Grão", 0 },
                    { 2, true, "Bebida", 0 },
                    { 3, true, "Carne", 0 },
                    { 4, true, "Produtos de Limpeza", 0 },
                    { 5, true, "Higiene Pessoal", 0 },
                    { 6, true, "Laticínios", 0 },
                    { 7, true, "Verdura", 0 },
                    { 8, true, "Legume", 0 },
                    { 9, true, "Fruta", 0 },
                    { 10, true, "Kg", 2 },
                    { 11, true, "mg", 2 },
                    { 12, true, "L", 2 },
                    { 13, true, "ml", 2 },
                    { 14, true, "Ambiente", 1 },
                    { 15, true, "Congelado", 1 },
                    { 16, true, "Local fechado", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ValoresLookup_Tipo_Nome",
                table: "ValoresLookup",
                columns: new[] { "Tipo", "Nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValoresLookup");
        }
    }
}
