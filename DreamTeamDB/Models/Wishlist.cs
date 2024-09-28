using System.ComponentModel.DataAnnotations;

namespace DreamTeamDB.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;

        public int[] DesiredEmployees { get; set; } = [];

        public Wishlist()
        {
        }

        public Wishlist(Wishlist wishlist)
        {
            EmployeeId = wishlist.EmployeeId;
            DesiredEmployees = wishlist.DesiredEmployees ?? Array.Empty<int>();
        }

        public override string ToString()
        {
            return $"Wishlist({EmployeeId})";
        }
    }
}
