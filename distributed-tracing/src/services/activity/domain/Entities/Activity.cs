using core_domain.Abstractions;
using domain.Events;

namespace domain.Entities
{
    public class Activity : AggregateRootBase
    {
        private Activity()
        {
            attendants = new List<Attendant>();
        }

        public int Id { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }

        private List<Attendant> attendants;
        public IReadOnlyCollection<Attendant> Attendants
        {
            get
            {
                return attendants.AsReadOnly();
            }
        }

        public static Activity CreateActivity(string description, DateTime startDate, DateTime endDate, decimal amount)
        {
            var activity = new Activity();
            activity.Description = description;
            activity.StartDate = startDate;
            activity.EndDate = endDate;
            activity.Amount = amount;
            return activity;
        }

        public void AddAttendant(string fullName, string identityNo)
        {
            this.attendants.Add(Attendant.CreateAttendant(fullName, identityNo));
            this.AddDomainEvent(new ActivityDemandedEvent
            {
                ActivityId = Id,
                IdentityNumber = identityNo
            });
        }
    }
}
