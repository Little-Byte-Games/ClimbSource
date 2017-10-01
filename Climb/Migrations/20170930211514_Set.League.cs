using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SetLeague : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeagueID",
                table: "Set",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Set_LeagueID",
                table: "Set",
                column: "LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_Set_League_LeagueID",
                table: "Set",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Set_League_LeagueID",
                table: "Set");

            migrationBuilder.DropIndex(
                name: "IX_Set_LeagueID",
                table: "Set");

            migrationBuilder.DropColumn(
                name: "LeagueID",
                table: "Set");
        }
    }
}
