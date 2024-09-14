namespace DreamTeamConsole.Models
{
    public record TeamLead(int Id, string Name) : Employee(Id, Name)
    {
        public override List<Employee> GetHackatonList(Hackaton hackaton)
        {
            return hackaton.Juniors.Cast<Employee>().ToList();
        }

        public override string ToString()
        {
            return $"TeamLead({Id} : {Name})";
        }
    }
}