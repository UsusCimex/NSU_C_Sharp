namespace Shared.Models
{
    public class Wishlist
    {
        public int EmployeeId { get; set; }
        public int HackathonId { get; set; }
        public int[] DesiredEmployeeIds { get; set; }

        public Wishlist(int employeeId, int hackathonId, int[] desiredEmployeeIds)
        {
            EmployeeId = employeeId;
            HackathonId = hackathonId;
            DesiredEmployeeIds = desiredEmployeeIds;
        }
    }
}
