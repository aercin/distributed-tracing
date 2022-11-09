using core_infrastructure.persistence;
using domain.Abstractions;
using domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace infrastructure.persistence
{
    public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        private readonly ActivityDbContext _context;
        public ActivityRepository(ActivityDbContext context) : base(context)
        {
            this._context = context;
        }

        public Activity GetActivityWithSpecificAttendant(int activityId, string identityNumber)
        {
            return this._context.Activities.Include(x => x.Attendants.Where(y => y.IdentityNumber == identityNumber)).FirstOrDefault(x => x.Id == activityId);
        }
    }
}
