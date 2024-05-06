using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenisRankingDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AddMixedDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenisMatchType",
                table: "Tournaments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MixedDoubleElo",
                table: "Players",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenisMatchType",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "MixedDoubleElo",
                table: "Players");
        }
    }
}
