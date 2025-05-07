using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ong.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaDataDoacaoEPedidoDoacaoOpcionalEmDoacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Usuario");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDoacao",
                table: "Doacao",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PedidoDoacaoId",
                table: "Doacao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doacao_PedidoDoacaoId",
                table: "Doacao",
                column: "PedidoDoacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doacao_PedidoDoacao_PedidoDoacaoId",
                table: "Doacao",
                column: "PedidoDoacaoId",
                principalTable: "PedidoDoacao",
                principalColumn: "PedidoDoacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doacao_PedidoDoacao_PedidoDoacaoId",
                table: "Doacao");

            migrationBuilder.DropIndex(
                name: "IX_Doacao_PedidoDoacaoId",
                table: "Doacao");

            migrationBuilder.DropColumn(
                name: "DataDoacao",
                table: "Doacao");

            migrationBuilder.DropColumn(
                name: "PedidoDoacaoId",
                table: "Doacao");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
