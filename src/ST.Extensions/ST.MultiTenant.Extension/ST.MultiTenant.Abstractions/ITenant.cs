using System;

namespace ST.MultiTenant.Abstractions
{
    public interface ITenant
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        Guid? CurrentUserTenantId { get; }
    }
}
