namespace ST.Email.Abstractions.Models.EmailViewModels
{
    public sealed class EmailSettingsViewModel
    {
        /// <summary>
        /// Is enabled service
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port 
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Timeout
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Enable Ssl
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Email Network Credential
        /// </summary>
        public EmailNetworkCredential NetworkCredential { get; set; }
    }

    public sealed class EmailNetworkCredential
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}
