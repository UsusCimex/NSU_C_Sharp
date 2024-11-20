namespace Shared.Models
{
    public class Participant : Employee
    {
        public Participant(int id, string name, string roleString)
            : base(id, name, roleString == "TeamLead" ? EmployeeRole.TeamLead : EmployeeRole.Junior)
        {
        }
    }
}
