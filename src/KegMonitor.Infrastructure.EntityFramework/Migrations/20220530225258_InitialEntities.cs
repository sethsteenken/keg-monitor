using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    public partial class InitialEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "beers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    abv = table.Column<decimal>(type: "numeric", nullable: false),
                    last_updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "scales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    current_weight = table.Column<int>(type: "integer", nullable: false),
                    full_weight = table.Column<int>(type: "integer", nullable: false),
                    empty_weight = table.Column<int>(type: "integer", nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    beer_id = table.Column<int>(type: "integer", nullable: true),
                    last_updated_dated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scales", x => x.id);
                    table.ForeignKey(
                        name: "FK_scales_beers_beer_id",
                        column: x => x.beer_id,
                        principalTable: "beers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "scale_weight_changes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    scale_id = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false),
                    time_stamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scale_weight_changes", x => x.id);
                    table.ForeignKey(
                        name: "FK_scale_weight_changes_scales_scale_id",
                        column: x => x.scale_id,
                        principalTable: "scales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_scale_weight_changes_scale_id",
                table: "scale_weight_changes",
                column: "scale_id");

            migrationBuilder.CreateIndex(
                name: "IX_scales_beer_id",
                table: "scales",
                column: "beer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "scale_weight_changes");

            migrationBuilder.DropTable(
                name: "scales");

            migrationBuilder.DropTable(
                name: "beers");
        }
    }
}
