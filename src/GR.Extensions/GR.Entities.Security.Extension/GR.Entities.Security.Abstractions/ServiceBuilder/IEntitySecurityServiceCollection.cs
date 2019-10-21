using Microsoft.Extensions.DependencyInjection;

namespace GR.Entities.Security.Abstractions.ServiceBuilder
{
    public interface IEntitySecurityServiceCollection
    {
        IServiceCollection Services { get; }
    }
}