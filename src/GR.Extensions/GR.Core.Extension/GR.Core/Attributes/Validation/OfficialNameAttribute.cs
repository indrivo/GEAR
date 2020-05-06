using System.ComponentModel.DataAnnotations;

namespace GR.Core.Attributes.Validation
{
    public class OfficialNameAttribute : RegularExpressionAttribute
    {
        private const string RegexPattern = "^[\\w'\\-,.][^0-9_!¡?÷?¿/\\\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$";

        public OfficialNameAttribute(string propriety) : base(RegexPattern)
        {
            ErrorMessage = $"{propriety} does not correspond to an official name";
        }
    }
}
