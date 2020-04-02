using Microsoft.Extensions.Configuration;

namespace GR.Modules.Abstractions
{
    public interface IModule
    {
        string Name { get; }
        string SystemName { get; }
        string Version { get; }
        void ApplyBuilderConfiguration(IConfigurationBuilder builder);
    }
}