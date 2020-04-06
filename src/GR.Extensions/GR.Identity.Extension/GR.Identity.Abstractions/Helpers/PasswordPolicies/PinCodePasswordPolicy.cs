using System;
using Microsoft.AspNetCore.Identity;

namespace GR.Identity.Abstractions.Helpers.PasswordPolicies
{
    public sealed class PinCodePasswordPolicy : PasswordPolicy
    {
        private int PinLength { get; set; }

        public PinCodePasswordPolicy(int pinLength)
        {
            PinLength = pinLength;
        }

        public override PasswordOptions Policy(PasswordOptions options)
        {
            options.RequireDigit = true;
            options.RequiredLength = PinLength;
            options.RequireNonAlphanumeric = false;
            options.RequireUppercase = false;
            options.RequireLowercase = false;
            return options;
        }
    }
}
