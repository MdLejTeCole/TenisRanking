﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TenisRankingDatabase;

#nullable disable

namespace TenisRankingDatabase.Migrations
{
    [DbContext(typeof(TenisRankingDbContext))]
    partial class TenisRankingDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("TenisRankingDatabase.Tables.Match", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MatchResult")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MatchWinnerResult")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Round")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TournamentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TournamentId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Player", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Elo")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LostMatches")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nick")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Tournament1Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tournament2Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Tournament3Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TournamentsPlayed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TournamentsPoints")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinMatches")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Nick")
                        .IsUnique();

                    b.ToTable("Players");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.PlayerMatch", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Elo")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GrantedElo")
                        .HasColumnType("INTEGER");

                    b.Property<long>("MatchId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MatchPoint")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Set1")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Set2")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Set3")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Set4")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Set5")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TieBreak")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinnerResult")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("WonGames")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("WonSets")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("PlayerId", "MatchId")
                        .IsUnique();

                    b.ToTable("PlayerMatches");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Setting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExtraPoints1Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExtraPoints2Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExtraPoints3Place")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ExtraPointsForTournamentWon")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumberOfSets")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StartElo")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("TieBreak")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Tournament", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AvarageElo")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Ended")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExtraPoints1Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExtraPoints2Place")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExtraPoints3Place")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ExtraPointsForTournamentWon")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("NumberOfSets")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.TournamentPlayer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Place")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TournamentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("TournamentId", "PlayerId")
                        .IsUnique();

                    b.ToTable("TournamentPlayers");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Match", b =>
                {
                    b.HasOne("TenisRankingDatabase.Tables.Tournament", "Tournament")
                        .WithMany("Matches")
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.PlayerMatch", b =>
                {
                    b.HasOne("TenisRankingDatabase.Tables.Match", "Match")
                        .WithMany("PlayerMatches")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TenisRankingDatabase.Tables.Player", "Player")
                        .WithMany("PlayerMatches")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Match");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.TournamentPlayer", b =>
                {
                    b.HasOne("TenisRankingDatabase.Tables.Player", "Player")
                        .WithMany("TournamentPlayers")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TenisRankingDatabase.Tables.Tournament", "Tournament")
                        .WithMany("TournamentPlayers")
                        .HasForeignKey("TournamentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Tournament");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Match", b =>
                {
                    b.Navigation("PlayerMatches");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Player", b =>
                {
                    b.Navigation("PlayerMatches");

                    b.Navigation("TournamentPlayers");
                });

            modelBuilder.Entity("TenisRankingDatabase.Tables.Tournament", b =>
                {
                    b.Navigation("Matches");

                    b.Navigation("TournamentPlayers");
                });
#pragma warning restore 612, 618
        }
    }
}
