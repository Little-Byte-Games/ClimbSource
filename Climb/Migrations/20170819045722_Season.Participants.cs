using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SeasonParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonID",
                table: "LeagueUser",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUser_SeasonID",
                table: "LeagueUser",
                column: "SeasonID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_Season_SeasonID",
                table: "LeagueUser",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_Season_SeasonID",
                table: "LeagueUser");

            migrationBuilder.DropIndex(
                name: "IX_LeagueUser_SeasonID",
                table: "LeagueUser");

            migrationBuilder.DropColumn(
                name: "SeasonID",
                table: "LeagueUser");
        }
    }
}
