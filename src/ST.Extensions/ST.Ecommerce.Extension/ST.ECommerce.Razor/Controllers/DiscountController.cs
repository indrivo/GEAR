using Microsoft.AspNetCore.Mvc;
using ST.Core.Abstractions;
using ST.ECommerce.Abstractions;
using ST.ECommerce.Abstractions.Models;
using ST.ECommerce.Razor.Helpers.BaseControllers;

namespace ST.ECommerce.Razor.Controllers
{
    public class DiscountController : CommerceBaseController<Discount, Discount>
    {
        public DiscountController(ICommerceContext context, IDataFilter dataFilter) : base(context, dataFilter)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        public override IActionResult Index()
        {
            return View();
        }
    }
}
