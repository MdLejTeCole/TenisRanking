﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenisRankingDatabase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StartMixedDoubleElo",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartMixedDoubleElo",
                table: "Settings");
        }
    }
}
