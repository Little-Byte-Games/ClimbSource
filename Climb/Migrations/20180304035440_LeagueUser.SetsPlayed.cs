using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class LeagueUserSetsPlayed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "LeagueUser");

            migrationBuilder.AddColumn<int>(
                name: "SetsPlayed",
                table: "LeagueUser",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetsPlayed",
                table: "LeagueUser");

            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "LeagueUser",
                nullable: false,
                defaultValue: false);
        }
    }
}
