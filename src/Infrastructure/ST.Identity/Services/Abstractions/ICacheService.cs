using System.Threading.Tasks;

namespace ST.Identity.Services.Abstractions
{
    public interface ICacheService
    {
        Task<bool> Set<TObject>(string key, TObject obj) where TObject : class, ICacheModel;
        Task<TObject> Get<TObject>(string key) where TObject : class, ICacheModel;
    }
}
