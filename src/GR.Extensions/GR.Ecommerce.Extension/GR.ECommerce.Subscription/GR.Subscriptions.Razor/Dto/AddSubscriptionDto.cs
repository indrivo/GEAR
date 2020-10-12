using System;

namespace GR.Subscriptions.Razor.Dto
{
    public sealed class AddSubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string Period { get; set; }
        public string Unit { get; set; }
    }
}