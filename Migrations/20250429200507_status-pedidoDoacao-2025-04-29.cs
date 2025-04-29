using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ong.Migrations
{
    /// <inheritdoc />
    public partial class statuspedidoDoacao20250429 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PedidoDoacao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PedidoDoacao");
        }
    }
}
