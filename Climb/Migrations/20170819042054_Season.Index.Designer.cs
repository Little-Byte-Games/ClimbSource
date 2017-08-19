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
    [Migration("20170819042054_Season.Index")]
    partial class SeasonIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Climb.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

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

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("UserID");

                    b.ToTable("LeagueUser");
                });

            modelBuilder.Entity("Climb.Models.Match", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Player1Score");

                    b.Property<int>("Player2Score");

                    b.Property<int?>("SetID");

                    b.HasKey("ID");

                    b.HasIndex("SetID");

                    b.ToTable("Match");
                });

            modelBuilder.Entity("Climb.Models.RankEvent", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Elo");

                    b.Property<int>("LeagueID");

                    b.Property<int>("Rank");

                    b.Property<int>("SetID");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("SetID");

                    b.ToTable("RankEvent");
                });

            modelBuilder.Entity("Climb.Models.Season", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Index");

                    b.Property<int>("LeagueID");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.ToTable("Season");
                });

            modelBuilder.Entity("Climb.Models.Set", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Player1ID");

                    b.Property<int>("Player2ID");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("ID");

                    b.HasIndex("Player1ID");

                    b.HasIndex("Player2ID");

                    b.ToTable("Set");
                });

            modelBuilder.Entity("Climb.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Username");

                    b.HasKey("ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Climb.Models.League", b =>
                {
                    b.HasOne("Climb.Models.User", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany()
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

            modelBuilder.Entity("Climb.Models.Match", b =>
                {
                    b.HasOne("Climb.Models.Set", "Set")
                        .WithMany("Matches")
                        .HasForeignKey("SetID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.RankEvent", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany()
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Set", "Set")
                        .WithMany()
                        .HasForeignKey("SetID")
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
                    b.HasOne("Climb.Models.User", "Player1")
                        .WithMany("P1Sets")
                        .HasForeignKey("Player1ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.User", "Player2")
                        .WithMany("P2Sets")
                        .HasForeignKey("Player2ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
