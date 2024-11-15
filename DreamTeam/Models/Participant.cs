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

        public override string ToString()
        {
            return Role==EmployeeRole.Junior ?
                $"Junior(id={Id}, name={Name})" :
                $"TeamLead(id={Id}, name={Name})";
        }
    }
}
