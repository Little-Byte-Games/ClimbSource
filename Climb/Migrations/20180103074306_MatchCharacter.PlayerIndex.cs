using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class MatchCharacterPlayerIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchCharacters",
                table: "MatchCharacters");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "MatchCharacters",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "PlayerIndex",
                table: "MatchCharacters",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "PlayerIndex",
                table: "MatchCharacters");

            migrationBuilder.AlterColumn<int>(
                name: "SetID",
                table: "Match",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchCharacters",
                table: "MatchCharacters",
                columns: new[] { "MatchID", "CharacterID" });
        }
    }
}
