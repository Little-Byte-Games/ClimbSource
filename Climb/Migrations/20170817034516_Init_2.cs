using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class Init_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_League_Game_GameID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_League_User_UserID",
                table: "League");

            migrationBuilder.AddColumn<int>(
                name: "Player1ID",
                table: "Set",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2ID",
                table: "Set",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Set_Player1ID",
                table: "Set",
                column: "Player1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Set_Player2ID",
                table: "Set",
                column: "Player2ID");

            migrationBuilder.AddForeignKey(
                name: "FK_League_Game_GameID",
                table: "League",
                column: "GameID",
                principalTable: "Game",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_League_User_UserID",
                table: "League",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_League_Game_GameID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_League_User_UserID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player1ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player2ID",
                table: "Set");

            migrationBuilder.DropIndex(
                name: "IX_Set_Player1ID",
                table: "Set");

            migrationBuilder.DropIndex(
                name: "IX_Set_Player2ID",
                table: "Set");

            migrationBuilder.DropColumn(
                name: "Player1ID",
                table: "Set");

            migrationBuilder.DropColumn(
                name: "Player2ID",
                table: "Set");

            migrationBuilder.AddForeignKey(
                name: "FK_League_Game_GameID",
                table: "League",
                column: "GameID",
                principalTable: "Game",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_League_User_UserID",
                table: "League",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
