using Microsoft.AspNetCore.Identity;

namespace GR.Identity.Abstractions.Helpers.PasswordPolicies
{
    public class DefaultPasswordPolicy : PasswordPolicy
    {
        public override PasswordOptions Policy(PasswordOptions options)
        {
            options.RequireDigit = true;
            options.RequireLowercase = true;
            options.RequireNonAlphanumeric = true;
            options.RequireUppercase = true;
            options.RequiredLength = 6;
            options.RequiredUniqueChars = 1;
            return options;
        }
    }
}
