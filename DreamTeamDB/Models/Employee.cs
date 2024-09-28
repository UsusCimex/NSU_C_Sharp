using System.ComponentModel.DataAnnotations;

namespace DreamTeamDB.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<Wishlist> Wishlists { get; set; } = [];

        public Employee()
        {
        }

        public Employee(int id, string name)
        {
            Id = id;
            Name = name;
            Wishlists = [];
        }

        public Wishlist GenerateWishlist(IEnumerable<Employee> potentialPartners)
        {
            return GenerateRandomWishlist(potentialPartners);
        }

        private Wishlist GenerateRandomWishlist(IEnumerable<Employee> potentialPartners)
        {
            var random = new Random();
            var shuffledPartners = potentialPartners
                .OrderBy(it => random.Next())
                .Select(it => it.Id)
                .ToArray();

            return new Wishlist
            {
                EmployeeId = Id,
                DesiredEmployees = shuffledPartners
            };
        }

        public void SendWishlistToHrManager(Wishlist wishlist, string type, HrManager hrManager)
        {
            hrManager.ReceiveWishlist(wishlist, type);
        }

        public override string ToString()
        {
            return $"Employee({Id} : {Name})";
        }
    }
}
