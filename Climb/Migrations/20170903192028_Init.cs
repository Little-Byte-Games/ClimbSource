using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Climb.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "League",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdminID = table.Column<int>(type: "int", nullable: false),
                    GameID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_League", x => x.ID);
                    table.ForeignKey(
                        name: "FK_League_User_AdminID",
                        column: x => x.AdminID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_League_Game_GameID",
                        column: x => x.GameID,
                        principalTable: "Game",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Index = table.Column<int>(type: "int", nullable: false),
                    LeagueID = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Season_League_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "League",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeagueUser",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Elo = table.Column<int>(type: "int", nullable: false),
                    HasLeft = table.Column<bool>(type: "bit", nullable: false),
                    LeagueID = table.Column<int>(type: "int", nullable: false),
                    ProfilePicKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeasonID = table.Column<int>(type: "int", nullable: true),
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
                        name: "FK_LeagueUser_Season_SeasonID",
                        column: x => x.SeasonID,
                        principalTable: "Season",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeagueUser_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Set",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Player1ID = table.Column<int>(type: "int", nullable: true),
                    Player2ID = table.Column<int>(type: "int", nullable: true),
                    SeasonID = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Set", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Set_LeagueUser_Player1ID",
                        column: x => x.Player1ID,
                        principalTable: "LeagueUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Set_LeagueUser_Player2ID",
                        column: x => x.Player2ID,
                        principalTable: "LeagueUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Set_Season_SeasonID",
                        column: x => x.SeasonID,
                        principalTable: "Season",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Index = table.Column<int>(type: "int", nullable: false),
                    Player1Score = table.Column<int>(type: "int", nullable: false),
                    Player2Score = table.Column<int>(type: "int", nullable: false),
                    SetID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Match_Set_SetID",
                        column: x => x.SetID,
                        principalTable: "Set",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RankEvent",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Elo = table.Column<int>(type: "int", nullable: false),
                    LeagueID = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    SetID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankEvent", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RankEvent_League_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "League",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RankEvent_Set_SetID",
                        column: x => x.SetID,
                        principalTable: "Set",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_League_AdminID",
                table: "League",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_League_GameID",
                table: "League",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUser_LeagueID",
                table: "LeagueUser",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUser_SeasonID",
                table: "LeagueUser",
                column: "SeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_LeagueUser_UserID",
                table: "LeagueUser",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Match_SetID",
                table: "Match",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_RankEvent_LeagueID",
                table: "RankEvent",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_RankEvent_SetID",
                table: "RankEvent",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Season_LeagueID",
                table: "Season",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_Set_Player1ID",
                table: "Set",
                column: "Player1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Set_Player2ID",
                table: "Set",
                column: "Player2ID");

            migrationBuilder.CreateIndex(
                name: "IX_Set_SeasonID",
                table: "Set",
                column: "SeasonID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "RankEvent");

            migrationBuilder.DropTable(
                name: "Set");

            migrationBuilder.DropTable(
                name: "LeagueUser");

            migrationBuilder.DropTable(
                name: "Season");

            migrationBuilder.DropTable(
                name: "League");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Game");
        }
    }
}
