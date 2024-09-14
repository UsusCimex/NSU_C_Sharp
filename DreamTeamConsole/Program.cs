using DreamTeamConsole.Models;

namespace DreamTeamConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string teamLeadsCsvPath = "csvFiles/TeamLeads20.csv";
            string juniorsCsvPath = "csvFiles/Juniors20.csv";

            Hackaton hackaton = new(teamLeadsCsvPath, juniorsCsvPath);

            HrManager hrManager = new();
            HrDirector hrDirector = new();

            double harmonicGlobalMean = 0;
            int countIteration = 1000;
            for (int i = 0; i < countIteration; i++) {
                foreach (TeamLead teamLead in hackaton.TeamLeads) {
                    Wishlist wishlist = teamLead.GenerateRandomWishlist(hackaton.Juniors.Cast<Employee>().ToList());
                    teamLead.SendWishlistToHrManager(wishlist, hrManager);
                }
                foreach (Junior junior in hackaton.Juniors) {
                    Wishlist wishlist = junior.GenerateRandomWishlist(hackaton.TeamLeads.Cast<Employee>().ToList());
                    junior.SendWishlistToHrManager(wishlist, hrManager);
                }
                
                List<Team> teams = hrManager.GenerateTeams(hackaton); // Hackaton?
                hrManager.SendTeamsToHrDirector(teams, hrDirector);
                double harmonicMean = hrDirector.CalculateTeamsScore();

                Console.WriteLine($"Harmonic Mean of Satisfaction: {harmonicMean}");

                harmonicGlobalMean += harmonicMean;
            }
            harmonicGlobalMean /= countIteration;
            Console.WriteLine($"Harmonic Global Mean of Satisfaction: {harmonicGlobalMean}");
        }
    }
}
