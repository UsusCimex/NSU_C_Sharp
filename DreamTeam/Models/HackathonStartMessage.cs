namespace DreamTeam.Models
{
    public class HackathonStartMessage
    {
        public int HackathonId { get; set; }
        public List<Employee> TeamLeads { get; set; }
        public List<Employee> Juniors { get; set; }
    }
}
