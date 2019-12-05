using Microsoft.Extensions.DependencyInjection;

namespace GR.WebApplication.Helpers
{
    public interface IGearServiceCollection : IServiceCollection
    {

    }

    public class GearServiceCollection : ServiceCollection, IGearServiceCollection
    {

    }
}