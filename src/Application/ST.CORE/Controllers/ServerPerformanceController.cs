using Microsoft.AspNetCore.Mvc;
using ST.CORE.Services;

namespace ST.CORE.Controllers
{
	public class ServerPerformanceController : Controller
	{
		/// <summary>
		/// Load page
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult LoadPerformanceServerData()
		{
			var cpuInfo = PerformanceServer.GetCpuInfo();
			var ramInfo = PerformanceServer.GetAllRamInformation();
			var osInfo = PerformanceServer.GetOSVersion();
			return Json(new
			{
				CpuInfo = cpuInfo,
				RamInfo = ramInfo,
				OsInfo = osInfo
			});
		}

		/// <summary>
		/// Get cpu load %
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult GetCpuPercentage()
		{
			var percentage = PerformanceServer.GetCpuUtilization();
			return Json(percentage);
		}
	}
}