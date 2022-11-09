using core_domain.Abstractions;
using domain.Entities;

namespace domain.Abstractions
{
    public interface IActivityRepository : IGenericRepository<Activity>
    {
        Activity GetActivityWithSpecificAttendant(int activityId, string identityNumber);
    }
}
