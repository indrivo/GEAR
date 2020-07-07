using System;

namespace GR.ECommerce.Abstractions.ViewModels.BrandViewModels
{
    public class UpdateBrandViewModel : CreateBrandViewModel
    {
        public virtual Guid Id { get; set; }
    }
}