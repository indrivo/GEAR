using GR.Core.Attributes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace GR.PageRender.Razor.Controllers.StaticFiles
{
    public class StaticFileController : Controller
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<StaticFileController> _logger;
        private const string BasePath = "Static/Templates/";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public StaticFileController(IHostingEnvironment env, ILogger<StaticFileController> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Get template from static folder
        /// </summary>
        /// <param name="relPath"></param>
        /// <returns></returns>
        [AjaxOnly]
        public IActionResult GetJRenderTemplate(string relPath)
        {
            try
            {
                var fileInfo = _env.ContentRootFileProvider.GetFileInfo($"{BasePath}{relPath}");
                return File(fileInfo.CreateReadStream(), "text/x-jsrender");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured when retrieving template {0}", relPath);
                return NotFound();
            }
        }
    }
}