using application;
using core_application.Abstractions;
using core_messages;
using MediatR;
using System.Diagnostics;
using System.Text.Json;

namespace api.Consumers
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly IEventListener _eventListener;
        private readonly IConfiguration _configuration;
        private readonly ICustomTracing _customTracing;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerWorker(IEventListener eventListener,
                              IConfiguration configuration,
                              ICustomTracing customTracing,
                              IServiceProvider serviceProvider)
        {
            this._eventListener = eventListener;
            this._configuration = configuration;
            this._customTracing = customTracing;
            this._serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this._eventListener.ConsumeEvent(this._configuration.GetValue<string>("Kafka:ConsumeTopic:FromActivityService"), async (consumeResult) =>
            {
                var message = consumeResult.Message;
                var messageBase = JsonSerializer.Deserialize<MessageBase>(message);

                if (messageBase.EventType == typeof(IE_ActivityDemandedEvent).FullName)
                {
                    await this._customTracing.StartActivity("consume attendant message from activity stream", ActivityKind.Consumer, consumeResult.Headers, async () =>
                    {
                        var activityDemanded = JsonSerializer.Deserialize<IE_ActivityDemandedEvent>(message);

                        using (var scope = this._serviceProvider.CreateScope())
                        {
                            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                            await mediator.Send(new ActivityPayment.Command
                            {
                                ActivityId = activityDemanded.ActivityId,
                                IdentityNumber = activityDemanded.IdentityNumber
                            });
                        }
                    });
                }
            }, stoppingToken);
        }

        public class MessageBase
        {
            public string EventType { get; set; }
        }
    }
}
