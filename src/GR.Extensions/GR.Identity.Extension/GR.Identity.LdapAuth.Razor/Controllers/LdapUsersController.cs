using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.LdapAuth.Razor.Controllers
{
    [Admin]
    public class LdapUsersController : Controller
    {

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}