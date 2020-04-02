using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Validation;

namespace GR.Identity.Clients.Abstractions.Helpers
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            context.Result = new GrantValidationResult("2941f2db-36e1-41df-b383-83dba777230c", OidcConstants.AuthenticationMethods.Password);
            return Task.FromResult(0);
        }
    }
}
