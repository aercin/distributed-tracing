using core_domain.Abstractions;

namespace domain.Events
{
    public class PaymentSuccessedEvent : DomainEventBase
    {
        public int ActivityId { get; set; }
        public string IdentityNumber { get; set; }
    }
}
