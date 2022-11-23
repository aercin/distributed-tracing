using core_application.Abstractions;
using core_application.Models;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace core_infrastructure.Services
{
    public class CustomTracing : ICustomTracing
    { 
        private ActivitySource activitySourceInstance;

        public CustomTracing(string libraryName)
        {
            this.activitySourceInstance = new ActivitySource(libraryName);
        }

        public List<MessageHeader> InjectTraceContextToMessageHeaderList()
        {
            var messageHeaders = new List<MessageHeader>();
            Activity? currentTraceActivity = Activity.Current;
            if (currentTraceActivity != null)
            {
                TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
                Propagator.Inject(new PropagationContext(currentTraceActivity.Context, Baggage.Current), messageHeaders, (messageHeaders, key, value) =>
                {
                    //traceparent,tracestate,baggage keyleri activity context üzerinden messageHeader carrier'a eklenmektedir.
                    messageHeaders.Add(new MessageHeader { Key = key, Value = value });
                });
            }

            return messageHeaders;
        }

        public async Task StartActivity(string name, ActivityKind kind, List<MessageHeader> parentTraceHeaders, Func<Task> traceLogic)
        {
            PropagationContext parentTraceContext = default;
            if (parentTraceHeaders != null && parentTraceHeaders.Count > 0)
            {
                TextMapPropagator propagator = Propagators.DefaultTextMapPropagator;
                parentTraceContext = propagator.Extract(default, parentTraceHeaders, (parentTraceHeaders, key) =>
                {
                    try
                    {
                        var header = parentTraceHeaders.SingleOrDefault(x => x.Key == key);

                        return header == null ? Enumerable.Empty<string>() : new[] { header.Value };
                    }
                    catch (Exception ex)
                    {

                    }
                    return Enumerable.Empty<string>();
                });
                Baggage.Current = parentTraceContext.Baggage;
            }

            using (var activity = this.activitySourceInstance.StartActivity(name, kind, parentTraceContext.ActivityContext))
            {
                await traceLogic();
            }
        }
    }
}
