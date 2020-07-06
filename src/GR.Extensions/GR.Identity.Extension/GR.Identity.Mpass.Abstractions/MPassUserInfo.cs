namespace GR.Identity.Mpass.Abstractions
{
    /// <summary>
    /// Represents an object that contains basic user info
    /// received from MPass SAML Response.
    /// </summary>
    public class MPassUserInfo
    {
        /// <summary>
        /// User's IDNP (Moldovan ID Card Identification Number)
        /// </summary>
        public string NameID { get; set; }

        /// <summary>
        /// User's First Name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Users's Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Generated MPass Request Id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// User PhoneNumber
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///  User Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///  User Gender
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        ///  User LangID
        /// </summary>
        public string LangID { get; set; }


        /// <summary>
        /// String representation of the current object used for debugging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{FirstName ?? "No FirstName"} {LastName ?? "No LastName"}";
        }
    }
}
