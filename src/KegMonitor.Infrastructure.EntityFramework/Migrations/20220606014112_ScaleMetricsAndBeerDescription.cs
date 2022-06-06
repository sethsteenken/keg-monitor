using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    public partial class ScaleMetricsAndBeerDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "max_threshold",
                table: "scales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "pour_difference_threshold",
                table: "scales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "beers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_threshold",
                table: "scales");

            migrationBuilder.DropColumn(
                name: "pour_difference_threshold",
                table: "scales");

            migrationBuilder.DropColumn(
                name: "description",
                table: "beers");
        }
    }
}
