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

            if (domainEvent is ActivityDemandedEvent activityDemandedEvent)
            {
                resIntegrationEventBase = new IE_ActivityDemandedEvent
                {
                    ActivityId = activityDemandedEvent.ActivityId,
                    IdentityNumber = activityDemandedEvent.IdentityNumber,
                    CreatedOn = DateTime.Now,
                    EventType = typeof(IE_ActivityDemandedEvent).FullName
                };
            }

            return resIntegrationEventBase;
        }
    }
}
