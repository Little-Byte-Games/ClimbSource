using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class EloToPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Elo",
                table: "RankSnapshot",
                newName: "Points");

            migrationBuilder.RenameColumn(
                name: "DeltaElo",
                table: "RankSnapshot",
                newName: "DeltaPoints");

            migrationBuilder.RenameColumn(
                name: "Elo",
                table: "LeagueUser",
                newName: "Points");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "League",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "RankSnapshot",
                newName: "Elo");

            migrationBuilder.RenameColumn(
                name: "DeltaPoints",
                table: "RankSnapshot",
                newName: "DeltaElo");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "LeagueUser",
                newName: "Elo");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "League",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30);
        }
    }
}
