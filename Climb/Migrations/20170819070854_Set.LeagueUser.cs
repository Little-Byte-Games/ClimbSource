using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SetLeagueUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player1ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player2ID",
                table: "Set");

            migrationBuilder.AddForeignKey(
                name: "FK_Set_LeagueUser_Player1ID",
                table: "Set",
                column: "Player1ID",
                principalTable: "LeagueUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Set_LeagueUser_Player2ID",
                table: "Set",
                column: "Player2ID",
                principalTable: "LeagueUser",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Set_LeagueUser_Player1ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_LeagueUser_Player2ID",
                table: "Set");

            migrationBuilder.AddForeignKey(
                name: "FK_Set_User_Player1ID",
                table: "Set",
                column: "Player1ID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Set_User_Player2ID",
                table: "Set",
                column: "Player2ID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
