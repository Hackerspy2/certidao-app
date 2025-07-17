using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class _15032024_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrosEmissaoApi",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Pdf",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "PlexiRequestId",
                table: "Ticket");

            migrationBuilder.AddColumn<string>(
                name: "Dominio",
                table: "Visita",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dominio",
                table: "Visita");

            migrationBuilder.AddColumn<string>(
                name: "ErrosEmissaoApi",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pdf",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlexiRequestId",
                table: "Ticket",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
