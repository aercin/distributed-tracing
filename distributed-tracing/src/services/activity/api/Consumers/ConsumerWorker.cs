using application;
using core_application.Abstractions;
using core_messages;
using MediatR;
using System.Text.Json;

namespace api.Consumers
{
    public class ConsumerWorker : BackgroundService
    {
        private readonly IEventListener _eventListener;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public ConsumerWorker(IEventListener eventListener,
                              IConfiguration configuration,
                              IServiceProvider serviceProvider)
        {
            this._eventListener = eventListener;
            this._configuration = configuration;
            this._serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await this._eventListener.ConsumeEvent(this._configuration.GetValue<string>("Kafka:ConsumeTopic:FromPaymentService"), async (message) =>
            {
                var messageBase = JsonSerializer.Deserialize<MessageBase>(message);

                if (messageBase.EventType == typeof(IE_PaymentSuccessedEvent).FullName)
                {
                    var activityDemanded = JsonSerializer.Deserialize<IE_PaymentSuccessedEvent>(message);

                    using (var scope = this._serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        await mediator.Send(new ConfirmAttendant.Command
                        {
                            ActivityId = activityDemanded.ActivityId,
                            IdentityNumber = activityDemanded.IdentityNumber
                        });
                    }
                }
            }, stoppingToken);
        }

        public class MessageBase
        {
            public string EventType { get; set; }
        }
    }
}
