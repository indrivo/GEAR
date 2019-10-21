using System.ComponentModel.DataAnnotations;

namespace GR.Email.Abstractions.Models.EmailViewModels
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
        /// 
        [Required]
        public string Host { get; set; }

        /// <summary>
        /// Port 
        /// </summary>
        [Required]
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
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
