using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class MatchCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_Character_Player1CharacterID",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Character_Player2CharacterID",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_Player1CharacterID",
                table: "Match");

            migrationBuilder.DropIndex(
                name: "IX_Match_Player2CharacterID",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "Player1CharacterID",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "Player2CharacterID",
                table: "Match");

            migrationBuilder.AddColumn<int>(
                name: "KingID",
                table: "League",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "KingReignStart",
                table: "League",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "MatchCharacters",
                columns: table => new
                {
                    MatchID = table.Column<int>(nullable: false),
                    CharacterID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchCharacters", x => new { x.MatchID, x.CharacterID });
                    table.ForeignKey(
                        name: "FK_MatchCharacters_Character_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Character",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchCharacters_Match_MatchID",
                        column: x => x.MatchID,
                        principalTable: "Match",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchCharacters_CharacterID",
                table: "MatchCharacters",
                column: "CharacterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchCharacters");

            migrationBuilder.DropColumn(
                name: "KingID",
                table: "League");

            migrationBuilder.DropColumn(
                name: "KingReignStart",
                table: "League");

            migrationBuilder.AddColumn<int>(
                name: "Player1CharacterID",
                table: "Match",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Player2CharacterID",
                table: "Match",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Match_Player1CharacterID",
                table: "Match",
                column: "Player1CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_Player2CharacterID",
                table: "Match",
                column: "Player2CharacterID");

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Character_Player1CharacterID",
                table: "Match",
                column: "Player1CharacterID",
                principalTable: "Character",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Character_Player2CharacterID",
                table: "Match",
                column: "Player2CharacterID",
                principalTable: "Character",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
