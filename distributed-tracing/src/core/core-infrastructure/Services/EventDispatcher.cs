using Confluent.Kafka;
using core_application.Abstractions;
using System.Text;

namespace core_infrastructure.Services
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ICustomTracing _customTracing;
        public EventDispatcher(IProducer<Null, string> producer, ICustomTracing customTracing)
        {
            this._producer = producer;
            this._customTracing = customTracing;
        }
        public async Task DispatchEvent<T>(T topic, string integrationEvent) where T : class
        {
            var messageHeaders = this._customTracing.InjectTraceContextToMessageHeaderList();

            var kafkaMessageHeader = new Headers();

            messageHeaders.ForEach(messageHeader => kafkaMessageHeader.Add(messageHeader.Key, Encoding.UTF8.GetBytes(messageHeader.Value)));

            await this._producer.ProduceAsync(topic.ToString(), new Message<Null, string>
            {
                Headers = kafkaMessageHeader,
                Value = integrationEvent
            });
        } 
    }
}
