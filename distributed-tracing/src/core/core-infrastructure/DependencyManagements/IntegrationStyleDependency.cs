using Confluent.Kafka;
using core_application.Abstractions;
using core_infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace core_infrastructure.DependencyManagements
{
    public static class IntegrationStyleDependency
    {
        public static IServiceCollection AddAsyncIntegrationStyleDependency(this IServiceCollection services, Action<Options> options)
        {
            var dependencyOptions = new Options();
            options(dependencyOptions);

            services.AddSingleton<IProducer<Null, string>>(x => new ProducerBuilder<Null, string>(new ProducerConfig
            {
                BootstrapServers = dependencyOptions.BrokerAddress,
                Acks = Acks.Leader
            }).Build());
            services.AddSingleton<IEventDispatcher, EventDispatcher>(); 

            services.AddSingleton<IConsumer<Null, string>>(x => new ConsumerBuilder<Null, string>(new ConsumerConfig
            {
                BootstrapServers = dependencyOptions.BrokerAddress,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                GroupId = dependencyOptions.ConsumerGroupId
            }).Build());
            services.AddSingleton<IEventListener, EventListener>();

            return services;
        }

        public sealed class Options
        {
            public string BrokerAddress { get; set; }
            public string ConsumerGroupId { get; set; }
        }
    }
}
