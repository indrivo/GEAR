using System.Threading.Tasks;

namespace GR.Identity.Mpass.Abstractions.Security
{
    /// <summary>
    /// A service that will store <see cref="MPassSigningCredentials"/> throughout 
    /// the main service lifetime
    /// </summary>
    public interface IMPassSigningCredentialsStore
    {
        /// <summary>
        /// Gets the stored <see cref="MPassSigningCredentials"/>
        /// </summary>
        /// <returns></returns>
        MPassSigningCredentials GetMPassCredentials();

        /// <summary>
        /// Gets the stored <see cref="MPassSigningCredentials"/> asynchronously
        /// </summary>
        /// <returns></returns>
        Task<MPassSigningCredentials> GetMPassCredentialsAsync();
    }
}
