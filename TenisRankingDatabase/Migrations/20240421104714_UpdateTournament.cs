using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenisRankingDatabase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTournament : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TournamentStatus",
                table: "Tournaments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentStatus",
                table: "Tournaments");
        }
    }
}
