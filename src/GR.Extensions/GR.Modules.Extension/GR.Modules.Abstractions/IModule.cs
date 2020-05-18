namespace GR.Modules.Abstractions
{
    public interface IModule
    {
        string Name { get; }
        string SystemName { get; }
        string Version { get; }
    }
}