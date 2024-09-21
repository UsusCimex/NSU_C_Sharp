using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DreamTeamConsole.Models;
using DreamTeamConsole.Strategy;

namespace DreamTeamGenericHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    string teamLeadsCsvPath = configuration["TeamLeadsCsvPath"];
                    string juniorsCsvPath = configuration["JuniorsCsvPath"];
                    
                    services.AddHostedService<HackathonWorker>();
                    services.AddTransient<Hackathon>(_ => new Hackathon(teamLeadsCsvPath, juniorsCsvPath));
                    services.AddTransient<ITeamBuildingStrategy, RandomTeamBuildingStrategy>();
                    services.AddTransient<HrManager>();
                    services.AddTransient<HrDirector>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}
