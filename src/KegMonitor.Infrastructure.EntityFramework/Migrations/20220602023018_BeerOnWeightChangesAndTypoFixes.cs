using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    public partial class BeerOnWeightChangesAndTypoFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_updated_dated",
                table: "scales",
                newName: "last_updated_date");

            migrationBuilder.AddColumn<int>(
                name: "recording_difference_threshold",
                table: "scales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "beer_id",
                table: "scale_weight_changes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_scale_weight_changes_beer_id",
                table: "scale_weight_changes",
                column: "beer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_scale_weight_changes_beers_beer_id",
                table: "scale_weight_changes",
                column: "beer_id",
                principalTable: "beers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_scale_weight_changes_beers_beer_id",
                table: "scale_weight_changes");

            migrationBuilder.DropIndex(
                name: "IX_scale_weight_changes_beer_id",
                table: "scale_weight_changes");

            migrationBuilder.DropColumn(
                name: "recording_difference_threshold",
                table: "scales");

            migrationBuilder.DropColumn(
                name: "beer_id",
                table: "scale_weight_changes");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "scales",
                newName: "last_updated_dated");
        }
    }
}
