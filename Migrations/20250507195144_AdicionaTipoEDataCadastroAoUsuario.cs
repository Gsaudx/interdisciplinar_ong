using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ong.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTipoEDataCadastroAoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Usuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Usuario",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Usuario");
        }
    }
}
