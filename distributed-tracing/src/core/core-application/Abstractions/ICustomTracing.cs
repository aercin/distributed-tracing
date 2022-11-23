using core_application.Models;
using System.Diagnostics;

namespace core_application.Abstractions
{
    public interface ICustomTracing
    {
        List<MessageHeader> InjectTraceContextToMessageHeaderList();
        Task StartActivity(string name, ActivityKind kind, List<MessageHeader> parentTraceHeaders, Func<Task> traceLogic);
    }
}
