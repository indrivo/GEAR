using ST.Identity.Services.Abstractions;
using System;

namespace ST.Identity.CacheModels
{
    public class TenantSettings : ICacheModel
    {
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Allow access to tenant
        /// </summary>
        public  bool AllowAccess { get; set; }

        /// <summary>
        /// Tenant name
        /// </summary>
        public string TenantName { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        public Guid TenantId { get; set; }
    }
}
