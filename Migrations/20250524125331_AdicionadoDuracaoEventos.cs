using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ong.Migrations
{
    /// <inheritdoc />
    public partial class AdicionadoDuracaoEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracaoMinutos",
                table: "Evento",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracaoMinutos",
                table: "Evento");
        }
    }
}
