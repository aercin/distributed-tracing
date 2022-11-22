using core_application.Models;

namespace core_application.Abstractions
{
    public interface IEventListener
    {
        Task ConsumeEvent<T>(T topic, Func<ConsumeResult, Task> callback, CancellationToken cancellationToken) where T : class;
    } 
}
