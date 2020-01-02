using System;

namespace GR.Identity.CacheModels
{
    public class TenantSettings
    {
        /// <summary>
        /// Allow access to tenant
        /// </summary>
        public bool AllowAccess { get; set; }

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