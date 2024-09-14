namespace DreamTeamConsole.Models
{
    public record Junior(int Id, string Name) : Employee(Id, Name)
    {
        public override List<Employee> GetHackatonList(Hackaton hackaton)
        {
            return hackaton.TeamLeads.Cast<Employee>().ToList();
        }

        public override string ToString()
        {
            return $"Junior({Id} : {Name})";
        }
    }
}