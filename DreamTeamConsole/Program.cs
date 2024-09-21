using DreamTeamConsole.Models;
using DreamTeamConsole.Strategy;

namespace DreamTeamConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string teamLeadsCsvPath = "../csvFiles/TeamLeads20.csv";
            string juniorsCsvPath = "../csvFiles/Juniors20.csv";

            Hackathon hackathon = new(teamLeadsCsvPath, juniorsCsvPath);

            HrManager hrManager = new();
            HrDirector hrDirector = new();

            double harmonicGlobalMean = 0;
            int countIteration = 1000;
            for (int i = 0; i < countIteration; i++) {
                foreach (Employee teamLead in hackathon.TeamLeads) {
                    Wishlist wishlist = teamLead.GenerateWishlist(hackathon.Juniors.Cast<Employee>().ToList());
                    teamLead.SendWishlistToHrManager(wishlist, "TeamLead", hrManager);
                }
                foreach (Employee junior in hackathon.Juniors) {
                    Wishlist wishlist = junior.GenerateWishlist(hackathon.TeamLeads.Cast<Employee>().ToList());
                    junior.SendWishlistToHrManager(wishlist, "Junior", hrManager);
                }
                
                ITeamBuildingStrategy strategy = new MegaTeamBuildingStrategy();
                List<Team> teams = hrManager.GenerateTeams(strategy, hackathon);
                hrManager.SendTeamsToHrDirector(teams, hrDirector);
                double harmonicMean = hrDirector.CalculateTeamsScore();

                Console.WriteLine($"Harmonic Mean of Satisfaction: {harmonicMean}");

                harmonicGlobalMean += harmonicMean;
                hrManager.ClearAllWishlists();
            }
            harmonicGlobalMean /= countIteration;
            Console.WriteLine($"Harmonic Global Mean of Satisfaction: {harmonicGlobalMean}");
        }
    }
}
