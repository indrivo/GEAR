using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Razor.Extensions
{
    public static class ControllerExtensions
    {
        public static void AddIdentityErrors(this Controller controller, IdentityResult result)
        {
            foreach (var error in result.Errors)
                controller.ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}