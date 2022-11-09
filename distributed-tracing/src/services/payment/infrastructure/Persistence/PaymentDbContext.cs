using core_infrastructure.persistence;
using domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.persistence
{
    public class PaymentDbContext : BaseDbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
        }

        public DbSet<Payment> Payments { get; set; }
    }
}
