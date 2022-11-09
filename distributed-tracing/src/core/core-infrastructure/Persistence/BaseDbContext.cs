using core_domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace core_infrastructure.persistence
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}
