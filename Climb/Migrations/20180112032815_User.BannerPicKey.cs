using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class UserBannerPicKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerPicKey",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "LeagueUser",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerPicKey",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "LeagueUser");
        }
    }
}
