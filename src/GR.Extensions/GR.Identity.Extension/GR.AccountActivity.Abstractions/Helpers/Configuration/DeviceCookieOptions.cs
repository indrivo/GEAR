using Microsoft.AspNetCore.Http;

namespace GR.AccountActivity.Abstractions.Helpers.Configuration
{
    public class DeviceCookieOptions
    {
        public virtual bool Enabled { get; set; }
        public virtual CookieOptions CookieOptions { get; set; }
    }
}
