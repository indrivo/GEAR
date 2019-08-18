using Microsoft.Extensions.DependencyInjection;

namespace ST.Dashboard.Abstractions.ServiceBuilder
{
    internal class DashboardServiceCollection : IDashboardServiceCollection
    {
        public IServiceCollection Services { get; }

        public DashboardServiceCollection(IServiceCollection services)
        {
            Services = services;
        }
    }
}
