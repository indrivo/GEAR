namespace GR.Identity.Data.Permissions
{
    /// <summary>
    /// Custom ClaimTypes that represent profile data of our user
    /// </summary>
    public class BPMNClaimTypes
	{
        /// <summary>
        /// Flag indicating that this user has logged in
        /// using MPass SSO Identity Provider
        /// </summary>
        public const string IsMPass = "is_mpass";

        public const string Plenum = "plenum";

        /// <summary>
        /// Apartment part of the Address
        /// </summary>
        public const string Apartment = "apartment";

        /// <summary>
        /// First Name of the End-User
        /// </summary>
        public const string FirstName = "first_name";

        /// <summary>
        /// Last Name of the End-User
        /// </summary>
        public const string LastName = "last_name";

        /// <summary>
        /// A name derived from the name of a father or ancestor, 
        /// typically by the addition of a prefix or suffix, e.g., Johnson, O'Brien, Ivanovich
        /// </summary>
        public const string Patronymic = "patronymic";

        /// <summary>
        /// The Full Name of the End-User Which is a combination of
        /// LastName + FirstName + Patronymic e.g., Jon Doe Ivanovich
        /// </summary>
        public const string FullName = "full_name";

        /// <summary>
        /// Represents User's permission to do an action relative to a  module and a service.
        /// </summary>
        public const string Permission = "permission";
    }
}
