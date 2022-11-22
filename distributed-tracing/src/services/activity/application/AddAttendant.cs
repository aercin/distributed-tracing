using core_application.Abstractions;
using domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace application
{
    public static class AddAttendant
    {
        public class Command : IRequest<Response>
        {
            public int ActivityId { get; set; }
            public string FullName { get; set; }
            public string IdentityNumber { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly IUnitOfWork _uow;
            private readonly IOutboxMessagePublisher _outboxMessagePublisher;
            private readonly IConfiguration _configuration;
            private readonly ICustomTracing _customTracing;
            public CommandHandler(IUnitOfWork uow,
                                  IOutboxMessagePublisher outboxMessagePublisher,
                                  IConfiguration configuration,
                                  ICustomTracing customTracing)
            {
                this._uow = uow;
                this._outboxMessagePublisher = outboxMessagePublisher;
                this._configuration = configuration;
                this._customTracing = customTracing;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                await this._customTracing.StartActivity("add attendant command handler", System.Diagnostics.ActivityKind.Internal, null, async () =>
                {
                    var activity = this._uow.Activities.GetById(request.ActivityId);

                    activity.AddAttendant(request.FullName, request.IdentityNumber);

                    await this._uow.CompleteAsync(async () =>
                    {
                        await this._outboxMessagePublisher.PublishOutboxMessages("activity", this._configuration.GetValue<string>("Kafka:PublishTopic:ToActivityService"));
                    });
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
