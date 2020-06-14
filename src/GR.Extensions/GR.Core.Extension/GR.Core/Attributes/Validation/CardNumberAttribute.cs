using System;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;

namespace GR.Core.Attributes.Validation
{
    [Author(Authors.LUPEI_NICOLAE)]
    [AttributeUsage(AttributeTargets.Property)]
    public class CardNumberAttribute : RegularExpressionAttribute
    {
        private const string RegexPattern = GlobalResources.RegularExpressions.CREDIT_CARD;

        public CardNumberAttribute() : base(RegexPattern)
        {
            ErrorMessage = "Not valid card number";
        }
    }
}