namespace GR.MultiTenant.Abstractions.Events.EventArgs
{
    public class CompanyRegisterEventArgs : System.EventArgs
    {
        /// <summary>
        /// Company name
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Company user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Email adress
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
    }
}
