using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Attributes;
using GR.Core.Helpers;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Helpers;
using GR.PageRender.Abstractions.Models.RenderTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Razor.BaseControllers;

namespace GR.PageRender.Razor.Controllers
{
    [Authorize]
    public class TemplatesController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject page context
        /// </summary>
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        #endregion Injectable

        public TemplatesController(ICacheService cacheService, IDynamicPagesContext pagesContext)
        {
            _cacheService = cacheService;
            _pagesContext = pagesContext;
        }

        /// <summary>
		/// Load page types with ajax
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
        [AjaxOnly]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<JsonResult> LoadPages(DTParameters param)
        {
            var filtered = await _pagesContext.Templates.GetPagedAsDtResultAsync(param);
            return Json(filtered);
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new page type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<IActionResult> Create([Required] Template model)
        {
            if (_pagesContext.Templates.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError(string.Empty, "Name is used by another template!");
                return View(model);
            }
            try
            {
                model.IdentifierName = $"template_{model.Name}";
                _pagesContext.Templates.Add(model);
                _pagesContext.SaveChanges();
                await _cacheService.SetAsync(model.IdentifierName, new TemplateCacheModel
                {
                    Identifier = model.IdentifierName,
                    Value = model.Value
                });
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }

            return View(model);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public IActionResult Edit(Guid id)
        {
            if (id.Equals(Guid.Empty)) return NotFound();
            var model = _pagesContext.Templates.FirstOrDefault(x => x.Id.Equals(id));
            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<IActionResult> Edit(Template model)
        {
            if (model == null) return NotFound();
            var dataModel = _pagesContext.Templates.FirstOrDefault(x => x.Id.Equals(model.Id));

            if (dataModel == null) return NotFound();

            dataModel.Name = model.Name;
            dataModel.Description = model.Description;
            dataModel.Value = model.Value;
            try
            {
                _pagesContext.Templates.Update(dataModel);
                _pagesContext.SaveChanges();
                await _cacheService.SetAsync(dataModel.IdentifierName, new TemplateCacheModel
                {
                    Identifier = model.IdentifierName,
                    Value = dataModel.Value
                });
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }

            return View(model);
        }

        /// <summary>
        /// Delete page type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public async Task<JsonResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete template!", success = false });
            var template = _pagesContext.Templates.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
            if (template == null) return Json(new { message = "Fail to delete template!", success = false });

            try
            {
                _pagesContext.Templates.Remove(template);
                _pagesContext.SaveChanges();
                await _cacheService.RemoveAsync(template.IdentifierName);
                return Json(new { message = "Template was delete with success!", success = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Json(new { message = "Fail to delete template!", success = false });
        }

        /// <summary>
        /// Get template by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetTemplateByIdentifier(string identifier)
        {
            var result = new ResultModel<string>();
            if (string.IsNullOrEmpty(identifier))
            {
                result.Errors = new List<IErrorModel>
                {
                    new ErrorModel("null", "Not specified identifier")
                };
                return Json(result);
            }

            var template = await _cacheService.GetAsync<TemplateCacheModel>(identifier);
            if (template == null)
            {
                var templateFromStore = _pagesContext.Templates.FirstOrDefault(x => x.IdentifierName == identifier);
                if (templateFromStore == null)
                {
                    result.Errors = new List<IErrorModel>
                    {
                        new ErrorModel("null", "Template not found!")
                    };
                    return Json(result);
                }
                await _cacheService.SetAsync(templateFromStore.IdentifierName, new TemplateCacheModel
                {
                    Identifier = templateFromStore.IdentifierName,
                    Value = templateFromStore.Value
                });
                result.IsSuccess = true;
                result.Result = templateFromStore.Value;
            }
            else
            {
                result.IsSuccess = true;
                result.Result = template.Value;
            }

            return Json(result);
        }
    }
}