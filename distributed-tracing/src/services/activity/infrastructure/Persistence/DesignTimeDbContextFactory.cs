using infrastructure.persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace infrastructure.Persistence
{
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ActivityDbContext>
    {
        public ActivityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ActivityDbContext>();
            optionsBuilder.UseNpgsql("host=localhost;port=5001;database=activityDb;username=admin;password=sa1234");

            return new ActivityDbContext(optionsBuilder.Options);
        }
    }
}
