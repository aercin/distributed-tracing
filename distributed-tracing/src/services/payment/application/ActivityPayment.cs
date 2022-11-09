using core_application.Abstractions;
using domain.Abstractions;
using domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace application
{
    public static class ActivityPayment
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
            private readonly IOutboxMessagePublisher _outboxMessagePublisher;

            public CommandHandler(IUnitOfWork uow,
                                  IHttpClientFactory httpClientFactory,
                                  IConfiguration configuration,
                                  IOutboxMessagePublisher outboxMessagePublisher)
            {
                this._uow = uow;
                this._httpClientFactory = httpClientFactory;
                this._configuration = configuration;
                this._outboxMessagePublisher = outboxMessagePublisher;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var payment = Payment.CreatePayment(request.ActivityId, request.IdentityNumber);

                this._uow.Payments.Add(payment);
                await this._uow.CompleteAsync(async () =>
                {
                    await this._outboxMessagePublisher.PublishOutboxMessages("payment", this._configuration.GetValue<string>("Kafka:PublishTopic:ToPaymentService"));
                });

                var jsonContent = new StringContent(JsonSerializer.Serialize(new
                {
                    From = "distributed-tracing@poc.com",
                    To = "bla@bla.com",
                    Message = "Etkinlik katılımı ödemeniz başarıyla alındı"
                }), Encoding.UTF8, Application.Json);

                var httpClient = this._httpClientFactory.CreateClient();

                await httpClient.PostAsync(this._configuration.GetValue<string>("NotificationServiceAddress"), jsonContent);

                return new Response { IsSuccess = true };
            }
        }

        public class Response
        {
            public bool IsSuccess { get; set; }
        }
    }
}
