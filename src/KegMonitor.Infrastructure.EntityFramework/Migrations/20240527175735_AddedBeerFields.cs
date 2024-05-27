using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddedBeerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "brew_date",
                table: "beers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ibu",
                table: "beers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ingredients",
                table: "beers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "beers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipe",
                table: "beers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "secondary_date",
                table: "beers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "beers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "brew_date",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "ibu",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "ingredients",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "recipe",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "secondary_date",
                table: "beers");

            migrationBuilder.DropColumn(
                name: "url",
                table: "beers");
        }
    }
}
