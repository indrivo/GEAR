using System.Collections.Generic;
using GR.Core.Helpers.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace GR.ECommerce.Abstractions.Helpers
{
    public class ProductsFilterRequest
    {
        public virtual int Page { get; set; } = 1;
        public virtual int PerPage { get; set; } = 10;

        [ModelBinder(BinderType = typeof(GearBinder<IEnumerable<CommerceFilter>>))]
        public virtual IEnumerable<CommerceFilter> Filters { get; set; }
    }
}
