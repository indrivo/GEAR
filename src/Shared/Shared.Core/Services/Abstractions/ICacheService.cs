using System.Threading.Tasks;

namespace Shared.Core.Services.Abstractions
{
    public interface ICacheService
    {
        Task<bool> Set<TObject>(string key, TObject obj) where TObject : class;
        Task<TObject> Get<TObject>(string key) where TObject : class;
    }
}
