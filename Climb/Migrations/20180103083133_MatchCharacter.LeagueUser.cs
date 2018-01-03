using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class MatchCharacterLeagueUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MatchCharacters_LeagueUserID",
                table: "MatchCharacters",
                column: "LeagueUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchCharacters_LeagueUser_LeagueUserID",
                table: "MatchCharacters",
                column: "LeagueUserID",
                principalTable: "LeagueUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchCharacters_LeagueUser_LeagueUserID",
                table: "MatchCharacters");

            migrationBuilder.DropIndex(
                name: "IX_MatchCharacters_LeagueUserID",
                table: "MatchCharacters");
        }
    }
}
