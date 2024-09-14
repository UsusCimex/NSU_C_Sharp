namespace DreamTeamConsole.Models
{
    public class HrManager
    {
        private readonly List<Wishlist> receivedWishlists = [];

        public void ReceiveWishlist(Wishlist wishlist)
        {
            receivedWishlists.Add(wishlist);
        }

        public List<Team> GenerateTeams(Hackaton hackaton)
        {
            List<Team> Teams = [];
            var random = new Random();

            var shuffledJuniors = hackaton.Juniors.OrderBy(i => random.Next()).ToList();
            var shuffledTeamLeads = hackaton.TeamLeads.OrderBy(i => random.Next()).ToList();

            for (int i = 0; i < shuffledTeamLeads.Count; i++)
            {
                // Console.WriteLine($"[LOG] Команда: {shuffledTeamLeads[i].Id} : {shuffledJuniors[i].Id}");
                Teams.Add(new Team(shuffledTeamLeads[i], shuffledJuniors[i]));
            }
            return Teams;
        }

        public void SendTeamsToHrDirector(List<Team> teams, HrDirector hrDirector)
        {
            hrDirector.ReceiveTeamsAndWishlists(new List<Team>(teams), new List<Wishlist>(receivedWishlists));
            receivedWishlists.Clear();
        }
    }
}
