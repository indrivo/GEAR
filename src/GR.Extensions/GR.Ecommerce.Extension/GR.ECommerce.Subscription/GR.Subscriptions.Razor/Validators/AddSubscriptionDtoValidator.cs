using FluentValidation;
using GR.Subscriptions.Razor.Dto;

namespace GR.Subscriptions.Razor.Validators
{
    /// <summary>
    /// Validator for add subscription dto
    /// </summary>
    public class AddSubscriptionDtoValidator : AbstractValidator<AddSubscriptionDto>
    {
        public AddSubscriptionDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.ProductId)
                .NotNull();

            RuleFor(x => x.Unit)
                .NotNull();

            RuleFor(x => x.Period)
                .NotNull();
        }
    }
}