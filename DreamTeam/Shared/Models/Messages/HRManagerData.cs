using System.Collections.Generic;
using Shared.Models;

namespace Shared.Messages
{
    public class HRManagerData
    {
        public List<Team> Teams { get; set; }
        public List<Wishlist> Wishlists { get; set; }

        public HRManagerData()
        {
            Teams = new List<Team>();
            Wishlists = new List<Wishlist>();
        }
    }
}
