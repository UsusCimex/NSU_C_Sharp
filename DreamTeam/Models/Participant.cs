namespace DreamTeam.Models
{
    public enum EmployeeRole
    {
        Junior,
        TeamLead
    }

    public class Participant : Employee
    {
        public EmployeeRole Role { get; set; }

        public Participant(int id, string name, EmployeeRole role)
            : base(id, name)
        {
            Role = role;
        }
    }
}
