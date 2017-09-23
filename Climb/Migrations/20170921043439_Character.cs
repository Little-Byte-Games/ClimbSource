using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class Character : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Player1CharacterID",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Player2CharacterID",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GameID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Character_Game_GameID",
                        column: x => x.GameID,
                        principalTable: "Game",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_Player1CharacterID",
                table: "Match",
                column: "Player1CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_Player2CharacterID",
                table: "Match",
                column: "Player2CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_Character_GameID",
                table: "Character",
                column: "GameID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_Character_Player1CharacterID",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Character_Player2CharacterID",
                table: "Match");

            migrationBuilder.DropTable(
                name: "Character");

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
        }
    }
}
