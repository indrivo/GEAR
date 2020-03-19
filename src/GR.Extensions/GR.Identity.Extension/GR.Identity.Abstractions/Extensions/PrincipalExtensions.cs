using System.Diagnostics;
using System.Security.Principal;

namespace GR.Identity.Abstractions.Extensions
{
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Determines whether this instance is authenticated.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <returns>
        ///   <c>true</c> if the specified principal is authenticated; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public static bool IsAuthenticated(this IPrincipal principal)
        {
            return principal?.Identity != null && principal.Identity.IsAuthenticated;
        }
    }
}
