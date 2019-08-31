using Microsoft.Extensions.DependencyInjection;

namespace ST.Entities.Security.Abstractions.ServiceBuilder
{
    public interface IEntitySecurityServiceCollection
    {
        IServiceCollection Services { get; }
    }
}