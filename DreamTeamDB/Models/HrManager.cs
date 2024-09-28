using DreamTeamDB.Strategy;

namespace DreamTeamDB.Models
{
    public class HrManager
    {
        private readonly List<Wishlist> juniorWishlists = [];
        private readonly List<Wishlist> teamLeadWishlists = [];

        public void ReceiveWishlist(Wishlist wishlist, string type)
        {
            if (type.Equals("Junior"))
            {
                juniorWishlists.Add(wishlist);
            }
            else
            {
                teamLeadWishlists.Add(wishlist);
            }
        }

        public List<Team> GenerateTeams(ITeamBuildingStrategy teamBuildingStrategy, Hackathon hackathon)
        {
            return teamBuildingStrategy.BuildTeams(
                hackathon.TeamLeads,
                hackathon.Juniors,
                teamLeadWishlists,
                juniorWishlists
            ).ToList();
        }

        public void SendTeamsToHrDirector(List<Team> teams, HrDirector hrDirector)
        {
            var allWishlists = teamLeadWishlists.Concat(juniorWishlists).ToList();
            hrDirector.ReceiveTeamsAndWishlists(teams, allWishlists);
        }

        public void ClearAllWishlists()
        {
            juniorWishlists.Clear();
            teamLeadWishlists.Clear();
        }
    }
}
