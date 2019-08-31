using Microsoft.AspNetCore.Http;

namespace ST.Core.Services
{
    public interface IHttpContextAccessorService
    {
        HttpContext GetUserContext();
    }
}
