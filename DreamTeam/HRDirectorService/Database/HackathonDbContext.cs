using Microsoft.EntityFrameworkCore;
using HRDirectorService.Models;

namespace HRDirectorService.Database
{
    public class HackathonDbContext : DbContext
    {
        public HackathonDbContext(DbContextOptions<HackathonDbContext> options)
            : base(options)
        {
        }

        public DbSet<HackathonResult> HackathonResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройки моделей при необходимости
            base.OnModelCreating(modelBuilder);
        }
    }
}
