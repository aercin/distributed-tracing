using core_domain.Abstractions;

namespace core_domain.Entities
{
    public class OutboxMessage : IAggregateRoot
    {
        public int Id { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public string Message { get; private set; }
        public string Type { get; private set; }
        public string TraceContext { get; private set; }

        private OutboxMessage() { }

        public static OutboxMessage CreateOutboxMessage(string type, string message, DateTime createdOn, string traceContext)
        {
            return new OutboxMessage
            {
                Type = type,
                Message = message,
                CreatedOn = createdOn,
                TraceContext = traceContext
            };
        }
    }
}
