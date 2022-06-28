using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    public partial class ScaleWeightPourAndOtherThings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_threshold",
                table: "scales");

            migrationBuilder.DropColumn(
                name: "recording_difference_threshold",
                table: "scales");

            migrationBuilder.AlterColumn<int>(
                name: "beer_id",
                table: "scale_weight_changes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "is_pour_event",
                table: "scale_weight_changes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "tap_date",
                table: "beers",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_pour_event",
                table: "scale_weight_changes");

            migrationBuilder.DropColumn(
                name: "tap_date",
                table: "beers");

            migrationBuilder.AddColumn<int>(
                name: "max_threshold",
                table: "scales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "recording_difference_threshold",
                table: "scales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "beer_id",
                table: "scale_weight_changes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
