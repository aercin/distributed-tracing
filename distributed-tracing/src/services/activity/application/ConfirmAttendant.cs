using core_application.Abstractions;
using domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace application
{
    public static class ConfirmAttendant
    {
        public class Command : IRequest<Response>
        {
            public int ActivityId { get; set; }
            public string IdentityNumber { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly IUnitOfWork _uow;
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly IConfiguration _configuration;
            private readonly ICustomTracing _customTracing;
            public CommandHandler(IUnitOfWork uow, IHttpClientFactory httpClientFactory, IConfiguration configuration, ICustomTracing customTracing)
            {
                this._uow = uow;
                this._httpClientFactory = httpClientFactory;
                this._configuration = configuration;
                this._customTracing = customTracing;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                await this._customTracing.StartActivity("confirm attendant command handler", System.Diagnostics.ActivityKind.Internal, null, async () =>
                {
                    var activity = this._uow.Activities.GetActivityWithSpecificAttendant(request.ActivityId, request.IdentityNumber);

                    activity.Attendants.First().ConfirmAttendant();

                    await this._uow.CompleteAsync();

                    var jsonContent = new StringContent(JsonSerializer.Serialize(new
                    {
                        From = "distributed-tracing@poc.com",
                        To = "bla@bla.com",
                        Message = "Etkinliğe katılımınız onaylandı"
                    }), Encoding.UTF8, Application.Json);

                    var httpClient = this._httpClientFactory.CreateClient();

                    await httpClient.PostAsync(this._configuration.GetValue<string>("NotificationServiceAddress"), jsonContent);
                });

                return new Response { IsSuccess = true };
            }
        }

        public class Response
        {
            public bool IsSuccess { get; set; }
        }
    }
}
