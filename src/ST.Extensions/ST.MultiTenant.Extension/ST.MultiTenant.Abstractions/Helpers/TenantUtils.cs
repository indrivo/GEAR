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
           
            organizationName = organizationName.ToLower();
            organizationName = organizationName.Replace(" ", "");
            return organizationName;
        }
    }
}
