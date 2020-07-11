using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Card.Razor.Controllers
{
    [Authorize]
    public class CreditCardsController : Controller
    {
        public IActionResult Manage()
        {
            return View();
        }
    }
}
