using core_application.Abstractions;
using core_infrastructure;
using core_infrastructure.DependencyManagements;
using core_infrastructure.Persistence;
using core_infrastructure.Services;
using domain.Abstractions;
using infrastructure.persistence;
using infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDistributedCaching(options =>
            {
                options.RedisHost = config.GetValue<string>("Redis:Host");
                options.RedisPort = config.GetValue<int>("Redis:Port");
                options.RedisPassword = config.GetValue<string>("Redis:Password");
                options.RedisDefaultDb = config.GetValue<int>("Redis:DefaultDb");
            });

            services.AddAsyncIntegrationStyleDependency(options =>
            {
                options.BrokerAddress = config.GetValue<string>("Kafka:BootstrapServers");
                options.ConsumerGroupId = config.GetValue<string>("Kafka:ConsumerGroupId");
            });

            services.AddCoreInfrastructure<ActivityDbContext>(x =>
            {
                x.ConnectionString = config.GetConnectionString("ActivityDb");
            });

            services.AddSingleton<IDbConnectionFactory, PostgreDbConnectionFactory>((serviceProvider) =>
            {
                return new PostgreDbConnectionFactory("activity", config.GetConnectionString("ActivityDb"));
            });
            services.AddSingleton<IOutboxMessagePublisher, OutboxMessagePublisher>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IDomainEventToMessageMapper, DomainEventToMessageMapper>();

            return services;
        }
    }
}
