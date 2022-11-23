var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHealthChecksUI(setup =>
{
    setup.AddWebhookNotification("webhook1",
    uri: "http://localhost:5248/api/notification",
    payload: "{\r\n  \"message\": \"Webhook report for [[LIVENESS]]: [[FAILURE]] - Description: [[DESCRIPTIONS]]\"\r\n}",
    restorePayload: "{\r\n  \"message\": \"[[LIVENESS]] is back to life\"\r\n}");
    setup.SetMinimumSecondsBetweenFailureNotifications(10);
}).AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecksUI();

app.Run();
