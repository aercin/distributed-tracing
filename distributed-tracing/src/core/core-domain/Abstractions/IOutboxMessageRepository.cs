using core_domain.Entities;

namespace core_domain.Abstractions
{
    public interface IOutboxMessageRepository : IGenericRepository<OutboxMessage>
    {
    }
}
