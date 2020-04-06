using Microsoft.AspNetCore.Identity;

namespace GR.Identity.Abstractions.Helpers.PasswordPolicies
{
    public abstract class PasswordPolicy
    {
        public abstract PasswordOptions Policy(PasswordOptions options);
    }
}
