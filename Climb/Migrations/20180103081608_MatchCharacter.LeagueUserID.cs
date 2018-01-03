using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class MatchCharacterLeagueUserID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchCharacters",
                table: "MatchCharacters");

            migrationBuilder.DropIndex(
                name: "IX_MatchCharacters_MatchID",
                table: "MatchCharacters");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "MatchCharacters");

            migrationBuilder.RenameColumn(
                name: "PlayerIndex",
                table: "MatchCharacters",
                newName: "LeagueUserID");

            migrationBuilder.AlterColumn<int>(
                name: "SetID",
                table: "Match",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchCharacters",
                table: "MatchCharacters",
                columns: new[] { "MatchID", "LeagueUserID", "CharacterID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchCharacters",
                table: "MatchCharacters");

            migrationBuilder.RenameColumn(
                name: "LeagueUserID",
                table: "MatchCharacters",
                newName: "PlayerIndex");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "MatchCharacters",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<int>(
                name: "SetID",
                table: "Match",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchCharacters",
                table: "MatchCharacters",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_MatchCharacters_MatchID",
                table: "MatchCharacters",
                column: "MatchID");
        }
    }
}
