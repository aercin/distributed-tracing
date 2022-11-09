using core_domain.Abstractions;

namespace core_messages
{
    public class IE_PaymentSuccessedEvent : IntegrationEventBase
    {
        public int ActivityId { get; set; }
        public string IdentityNumber { get; set; }
    }
}
