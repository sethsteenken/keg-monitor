using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class BeerPourAndScaleDeviceInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "endpoint",
                table: "scales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "topic",
                table: "scales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE scales SET endpoint = CONCAT('kegscale', id, '.local')");
            migrationBuilder.Sql("UPDATE scales SET topic = CONCAT('tele/scale', id, '/SENSOR')");

            migrationBuilder.CreateTable(
                name: "beer_pours",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    beerid = table.Column<int>(name: "beer_id", type: "integer", nullable: false),
                    scaleid = table.Column<int>(name: "scale_id", type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(name: "time_stamp", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beer_pours", x => x.id);
                    table.ForeignKey(
                        name: "FK_beer_pours_beers_beer_id",
                        column: x => x.beerid,
                        principalTable: "beers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_beer_pours_scales_scale_id",
                        column: x => x.scaleid,
                        principalTable: "scales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_beer_pours_beer_id",
                table: "beer_pours",
                column: "beer_id");

            migrationBuilder.CreateIndex(
                name: "IX_beer_pours_scale_id",
                table: "beer_pours",
                column: "scale_id");

            migrationBuilder.Sql(@"INSERT INTO beer_pours (beer_id, scale_id, time_stamp)
                                   SELECT beer_id, scale_id, time_stamp
                                   FROM scale_weight_changes
                                   WHERE is_pour_event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "beer_pours");

            migrationBuilder.DropColumn(
                name: "endpoint",
                table: "scales");

            migrationBuilder.DropColumn(
                name: "topic",
                table: "scales");
        }
    }
}
