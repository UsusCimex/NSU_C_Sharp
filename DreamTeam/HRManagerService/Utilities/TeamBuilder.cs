using Shared.Models;

namespace HRManagerService.Utilities
{
    public static class TeamBuilder
    {
        public static List<Team> BuildTeams(int hackathonId, List<Employee> teamLeads, List<Employee> juniors, List<Wishlist> wishlists)
        {
            // Простая стратегия: случайное сочетание TeamLead и Junior
            var random = new System.Random();
            var shuffledTeamLeads = teamLeads.OrderBy(x => random.Next()).ToList();
            var shuffledJuniors = juniors.OrderBy(x => random.Next()).ToList();

            var teams = new List<Team>();
            int count = System.Math.Min(shuffledTeamLeads.Count, shuffledJuniors.Count);

            for (int i = 0; i < count; i++)
            {
                teams.Add(new Team(hackathonId, shuffledTeamLeads[i], shuffledJuniors[i]));
            }

            return teams;
        }
    }
}
