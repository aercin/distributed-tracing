using domain.Abstractions;
using MediatR;

namespace application
{
    public static class GetActivity
    {
        public class Command : IRequest<Response>
        {
            public string Description { get; set; }
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
                var activities = this._uow.Activities.Find(x => x.Description.ToLower().Contains(request.Description.ToLower()));
                 
                return new Response
                {
                    Activities = activities.Select(x => new ActivityItem
                    {
                        Id = x.Id,
                        Description = x.Description,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Amount = x.Amount
                    }).ToList()
                };
            }
        }
        public class Response
        {
            public List<ActivityItem> Activities { get; set; }
        }
        public class ActivityItem
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
