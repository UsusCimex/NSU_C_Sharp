namespace Shared.Models
{
    public enum EmployeeRole
    {
        Junior,
        TeamLead
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EmployeeRole Role { get; set; }

        public Employee(int id, string name, EmployeeRole role)
        {
            Id = id;
            Name = name;
            Role = role;
        }
    }
}
