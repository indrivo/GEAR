using GR.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using GR.WebApplication.Services;

namespace GR.Cms.Controllers
{
	[Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
	public class DataController : Controller
	{
		/// <summary>
		/// CreateZipArchive data
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Export()
		{
			var (stream, contentType, name) = await ExportManager.ExportAsync();
			return File(stream, contentType, name);
		}

		/// <summary>
		/// Import config zip file
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Import()
		{
			return View();
		}

		/// <summary>
		/// Apply zip config on current system
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public IActionResult Import(IFormFile file)
		{
			if (file == null)
			{
				ModelState.AddModelError(string.Empty, "File not selected!");
				return View();
			}

			if (file.ContentType != "application/x-zip-compressed")
			{
				ModelState.AddModelError(string.Empty, "File need to be in zip format!");
				return View();
			}

			var memStream = new MemoryStream();
			file.CopyTo(memStream);
			var response = ExportManager.Import(memStream);
			if (response.IsSuccess)
				return RedirectToAction("Index", "Home");
			ModelState.AddModelError(string.Empty, "Fail to import data");
			return View();
		}
	}
}