using core_infrastructure.persistence;
using domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.persistence
{
    public class ActivityDbContext : BaseDbContext
    {
        public ActivityDbContext(DbContextOptions<ActivityDbContext> options) : base(options)
        {
        } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActivityDbContext).Assembly);
        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Attendant> Attendants { get; set; }
    }
}
