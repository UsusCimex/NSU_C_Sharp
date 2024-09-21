using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DreamTeamConsole.Models;
using DreamTeamConsole.Strategy;

namespace DreamTeamGenericHost
{
    public class HackathonWorker : IHostedService
    {
        private readonly Hackathon hackathon;
        private readonly HrManager hrManager;
        private readonly HrDirector hrDirector;
        private readonly ITeamBuildingStrategy strategy;

        public HackathonWorker(Hackathon hackathon, HrManager hrManager, HrDirector hrDirector, ITeamBuildingStrategy strategy)
        {
            this.hackathon = hackathon;
            this.hrManager = hrManager;
            this.hrDirector = hrDirector;
            this.strategy = strategy;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            double harmonicGlobalMean = 0;
            int countIteration = 1000;
            for (int i = 0; i < countIteration; i++)
            {
                foreach (Employee teamLead in hackathon.TeamLeads)
                {
                    Wishlist wishlist = teamLead.GenerateWishlist(hackathon.Juniors);
                    teamLead.SendWishlistToHrManager(wishlist, "TeamLead", hrManager);
                }

                foreach (Employee junior in hackathon.Juniors)
                {
                    Wishlist wishlist = junior.GenerateWishlist(hackathon.TeamLeads);
                    junior.SendWishlistToHrManager(wishlist, "Junior", hrManager);
                }

                List<Team> teams = hrManager.GenerateTeams(strategy, hackathon);
                hrManager.SendTeamsToHrDirector(teams, hrDirector);
                double harmonicMean = hrDirector.CalculateTeamsScore();

                Console.WriteLine($"Harmonic Mean of Satisfaction: {harmonicMean}");

                harmonicGlobalMean += harmonicMean;
                hrManager.ClearAllWishlists();
            }
            harmonicGlobalMean /= countIteration;
            Console.WriteLine($"Harmonic Global Mean of Satisfaction: {harmonicGlobalMean}");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
