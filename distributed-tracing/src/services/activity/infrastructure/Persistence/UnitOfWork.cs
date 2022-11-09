using core_infrastructure.persistence;
using domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure.persistence
{
    public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(ActivityDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IActivityRepository Activities
        {
            get
            {
                return this._serviceProvider.GetRequiredService<IActivityRepository>();
            }
        }
    }
}
