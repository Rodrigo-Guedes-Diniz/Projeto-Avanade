using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoAvanade.Migrations
{
    /// <inheritdoc />
    public partial class criandoDTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmEstoque",
                table: "Produtos",
                newName: "Quantidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "Produtos",
                newName: "EmEstoque");
        }
    }
}
