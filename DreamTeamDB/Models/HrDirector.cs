namespace DreamTeamDB.Models
{
    public class HrDirector
    {
        private List<Team> Teams = [];
        private List<Wishlist> Wishlists = [];

        public void ReceiveTeamsAndWishlists(List<Team> teams, IEnumerable<Wishlist> wishlists)
        {
            Teams = teams ?? [];
            Wishlists = wishlists?.ToList() ?? [];
        }

        public double CalculateTeamsScore()
        {
            double totalSatisfaction = 0;
            int participantCount = Wishlists.Count;

            foreach (var team in Teams)
            {
                int teamLeadSatisfaction = CalculateSatisfaction(team.TeamLead, team.Junior);
                int juniorSatisfaction = CalculateSatisfaction(team.Junior, team.TeamLead);

                if (teamLeadSatisfaction != 0 && juniorSatisfaction != 0)
                {
                    totalSatisfaction += 1.0 / teamLeadSatisfaction + 1.0 / juniorSatisfaction;
                }
            }

            return participantCount != 0 ? participantCount / totalSatisfaction : 0;
        }

        private int CalculateSatisfaction(Employee current, Employee partner)
        {
            var wishlist = Wishlists.FirstOrDefault(it => it.EmployeeId == current.Id);
            if (wishlist == null)
            {
                return 0;
            }

            int index = wishlist.DesiredEmployees.ToList().FindIndex(it => it == partner.Id);
            return index >= 0 ? Teams.Count - index : 0;
        }
    }
}
