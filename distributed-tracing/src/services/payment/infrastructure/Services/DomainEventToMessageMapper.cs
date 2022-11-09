using core_application.Abstractions;
using core_domain.Abstractions;
using core_messages;
using domain.Events;

namespace infrastructure.Services
{
    public class DomainEventToMessageMapper : IDomainEventToMessageMapper
    {
        public IntegrationEventBase GetIntegrationEvent(DomainEventBase domainEvent)
        {
            IntegrationEventBase resIntegrationEventBase = null;

            if (domainEvent is PaymentSuccessedEvent paymentSuccessedEvent)
            {
                resIntegrationEventBase = new IE_PaymentSuccessedEvent
                {
                    ActivityId = paymentSuccessedEvent.ActivityId,
                    IdentityNumber = paymentSuccessedEvent.IdentityNumber, 
                    CreatedOn = DateTime.Now,
                    EventType = typeof(IE_PaymentSuccessedEvent).FullName
                };
            }

            return resIntegrationEventBase;
        }
    }
}
