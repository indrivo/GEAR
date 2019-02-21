using ST.Identity.Services.Abstractions;

namespace ST.Identity.CacheModels
{
    public class TenantSettings : ICacheModel
    {
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Allow access to tenant
        /// </summary>
        public  bool AllowAccess { get; set; }
    }
}
