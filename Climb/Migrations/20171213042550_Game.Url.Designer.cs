﻿// <auto-generated />
using Climb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Climb.Migrations
{
    [DbContext(typeof(ClimbContext))]
    [Migration("20171213042550_Game.Url")]
    partial class GameUrl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Climb.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<int>("UserID");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("UserID")
                        .IsUnique();

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Climb.Models.Character", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GameID");

                    b.Property<string>("Name");

                    b.Property<string>("PicKey");

                    b.HasKey("ID");

                    b.HasIndex("GameID");

                    b.ToTable("Character");
                });

            modelBuilder.Entity("Climb.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("ID");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("Climb.Models.League", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdminID");

                    b.Property<int>("GameID");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("AdminID");

                    b.HasIndex("GameID");

                    b.ToTable("League");
                });

            modelBuilder.Entity("Climb.Models.LeagueUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Elo");

                    b.Property<bool>("HasLeft");

                    b.Property<int>("LeagueID");

                    b.Property<string>("ProfilePicKey");

                    b.Property<int>("Rank");

                    b.Property<string>("SlackUsername");

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("UserID");

                    b.ToTable("LeagueUser");
                });

            modelBuilder.Entity("Climb.Models.LeagueUserSeason", b =>
                {
                    b.Property<int>("LeagueUserID");

                    b.Property<int>("SeasonID");

                    b.Property<int>("Points");

                    b.Property<int>("Standing");

                    b.HasKey("LeagueUserID", "SeasonID");

                    b.HasIndex("SeasonID");

                    b.ToTable("LeagueUserSeason");
                });

            modelBuilder.Entity("Climb.Models.Match", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Index");

                    b.Property<int?>("Player1CharacterID");

                    b.Property<int>("Player1Score");

                    b.Property<int?>("Player2CharacterID");

                    b.Property<int>("Player2Score");

                    b.Property<int?>("SetID");

                    b.Property<int?>("StageID");

                    b.HasKey("ID");

                    b.HasIndex("Player1CharacterID");

                    b.HasIndex("Player2CharacterID");

                    b.HasIndex("SetID");

                    b.HasIndex("StageID");

                    b.ToTable("Match");
                });

            modelBuilder.Entity("Climb.Models.RankSnapshot", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("DeltaElo");

                    b.Property<int>("DeltaRank");

                    b.Property<int>("Elo");

                    b.Property<int>("LeagueUserID");

                    b.Property<int>("Rank");

                    b.HasKey("ID");

                    b.HasIndex("LeagueUserID");

                    b.ToTable("RankSnapshot");
                });

            modelBuilder.Entity("Climb.Models.Season", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Index");

                    b.Property<int>("LeagueID");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.ToTable("Season");
                });

            modelBuilder.Entity("Climb.Models.Set", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DueDate");

                    b.Property<bool>("IsLocked");

                    b.Property<int>("LeagueID");

                    b.Property<int?>("Player1ID");

                    b.Property<int?>("Player1Score");

                    b.Property<int?>("Player2ID");

                    b.Property<int?>("Player2Score");

                    b.Property<int?>("SeasonID");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("Player1ID");

                    b.HasIndex("Player2ID");

                    b.HasIndex("SeasonID");

                    b.ToTable("Set");
                });

            modelBuilder.Entity("Climb.Models.Stage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GameID");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("GameID");

                    b.ToTable("Stage");
                });

            modelBuilder.Entity("Climb.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Username");

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Climb.Models.ApplicationUser", b =>
                {
                    b.HasOne("Climb.Models.User", "User")
                        .WithOne("ApplicationUser")
                        .HasForeignKey("Climb.Models.ApplicationUser", "UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Character", b =>
                {
                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany("Characters")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.League", b =>
                {
                    b.HasOne("Climb.Models.User", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany("Leagues")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.LeagueUser", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany("Members")
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.User", "User")
                        .WithMany("LeagueUsers")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.LeagueUserSeason", b =>
                {
                    b.HasOne("Climb.Models.LeagueUser", "LeagueUser")
                        .WithMany("Seasons")
                        .HasForeignKey("LeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Season", "Season")
                        .WithMany("Participants")
                        .HasForeignKey("SeasonID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Match", b =>
                {
                    b.HasOne("Climb.Models.Character", "Player1Character")
                        .WithMany()
                        .HasForeignKey("Player1CharacterID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Character", "Player2Character")
                        .WithMany()
                        .HasForeignKey("Player2CharacterID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Set", "Set")
                        .WithMany("Matches")
                        .HasForeignKey("SetID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Stage", "Stage")
                        .WithMany()
                        .HasForeignKey("StageID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.RankSnapshot", b =>
                {
                    b.HasOne("Climb.Models.LeagueUser", "LeagueUser")
                        .WithMany("RankSnapshots")
                        .HasForeignKey("LeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Season", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany("Seasons")
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Set", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany("Sets")
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.LeagueUser", "Player1")
                        .WithMany("P1Sets")
                        .HasForeignKey("Player1ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.LeagueUser", "Player2")
                        .WithMany("P2Sets")
                        .HasForeignKey("Player2ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Season", "Season")
                        .WithMany("Sets")
                        .HasForeignKey("SeasonID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Stage", b =>
                {
                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany("Stages")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
