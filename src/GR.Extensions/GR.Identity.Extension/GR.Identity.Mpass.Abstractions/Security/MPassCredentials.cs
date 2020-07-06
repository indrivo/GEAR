using System.Security.Cryptography.X509Certificates;

namespace GR.Identity.Mpass.Abstractions.Security
{
    /// <summary>
    /// Required certificates to successfully make a login request to MPass
    /// </summary>
    public class MPassSigningCredentials
    {
        /// <summary>
        /// The service provider certificate
        /// </summary>
        public X509Certificate2 ServiceProviderCertificate { get; set; }

        /// <summary>
        /// The identity provider certificate
        /// </summary>
        public X509Certificate2 IdentityProviderCertificate { get; set; }
    }
}
