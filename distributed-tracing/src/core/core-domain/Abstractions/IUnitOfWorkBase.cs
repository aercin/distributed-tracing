namespace core_domain.Abstractions
{
    public interface IUnitOfWorkBase : IDisposable
    {
        Task CompleteAsync(Action sendOutboxMessagesToBroker = null);
        IOutboxMessageRepository OutboxMessages { get; }
    }
}
