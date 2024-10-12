using Microsoft.EntityFrameworkCore;

namespace HrDirectorService.Models
{
    public class HackathonContext : DbContext
    {
        public HackathonContext(DbContextOptions<HackathonContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
    }
}
