using System.ComponentModel.DataAnnotations;

namespace DreamTeamDB.Models
{
    public class Hackathon
    {
        [Key]
        public int HackathonId { get; set; }

        public virtual ICollection<Employee> TeamLeads { get; set; } = [];
        public virtual ICollection<Employee> Juniors { get; set; } = [];
        public virtual ICollection<Team> Teams { get; set; } = [];
        public double HarmonyScore { get; set; }

        public Hackathon()
        {
        }

        public Hackathon(List<Employee> teamLeads, List<Employee> juniors)
        {
            TeamLeads = teamLeads ?? throw new ArgumentNullException(nameof(teamLeads));
            Juniors = juniors ?? throw new ArgumentNullException(nameof(juniors));
            Teams = [];

            if (TeamLeads.Count != Juniors.Count)
            {
                throw new Exception("The number of team leads and juniors must be equal!");
            }

            if (TeamLeads.Count == 0)
            {
                throw new Exception("At least two participants are required in a hackathon!");
            }
        }
    }
}
