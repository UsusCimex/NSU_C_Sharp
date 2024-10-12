using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using DreamTeamDB.Context;
using DreamTeamDB.Models;
using DreamTeam.Tests.DbContext;
using DreamTeamDB.Strategy;

namespace DreamTeam.Tests
{
    public class HackathonDatabaseTests
    {
        [Fact]
        public void TestWriteEventInformation()
        {
            using var context = DbContextInMemoryFactory.CreateContext();
            var hackathon = new Hackathon
            {
                TeamLeadIds = [1, 2, 3],
                JuniorIds = [4, 5, 6],
                HarmonyScore = 0.0
            };

            context.Hackathons.Add(hackathon);
            context.SaveChanges();

            var savedHackathon = context.Hackathons.FirstOrDefault();
            Assert.NotNull(savedHackathon);
            Assert.Equal(3, savedHackathon.TeamLeadIds.Count);
            Assert.Equal(3, savedHackathon.JuniorIds.Count);
        }

        [Fact]
        public void TestReadEventInformation()
        {
            using var context = DbContextInMemoryFactory.CreateContext();
            var hackathon = new Hackathon
            {
                TeamLeadIds = [1, 2, 3],
                JuniorIds = [4, 5, 6],
                HarmonyScore = 0.0
            };
            context.Hackathons.Add(hackathon);
            context.SaveChanges();

            var retrievedHackathon = context.Hackathons.FirstOrDefault(h => h.HackathonId == hackathon.HackathonId);

            Assert.NotNull(retrievedHackathon);
            Assert.Equal(hackathon.HackathonId, retrievedHackathon.HackathonId);
            Assert.Equal(hackathon.TeamLeadIds, retrievedHackathon.TeamLeadIds);
            Assert.Equal(hackathon.JuniorIds, retrievedHackathon.JuniorIds);
        }
        
        [Fact]
        public void TestCalculateAndWriteAverageHarmonyScore()
        {
            using var context = DbContextInMemoryFactory.CreateContext();

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

            context.Employees.AddRange(teamLeads);
            context.Employees.AddRange(juniors);
            context.SaveChanges();

            var hackathon = new Hackathon
            {
                TeamLeadIds = teamLeads.Select(tl => tl.Id).ToList(),
                JuniorIds = juniors.Select(jr => jr.Id).ToList()
            };
            context.Hackathons.Add(hackathon);
            context.SaveChanges();

            foreach (var teamLead in teamLeads)
            {
                var wishlist = teamLead.GenerateWishlist(juniors);
                context.Wishlists.Add(wishlist);
            }

            foreach (var junior in juniors)
            {
                var wishlist = junior.GenerateWishlist(teamLeads);
                context.Wishlists.Add(wishlist);
            }

            context.SaveChanges();

            var hrManager = new HrManager(context);

            var teamLeadWishlistIds = context.Wishlists
                .Where(w => teamLeads.Select(tl => tl.Id).Contains(w.EmployeeId))
                .Select(w => w.WishlistId).ToList();

            var juniorWishlistIds = context.Wishlists
                .Where(w => juniors.Select(jr => jr.Id).Contains(w.EmployeeId))
                .Select(w => w.WishlistId).ToList();

            foreach (var wishlistId in teamLeadWishlistIds)
            {
                hrManager.ReceiveWishlist(wishlistId, "TeamLead");
            }

            foreach (var wishlistId in juniorWishlistIds)
            {
                hrManager.ReceiveWishlist(wishlistId, "Junior");
            }

            ITeamBuildingStrategy strategy = new RandomTeamBuildingStrategy();
            var teams = hrManager.GenerateTeams(strategy, hackathon.HackathonId);

            foreach (var team in teams)
            {
                team.HackathonId = hackathon.HackathonId;
                context.Teams.Add(team);
            }

            context.SaveChanges();

            var hrDirector = new HrDirector(context);
            hrManager.SendTeamsToHrDirector(teams.Select(t => t.TeamId).ToList(), hrDirector);

            hackathon.HarmonyScore = hrDirector.CalculateTeamsScore();
            context.SaveChanges();

            var averageHarmony = context.Hackathons.Average(h => h.HarmonyScore);

            Assert.True(hackathon.HarmonyScore > 0);
            Assert.Equal(hackathon.HarmonyScore, averageHarmony);
        }
    }
}
