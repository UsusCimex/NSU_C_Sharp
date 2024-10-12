using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using DreamTeamDB.Context;

namespace DreamTeam.Tests.DbContext {
    public class DbContextInMemoryFactory
    {
        public static HackathonContext CreateContext()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<HackathonContext>()
                .UseSqlite(connection)
                .Options;

            var context = new HackathonContext(options);
            context.Database.EnsureCreated();

            return context;
        }
    }
}