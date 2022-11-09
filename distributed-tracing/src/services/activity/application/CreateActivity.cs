using domain.Abstractions;
using domain.Entities;
using MediatR;

namespace application
{
    public static class CreateActivity
    {
        public class Command : IRequest<Response>
        {
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Amount { get; set; }
        }

        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly IUnitOfWork _uow;
            public CommandHandler(IUnitOfWork uow)
            {
                this._uow = uow;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var newActivity = Activity.CreateActivity(request.Description, request.StartDate, request.EndDate, request.Amount);

                this._uow.Activities.Add(newActivity);

                await this._uow.CompleteAsync();

                return new Response { IsSuccess = true };
            }
        }

        public class Response
        {
            public bool IsSuccess { get; set; }
        }
    }
}
