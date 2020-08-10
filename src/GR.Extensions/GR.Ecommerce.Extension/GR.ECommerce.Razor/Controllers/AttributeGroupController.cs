using Microsoft.AspNetCore.Mvc;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.Helpers.BaseControllers;

namespace GR.ECommerce.Razor.Controllers
{
    public class AttributeGroupController : CommerceBaseController<AttributeGroup, AttributeGroup>
    {
        public AttributeGroupController(ICommerceContext context) : base(context)
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
