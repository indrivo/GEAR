using System;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class BillingUserProfile : BaseModel
    {
        public virtual Guid UserId { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string StreetAddress { get; set; }
        public virtual string SecondStreetAddress { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string City { get; set; }
        public virtual string Province { get; set; }

    }
}
