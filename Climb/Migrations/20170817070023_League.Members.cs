using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class LeagueMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeagueID",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_LeagueID",
                table: "User",
                column: "LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_League_LeagueID",
                table: "User",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_League_LeagueID",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_LeagueID",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LeagueID",
                table: "User");
        }
    }
}
