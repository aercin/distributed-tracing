using core_application.Abstractions;
using core_domain.Entities;
using Dapper;

namespace MessageRelayService
{
    public class ActivityWorker : BackgroundService
    {
        private readonly ILogger<ActivityWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOutboxMessagePublisher _outboxMessagePublisher; 
        public ActivityWorker(ILogger<ActivityWorker> logger,
                              IConfiguration configuration,
                              IOutboxMessagePublisher outboxMessagePublisher)
        {
            _logger = logger;
            _outboxMessagePublisher = outboxMessagePublisher;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(15));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Activity Message Relay Service is running at: {time}", DateTime.Now);

                await this._outboxMessagePublisher.PublishOutboxMessages("activity", _configuration.GetValue<string>("Kafka:PublishTopic:ToActivityService"));
            }
        }
    }
}