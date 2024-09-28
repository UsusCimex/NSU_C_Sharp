using Microsoft.EntityFrameworkCore;
using DreamTeamDB.Models;

namespace DreamTeamDB.Context
{
    public class HackathonContext(DbContextOptions<HackathonContext> options) : DbContext(options)
    {
        public DbSet<Hackathon> Hackathons { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
    }
}
