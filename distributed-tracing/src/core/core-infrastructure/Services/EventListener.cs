using Confluent.Kafka;
using core_application.Abstractions;
using System.Text;

namespace core_infrastructure.Services
{
    public class EventListener : IEventListener
    {
        private IConsumer<Null, string> _consumer;
        public EventListener(IConsumer<Null, string> consumer)
        {
            this._consumer = consumer;
        }

        public async Task ConsumeEvent<T>(T topic, Func<core_application.Models.ConsumeResult, Task> callback, CancellationToken cancellationToken) where T : class
        {
            this._consumer.Subscribe(topic.ToString());

            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));

            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var response = this._consumer.Consume();
                var consumeResult = new core_application.Models.ConsumeResult();
                consumeResult.Message = response.Message.Value;
                response.Message.Headers?.ToList().ForEach(header => consumeResult.Headers.Add(new core_application.Models.MessageHeader
                {
                    Key = header.Key,
                    Value = Encoding.UTF8.GetString(header.GetValueBytes())
                }));
                await callback(consumeResult);
            }
        }
    }
}
