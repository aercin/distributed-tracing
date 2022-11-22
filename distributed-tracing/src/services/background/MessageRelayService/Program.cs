using core_application.Abstractions;
using core_infrastructure.DependencyManagements;
using core_infrastructure.Persistence;
using core_infrastructure.Services;
using MessageRelayService;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDistributedCaching(options =>
        {
            options.RedisHost = hostContext.Configuration.GetValue<string>("Redis:Host");
            options.RedisPort = hostContext.Configuration.GetValue<int>("Redis:Port");
            options.RedisPassword = hostContext.Configuration.GetValue<string>("Redis:Password");
            options.RedisDefaultDb = hostContext.Configuration.GetValue<int>("Redis:DefaultDb");
        });

        services.AddAsyncIntegrationStyleDependency(options =>
        {
            options.BrokerAddress = hostContext.Configuration.GetValue<string>("Kafka:BootstrapServers");
        });
        services.AddSingleton<IDbConnectionFactory, PostgreDbConnectionFactory>((serviceProvider) =>
        {
            return new PostgreDbConnectionFactory("activity", hostContext.Configuration.GetConnectionString("ActivityDb"));
        });
        services.AddSingleton<IDbConnectionFactory, PostgreDbConnectionFactory>((serviceProvider) =>
        {
            return new PostgreDbConnectionFactory("payment", hostContext.Configuration.GetConnectionString("PaymentDb"));
        });
        services.AddSingleton<IOutboxMessagePublisher, OutboxMessagePublisher>();
        services.AddHostedService<ActivityWorker>();
        services.AddHostedService<PaymentWorker>();

        services.AddSingleton<ICustomTracing, CustomTracing>((serviceProvider) =>
        {
            return new CustomTracing(hostContext.Configuration.GetValue<string>("Tracing:ActivitySourceName"));
        });

        services.AddOpenTelemetryTracing(options =>
        {
            options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MessageRelayService"));
            options.SetSampler(new AlwaysOnSampler());
            options.AddSource(hostContext.Configuration.GetValue<string>("Tracing:ActivitySourceName"));
            options.AddJaegerExporter(opt =>
            {
                opt.AgentHost = hostContext.Configuration.GetValue<string>("Jaeger:Host");
                opt.AgentPort = hostContext.Configuration.GetValue<int>("Jaeger:Port");
            });
        });
    })
    .Build();

await host.RunAsync();
