using core_application.Models;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace core_application.Abstractions
{
    public interface ICustomTracing
    {
        //ActivitySource ActivitySourceInstance { get; }
        List<MessageHeader> InjectTraceContextToMessageHeaderList();
        //PropagationContext ExractTraceContextFromMessageHeaderList(List<MessageHeader> messageHeaders);
        Task StartActivity(string name, ActivityKind kind, List<MessageHeader> parentTraceHeaders, Func<Task> traceLogic);
    }
}
