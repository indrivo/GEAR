using Microsoft.Extensions.DependencyInjection;

namespace GR.Entities.Security.Abstractions.ServiceBuilder
{
    internal class EntitySecurityServiceCollection : IEntitySecurityServiceCollection
    {
        public IServiceCollection Services { get; }
        public EntitySecurityServiceCollection(IServiceCollection services)
        {
            Services = services;
        }
    }
}
