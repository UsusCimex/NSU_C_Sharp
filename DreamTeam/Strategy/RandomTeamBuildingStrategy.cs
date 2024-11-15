using DreamTeam.Models;

namespace DreamTeam.Strategy 
{
    public class RandomTeamBuildingStrategy : ITeamBuildingStrategy
    {
        public IEnumerable<Team> BuildTeams(
            int hackathonId,
            IEnumerable<Employee> teamLeads, 
            IEnumerable<Employee> juniors, 
            IEnumerable<Wishlist> teamLeadsWishlists, 
            IEnumerable<Wishlist> juniorsWishlists
        ) {
            List<Team> Teams = [];
            var random = new Random();

            var shuffledJuniors = juniors.OrderBy(i => random.Next()).ToList();
            var shuffledTeamLeads = teamLeads.OrderBy(i => random.Next()).ToList();

            for (int i = 0; i < shuffledTeamLeads.Count; i++)
            {
                // Console.WriteLine($"[LOG] Команда: {shuffledTeamLeads[i].Id} : {shuffledJuniors[i].Id}");
                Teams.Add(new Team(hackathonId, shuffledTeamLeads[i], shuffledJuniors[i]));
            }
            return Teams;
        }
    }
}