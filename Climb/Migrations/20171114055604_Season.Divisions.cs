using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class SeasonDivisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "Division",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Division_SeasonID",
                table: "Division",
                column: "SeasonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Division_Season_SeasonID",
                table: "Division",
                column: "SeasonID",
                principalTable: "Season",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Division_Season_SeasonID",
                table: "Division");

            migrationBuilder.DropIndex(
                name: "IX_Division_SeasonID",
                table: "Division");

            migrationBuilder.DropColumn(
                name: "Index",
                table: "Division");
        }
    }
}
