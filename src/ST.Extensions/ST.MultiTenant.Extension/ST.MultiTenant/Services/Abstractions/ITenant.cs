using System;

namespace ST.MultiTenant.Services.Abstractions
{
    public interface ITenant
    {
        /// <summary>
        /// Tenant id
        /// </summary>
        Guid? CurrentUserTenantId { get; }
    }
}
