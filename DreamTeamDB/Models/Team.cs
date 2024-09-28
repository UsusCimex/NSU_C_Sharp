using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamTeamDB.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [ForeignKey("TeamLead")]
        public int TeamLeadId { get; set; }
        public virtual Employee TeamLead { get; set; } = null!;

        [ForeignKey("Junior")]
        public int JuniorId { get; set; }
        public virtual Employee Junior { get; set; } = null!;

        public int HackathonId { get; set; }


        public Team()
        {
        }

        public Team(Employee teamLead, Employee junior)
        {
            TeamLead = teamLead ?? throw new ArgumentNullException(nameof(teamLead));
            Junior = junior ?? throw new ArgumentNullException(nameof(junior));
            TeamLeadId = teamLead.Id;
            JuniorId = junior.Id;
        }

        public override string ToString()
        {
            return $"Team({TeamLead} : {Junior})";
        }
    }
}
