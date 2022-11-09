using core_application.Abstractions;
using core_infrastructure.DependencyManagements;
using core_infrastructure.Persistence;
using core_infrastructure.Services;
using MessageRelayService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
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
    })
    .Build();

await host.RunAsync();
