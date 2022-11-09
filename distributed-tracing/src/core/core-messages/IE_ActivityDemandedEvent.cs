using core_domain.Abstractions;

namespace core_messages
{
    public class IE_ActivityDemandedEvent : IntegrationEventBase
    {
        public int ActivityId { get; set; }
        public string IdentityNumber { get; set; }
    }
}
