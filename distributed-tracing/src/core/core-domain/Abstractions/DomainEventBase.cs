namespace core_domain.Abstractions
{
    public class DomainEventBase
    { 
        public DateTime CreatedOn { get; private set; }
        public DomainEventBase()
        {
            this.CreatedOn = DateTime.Now;
        }
    }
}
