using core_domain.Abstractions;
using domain.Events;

namespace domain.Entities
{
    public class Payment : AggregateRootBase
    {
        public int Id { get; private set; }
        public int ActivityId { get; private set; }
        public string IdentityNumber { get; private set; }
        public DateTime PaymentDate { get; private set; }

        private Payment()
        {
            PaymentDate = DateTime.Now;
        }

        public static Payment CreatePayment(int activityId, string identityNumber)
        {
            var payment = new Payment();
            payment.ActivityId = activityId;
            payment.IdentityNumber = identityNumber;
            payment.AddDomainEvent(new PaymentSuccessedEvent
            {
                ActivityId = activityId,
                IdentityNumber = identityNumber
            });
            return payment;
        }
    }
}
