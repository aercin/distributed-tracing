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
            public CommandHandler(IUnitOfWork uow,
                                  IOutboxMessagePublisher outboxMessagePublisher,
                                  IConfiguration configuration)
            {
                this._uow = uow;
                this._outboxMessagePublisher = outboxMessagePublisher;
                this._configuration = configuration;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = this._uow.Activities.GetById(request.ActivityId);

                activity.AddAttendant(request.FullName, request.IdentityNumber);

                await this._uow.CompleteAsync(async () =>
                {
                    await this._outboxMessagePublisher.PublishOutboxMessages("activity", this._configuration.GetValue<string>("Kafka:PublishTopic:ToActivityService"));
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
