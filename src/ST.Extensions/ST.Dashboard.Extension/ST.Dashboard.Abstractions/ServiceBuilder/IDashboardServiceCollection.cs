using Microsoft.Extensions.DependencyInjection;

namespace ST.Dashboard.Abstractions.ServiceBuilder
{
    public interface IDashboardServiceCollection
    {
        IServiceCollection Services { get; }
    }
}