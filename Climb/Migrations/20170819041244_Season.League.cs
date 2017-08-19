using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SeasonLeague : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeagueID",
                table: "Season",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Season_LeagueID",
                table: "Season",
                column: "LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Season_League_LeagueID",
                table: "Season",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Season_League_LeagueID",
                table: "Season");

            migrationBuilder.DropIndex(
                name: "IX_Season_LeagueID",
                table: "Season");

            migrationBuilder.DropColumn(
                name: "LeagueID",
                table: "Season");
        }
    }
}
