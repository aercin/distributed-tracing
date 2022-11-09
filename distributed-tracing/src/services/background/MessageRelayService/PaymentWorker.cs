using core_application.Abstractions;

namespace MessageRelayService
{
    public class PaymentWorker : BackgroundService
    {
        private readonly ILogger<PaymentWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IOutboxMessagePublisher _outboxMessagePublisher;
        public PaymentWorker(ILogger<PaymentWorker> logger,
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
                _logger.LogInformation("Payment Message Relay Service is running at: {time}", DateTime.Now);

                await this._outboxMessagePublisher.PublishOutboxMessages("payment", _configuration.GetValue<string>("Kafka:PublishTopic:ToPaymentService"));
            }
        }
    }
}