using core_application.Abstractions;
using core_infrastructure;
using core_infrastructure.DependencyManagements;
using core_infrastructure.Persistence;
using core_infrastructure.Services;
using domain.Abstractions;
using infrastructure.persistence;
using infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
                x.TraceActivitySourceName = config.GetValue<string>("Tracing:ActivitySourceName");
            });

            services.AddSingleton<IDbConnectionFactory, PostgreDbConnectionFactory>((serviceProvider) =>
            {
                return new PostgreDbConnectionFactory("activity", config.GetConnectionString("ActivityDb"));
            });
            services.AddSingleton<IOutboxMessagePublisher, OutboxMessagePublisher>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IDomainEventToMessageMapper, DomainEventToMessageMapper>();

            services.AddOpenTelemetryTracing(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("ActivityMicroService"));
                options.AddAspNetCoreInstrumentation();
                options.AddNpgsql();
                options.AddHttpClientInstrumentation();
                options.ConfigureBuilder((sp, builder) =>
                {
                    RedisCache cache = (RedisCache)sp.GetRequiredService<IDistributedCache>();
                    builder.AddRedisInstrumentation(cache.GetConnection());
                });
                options.AddSource(config.GetValue<string>("Tracing:ActivitySourceName"));
                options.AddJaegerExporter(opt =>
                {
                    opt.AgentHost = config.GetValue<string>("Jaeger:Host");
                    opt.AgentPort = config.GetValue<int>("Jaeger:Port");
                });
            });

            return services;
        }
    }
}
