using GR.Subscriptions.Abstractions.Models;

namespace GR.Subscriptions.Abstractions.Attributes.SubscriptionPermissionValidators
{
    public interface ISubscriptionValidator
    {
        string AttributeName { get; }
        bool Validate(SubscriptionPermission permission);
    }
}