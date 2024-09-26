using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using DreamTeamDB.Context;
using DreamTeamDB.Models;

namespace DreamTeam.Tests
{
    public class HackathonDatabaseTests : IDisposable
    {
        private readonly HackathonContext context;
        private readonly SqliteConnection connection;

        public HackathonDatabaseTests()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<HackathonContext>()
                .UseSqlite(connection)
                .Options;

            context = new HackathonContext(options);

            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            context.Dispose();
            connection.Close();
        }

        [Fact]
        public void CanWriteHackathonToDatabase()
        {
            var teamLeads = new List<Employee>
            {
                new() { Name = "TeamLead1" },
                new() { Name = "TeamLead2" }
            };

            var juniors = new List<Employee>
            {
                new() { Name = "Junior1" },
                new() { Name = "Junior2" }
            };

            var hackathon = new Hackathon(teamLeads, juniors);

            context.Hackathons.Add(hackathon);
            context.SaveChanges();

            var savedHackathon = context.Hackathons
                .Include(h => h.TeamLeads)
                .Include(h => h.Juniors)
                .FirstOrDefault();

            Assert.NotNull(savedHackathon);
            Assert.Equal(2, savedHackathon.TeamLeads.Count);
            Assert.Equal(2, savedHackathon.Juniors.Count);

            Assert.Equal(hackathon.HackathonId, savedHackathon.HackathonId);
            Assert.Equal("TeamLead1", savedHackathon.TeamLeads.First().Name);
            Assert.Equal("Junior1", savedHackathon.Juniors.First().Name);
        }
    }
}
