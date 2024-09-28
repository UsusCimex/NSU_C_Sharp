using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DreamTeamDB.Context;
using DreamTeamDB.Models;
using DreamTeamDB.Strategy;

namespace DreamTeamDB
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<HackathonContext>(options =>
                    options.UseSqlite("Data Source=hackathons.db"))
                .AddSingleton<HrManager>()
                .AddSingleton<HrDirector>()
                .BuildServiceProvider();

            using var context = serviceProvider.GetRequiredService<HackathonContext>();
            context.Database.EnsureCreated();

            var teamLeadNames = DreamTeamConsole.Models.CsvLoader.LoadEmployeeNamesFromCsv("../csvFiles/Juniors20.csv");
            var teamLeads = new List<Employee>();
            foreach (var teamLeadName in teamLeadNames) 
            {
                teamLeads.Add(new Employee() {Name = teamLeadName});
            }

            var juniorNames = DreamTeamConsole.Models.CsvLoader.LoadEmployeeNamesFromCsv("../csvFiles/Teamleads20.csv");
            var juniors = new List<Employee>();
            foreach (var juniorName in juniorNames)
            {
                juniors.Add(new Employee() {Name = juniorName});
            }

            context.Employees.AddRange(teamLeads);
            context.Employees.AddRange(juniors);
            context.SaveChanges();

            var hackathon = new Hackathon(teamLeads, juniors);
            context.Hackathons.Add(hackathon);
            context.SaveChanges();

            var hrManager = serviceProvider.GetRequiredService<HrManager>();

            foreach (var tl in teamLeads)
            {
                var wishlist = tl.GenerateWishlist(juniors);
                tl.SendWishlistToHrManager(wishlist, "TeamLead", hrManager);
            }

            foreach (var jr in juniors)
            {
                var wishlist = jr.GenerateWishlist(teamLeads);
                jr.SendWishlistToHrManager(wishlist, "Junior", hrManager);
            }

            ITeamBuildingStrategy strategy = new MegaTeamBuildingStrategy();
            var teams = hrManager.GenerateTeams(strategy, hackathon);
            hackathon.Teams = teams;
            context.Teams.AddRange(teams);
            context.SaveChanges();

            var hrDirector = serviceProvider.GetRequiredService<HrDirector>();
            hrManager.SendTeamsToHrDirector(teams, hrDirector);
            hackathon.HarmonyScore = hrDirector.CalculateTeamsScore();
            context.SaveChanges();

            Console.WriteLine($"ИД Хакатона: {hackathon.HackathonId}");
            Console.WriteLine($"Среднее гармоническое: {hackathon.HarmonyScore}");

            foreach (var team in teams)
            {
                Console.WriteLine(team);
            }

            var averageHarmony = context.Hackathons.Average(h => h.HarmonyScore);
            Console.WriteLine($"Средняя гармоничность по всем хакатонам: {averageHarmony}");
        }
    }
}
