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
                    string teamLeadsCsvPath = configuration["TeamLeadsCsvPath"] ?? throw new FileLoadException("TeamLeadsCsvPath not found");
                    string juniorsCsvPath = configuration["JuniorsCsvPath"] ?? throw new FileLoadException("JuniorsCsvPath not found");
                    
                    services.AddHostedService<HackathonWorker>();
                    services.AddSingleton<Hackathon>(_ => new Hackathon(teamLeadsCsvPath, juniorsCsvPath));
                    services.AddSingleton<ITeamBuildingStrategy, RandomTeamBuildingStrategy>();
                    services.AddScoped<HrManager>();
                    services.AddTransient<HrDirector>();
                })
                .Build();
            
            _ = host.RunAsync();
            await host.StopAsync(); // or new CancellationTokenSource(msDelay).Token
        }
    }
}
