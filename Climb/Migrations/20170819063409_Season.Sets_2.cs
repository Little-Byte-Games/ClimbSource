using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SeasonSets_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_League_User_AdminID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_League_Game_GameID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_League_LeagueID",
                table: "LeagueUser");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_Season_SeasonID",
                table: "LeagueUser");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_User_UserID",
                table: "LeagueUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Set_SetID",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_RankEvent_League_LeagueID",
                table: "RankEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_RankEvent_Set_SetID",
                table: "RankEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Season_League_LeagueID",
                table: "Season");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player1ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player2ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_Season_SeasonID",
                table: "Set");

            migrationBuilder.AddForeignKey(
                name: "FK_League_User_AdminID",
                table: "League",
                column: "AdminID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_League_Game_GameID",
                table: "League",
                column: "GameID",
                principalTable: "Game",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_League_LeagueID",
                table: "LeagueUser",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_Season_SeasonID",
                table: "LeagueUser",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_User_UserID",
                table: "LeagueUser",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Set_SetID",
                table: "Match",
                column: "SetID",
                principalTable: "Set",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RankEvent_League_LeagueID",
                table: "RankEvent",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RankEvent_Set_SetID",
                table: "RankEvent",
                column: "SetID",
                principalTable: "Set",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Season_League_LeagueID",
                table: "Season",
                column: "LeagueID",
                principalTable: "League",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Set_Season_SeasonID",
                table: "Set",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_League_User_AdminID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_League_Game_GameID",
                table: "League");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_League_LeagueID",
                table: "LeagueUser");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_Season_SeasonID",
                table: "LeagueUser");

            migrationBuilder.DropForeignKey(
                name: "FK_LeagueUser_User_UserID",
                table: "LeagueUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Set_SetID",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_RankEvent_League_LeagueID",
                table: "RankEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_RankEvent_Set_SetID",
                table: "RankEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Season_League_LeagueID",
                table: "Season");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player1ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_User_Player2ID",
                table: "Set");

            migrationBuilder.DropForeignKey(
                name: "FK_Set_Season_SeasonID",
                table: "Set");

            migrationBuilder.AddForeignKey(
                name: "FK_League_User_AdminID",
                table: "League",
                column: "AdminID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_League_Game_GameID",
                table: "League",
                column: "GameID",
                principalTable: "Game",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_League_LeagueID",
                table: "LeagueUser",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_Season_SeasonID",
                table: "LeagueUser",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_LeagueUser_User_UserID",
                table: "LeagueUser",
                column: "UserID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Set_SetID",
                table: "Match",
                column: "SetID",
                principalTable: "Set",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RankEvent_League_LeagueID",
                table: "RankEvent",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RankEvent_Set_SetID",
                table: "RankEvent",
                column: "SetID",
                principalTable: "Set",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Season_League_LeagueID",
                table: "Season",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Set_User_Player1ID",
                table: "Set",
                column: "Player1ID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Set_User_Player2ID",
                table: "Set",
                column: "Player2ID",
                principalTable: "User",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Set_Season_SeasonID",
                table: "Set",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
