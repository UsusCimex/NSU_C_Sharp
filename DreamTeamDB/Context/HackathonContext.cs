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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Wishlist>()
                .Property(w => w.DesiredEmployees)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(int.Parse)
                          .ToArray()
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
