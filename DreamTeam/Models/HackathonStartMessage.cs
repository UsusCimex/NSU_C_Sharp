namespace DreamTeam.Models
{
    public class HackathonStartMessage
    {
        public int HackathonId { get; set; }
        public required List<Employee> TeamLeads { get; set; }
        public required List<Employee> Juniors { get; set; }
    }
}
