using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class BeerOGandFG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "fg",
                table: "beers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "og",
                table: "beers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fg",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "og",
                table: "beers");
        }
    }
}
