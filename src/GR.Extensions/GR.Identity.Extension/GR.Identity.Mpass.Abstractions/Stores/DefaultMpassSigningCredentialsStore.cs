using System;
using System.Threading.Tasks;
using GR.Identity.Mpass.Abstractions.Security;

namespace GR.Identity.Mpass.Abstractions.Stores
{
    /// <summary>
    /// The default store for retrieving MPass certificates. It can be your own, just implement the interface.
    /// </summary>
    public class DefaultMpassSigningCredentialsStore : IMPassSigningCredentialsStore
    {
        private readonly MPassSigningCredentials _credentials;

        /// <summary>
        /// The default constructor recieving the credentials and storing them.
        /// </summary>
        /// <param name="credentials"></param>
        public DefaultMpassSigningCredentialsStore(MPassSigningCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));

            if (credentials.ServiceProviderCertificate == null)
                throw new ArgumentNullException(nameof(credentials.ServiceProviderCertificate));

            if (credentials.IdentityProviderCertificate == null)
                throw new ArgumentNullException(nameof(credentials.IdentityProviderCertificate));

            _credentials = credentials;
        }

        /// <summary>
        /// Gets the stored <see cref="MPassSigningCredentials"/>
        /// </summary>
        /// <returns><see cref="MPassSigningCredentials"/> object containing the certificates</returns>
        public MPassSigningCredentials GetMPassCredentials()
        {
            return _credentials;
        }

        /// <summary>
        /// Gets the stored <see cref="MPassSigningCredentials"/> asynchronously
        /// </summary>
        /// <returns><see cref="MPassSigningCredentials"/> object containing the certificates</returns>
        public Task<MPassSigningCredentials> GetMPassCredentialsAsync()
        {
            return Task.FromResult(_credentials);
        }
    }
}
