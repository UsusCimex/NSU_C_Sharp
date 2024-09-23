using DreamTeamConsole.Models;

namespace DreamTeamConsole.Tests
{
    public class EmployeeTests
    {
        [Fact]
        public void GenerateWishlistSizesTest()
        {
            var employee = new Employee(1, "Test Employee");
            var potentialPartners = new List<Employee>
            {
                new(2, "Partner 1"),
                new(3, "Partner 2"),
                new(4, "Partner 3")
            };

            var wishlist = employee.GenerateWishlist(potentialPartners);

            Assert.Equal(potentialPartners.Count, wishlist.DesiredEmployees.Length);
        }

        [Fact]
        public void GenerateWishlistContainsEmployeeTest()
        {
            var employee = new Employee(1, "Partner 1");
            var predefinedPartner = new Employee(2, "Partner 2");
            var potentialPartners = new List<Employee>
            {
                predefinedPartner,
                new(3, "Partner 3"),
                new(4, "Partner 4")
            };

            var wishlist = employee.GenerateWishlist(potentialPartners);

            Assert.Contains(predefinedPartner.Id, wishlist.DesiredEmployees);
        }
    }
}
