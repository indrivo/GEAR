using Microsoft.Extensions.DependencyInjection;

namespace GR.Dashboard.Abstractions.ServiceBuilder
{
    public interface IDashboardServiceCollection
    {
        IServiceCollection Services { get; }
    }
}