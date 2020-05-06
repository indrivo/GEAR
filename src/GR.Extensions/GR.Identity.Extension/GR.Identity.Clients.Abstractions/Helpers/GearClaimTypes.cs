namespace GR.Identity.Clients.Abstractions.Helpers
{
    public static class GearClaimTypes
    {
        /// <summary>
        /// Tenant claim
        /// </summary>
        public static string Tenant = nameof(Identity.Abstractions.Models.MultiTenants.Tenant).ToLowerInvariant();

        /// <summary>
        /// User photo url claim
        /// </summary>
        public static string UserPhotoUrl = "userPhotoUrl";

        /// <summary>
        /// First name
        /// </summary>
        public static string FirstName = "firstName";

        /// <summary>
        /// Last name
        /// </summary>
        public static string LastName = "lastName";

        /// <summary>
        /// Birth day claim
        /// </summary>
        public static string BirthDay = "birthDay";
        
        /// <summary>
        /// Is disabled
        /// </summary>
        public static string IsDisabled = "isDisabled";
    }
}