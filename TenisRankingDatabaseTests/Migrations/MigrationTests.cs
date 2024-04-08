using Dapper;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using TenisRankingDatabase;

namespace ItBuildsXUnitTests.Migrations;

[TestCaseOrderer("TestingEntityFramework.MigrationTestOrderer", "TestingEntityFramework")]
public class MigrationTests : IClassFixture<MigratingDbFixture>
{
    private MigratingDbFixture _databaseFixture;
    private TenisRankingDbContext _dbContext;
    private string _connectionString = "Data Source=TenisRankingTest.db";

    public MigrationTests(MigratingDbFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        _dbContext = _databaseFixture.DatabaseContext;
    }

    public MigratingDbFixture DatabaseFixture => _databaseFixture;

    [Fact, MigrationTest(0)]
    public async Task MigrationTest() 
    {
        await TestInitialMigration();

    }

    private async Task TestInitialMigration()
    {
        await DatabaseFixture.SetMigration("Init");
        DatabaseFixture.RunScript(@"..\..\..\Migrations\Scripts\Init.sql");
        CheckInitData();
    }

    private void CheckInitData()
    {
        var players = _dbContext.Players
            .Include(x => x.PlayerMatches)
            .Include(x => x.TournamentPlayers)
            .ToList();
        _ = players.Count.Should().Be(6);
        _ = players[0].PlayerMatches.Count.Should().Be(5);
        _ = players[1].PlayerMatches.Count.Should().Be(5);
        _ = players[2].PlayerMatches.Count.Should().Be(5);
        _ = players[3].PlayerMatches.Count.Should().Be(5);
        _ = players[4].PlayerMatches.Count.Should().Be(3);
        _ = players[5].PlayerMatches.Count.Should().Be(3);
        _ = players[0].TournamentPlayers.Count.Should().Be(2);
        _ = players[1].TournamentPlayers.Count.Should().Be(2);
        _ = players[2].TournamentPlayers.Count.Should().Be(2);
        _ = players[3].TournamentPlayers.Count.Should().Be(2);
        _ = players[4].TournamentPlayers.Count.Should().Be(1);
        _ = players[5].TournamentPlayers.Count.Should().Be(1);

        _dbContext.Settings.Count().Should().Be(1);
        var turnamets = _dbContext.Tournaments
            .Include(x => x.TournamentPlayers)
            .Include(x => x.Matches)
            .ToList();
        _ = turnamets.Count.Should().Be(2);
        _ = turnamets[0].TournamentPlayers.Count.Should().Be(6);
        _ = turnamets[1].TournamentPlayers.Count.Should().Be(4);
        _ = turnamets[0].Matches.Count.Should().Be(9);
        _ = turnamets[1].Matches.Count.Should().Be(4);

        _dbContext.TournamentPlayers.Count().Should().Be(10);

        _dbContext.Matches.Count().Should().Be(13);
    }
}
