using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GR.MobilPay.Razor.Controllers
{
    [Authorize]
    public class MobilPayController : Controller
    {
        public MobilPayController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCard(object data)
        {
            Response.ContentType = "text/xml";
            var message = "";
            await Response.WriteAsync(message);
            return Ok();
        }

        [HttpGet]
        public IActionResult ReturnCard()
        {
            return Ok();
        }
    }
}
