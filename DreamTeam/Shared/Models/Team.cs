namespace Shared.Models
{
    public class Team
    {
        public int HackathonId { get; set; }
        public Employee TeamLead { get; set; }
        public Employee Junior { get; set; }

        public Team(int hackathonId, Employee teamLead, Employee junior)
        {
            HackathonId = hackathonId;
            TeamLead = teamLead;
            Junior = junior;
        }
    }
}
