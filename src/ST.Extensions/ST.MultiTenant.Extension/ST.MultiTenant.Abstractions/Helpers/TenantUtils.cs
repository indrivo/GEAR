using System.Text.RegularExpressions;

namespace ST.MultiTenant.Abstractions.Helpers
{
    public static class TenantUtils
    {
        /// <summary>
        /// Get machine name of tenant
        /// </summary>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        public static string GetTenantMachineName(string organizationName)
        {
            if (string.IsNullOrEmpty(organizationName)) return string.Empty;
            var regEx = new Regex(Resources.RegularExpressions.TENANT_MACHINE_NAME);
            return $"tenant_schema_{regEx.Replace(organizationName, string.Empty).ToLowerInvariant()}";
        }
    }
}
