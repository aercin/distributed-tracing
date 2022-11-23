using core_application.Abstractions;
using core_infrastructure.DependencyManagements;
using core_infrastructure.Persistence;
using core_infrastructure.Services;
using HealthChecks.UI.Client;
using MessageRelayService;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDistributedCaching(options =>
{
    options.RedisHost = builder.Configuration.GetValue<string>("Redis:Host");
    options.RedisPort = builder.Configuration.GetValue<int>("Redis:Port");
    options.RedisPassword = builder.Configuration.GetValue<string>("Redis:Password");
    options.RedisDefaultDb = builder.Configuration.GetValue<int>("Redis:DefaultDb");
}, out string redisConnStr);

builder.Services.AddAsyncIntegrationStyleDependency(options =>
{
    options.BrokerAddress = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
});
builder.Services.AddSingleton<IDbConnectionFactory, PostgreDbConnectionFactory>((serviceProvider) =>
{
    return new PostgreDbConnectionFactory("activity", builder.Configuration.GetConnectionString("ActivityDb"));
});
builder.Services.AddSingleton<IDbConnectionFactory, PostgreDbConnectionFactory>((serviceProvider) =>
{
    return new PostgreDbConnectionFactory("payment", builder.Configuration.GetConnectionString("PaymentDb"));
});
builder.Services.AddSingleton<IOutboxMessagePublisher, OutboxMessagePublisher>();
builder.Services.AddHostedService<ActivityWorker>();
builder.Services.AddHostedService<PaymentWorker>();

builder.Services.AddSingleton<ICustomTracing, CustomTracing>((serviceProvider) =>
{
    return new CustomTracing(builder.Configuration.GetValue<string>("Tracing:ActivitySourceName"));
});

builder.Services.AddOpenTelemetryTracing(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MessageRelayService"));
    options.SetSampler(new AlwaysOnSampler());
    options.AddSource(builder.Configuration.GetValue<string>("Tracing:ActivitySourceName"));
    options.AddJaegerExporter(opt =>
    {
        opt.AgentHost = builder.Configuration.GetValue<string>("Jaeger:Host");
        opt.AgentPort = builder.Configuration.GetValue<int>("Jaeger:Port");
    });
});

builder.Services.AddHealthChecks()
        .AddNpgSql(builder.Configuration.GetConnectionString("ActivityDb"), name: "postgre-activity-db")
        .AddNpgSql(builder.Configuration.GetConnectionString("PaymentDb"), name: "postgre-payment-db")
        .AddRedis(redisConnStr, name: "redis")
        .AddKafka(setup =>
        {
            setup.BootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
            setup.MessageTimeoutMs = 5000;
        }, name: "kafka");

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = reg => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
