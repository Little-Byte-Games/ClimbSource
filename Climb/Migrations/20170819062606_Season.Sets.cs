using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SeasonSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeasonID",
                table: "Set",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Set_SeasonID",
                table: "Set",
                column: "SeasonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Set_Season_SeasonID",
                table: "Set",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Set_Season_SeasonID",
                table: "Set");

            migrationBuilder.DropIndex(
                name: "IX_Set_SeasonID",
                table: "Set");

            migrationBuilder.DropColumn(
                name: "SeasonID",
                table: "Set");
        }
    }
}
