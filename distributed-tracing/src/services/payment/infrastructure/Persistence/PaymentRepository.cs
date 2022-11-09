using core_infrastructure.persistence;
using domain.Abstractions;
using domain.Entities;

namespace infrastructure.persistence
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(PaymentDbContext context) : base(context)
        {
        }
    }
}
