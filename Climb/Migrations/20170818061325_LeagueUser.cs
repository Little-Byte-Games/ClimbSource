using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class LeagueUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_League_LeagueID",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_LeagueID",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LeagueID",
                table: "User");

            migrationBuilder.CreateTable(
                name: "LeagueUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Elo = table.Column<int>(type: "int", nullable: false),
                    LeagueID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_LeagueUser_League_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "League",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeagueUser_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUser_LeagueID",
                table: "LeagueUser",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUser_UserID",
                table: "LeagueUser",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeagueUser");

            migrationBuilder.AddColumn<int>(
                name: "LeagueID",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_LeagueID",
                table: "User",
                column: "LeagueID");

            migrationBuilder.AddForeignKey(
                name: "FK_User_League_LeagueID",
                table: "User",
                column: "LeagueID",
                principalTable: "League",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
