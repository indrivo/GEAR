using GR.Core;
using GR.Core.Attributes;
using GR.Core.Helpers;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Identity.Abstractions.Helpers.Attributes;

namespace GR.PageRender.Razor.Controllers
{
    [Admin]
    public class PageTypeController : Controller
    {
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pagesContext"></param>
        public PageTypeController(IDynamicPagesContext pagesContext)
        {
            _pagesContext = pagesContext;
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
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
        public IActionResult Create(PageType model)
        {
            if (model != null)
            {
                try
                {
                    _pagesContext.PageTypes.Add(model);
                    _pagesContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, e.Message);
                }
            }

            return View(model);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            if (id.Equals(Guid.Empty)) return NotFound();
            var model = _pagesContext.PageTypes.FirstOrDefault(x => x.Id.Equals(id));
            if (model == null) return NotFound();

            return View(model);
        }

        /// <summary>
        /// Edit page type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(PageType model)
        {
            if (model == null) return NotFound();
            var dataModel = _pagesContext.PageTypes.FirstOrDefault(x => x.Id.Equals(model.Id));

            if (dataModel == null) return NotFound();

            dataModel.Name = model.Name;
            dataModel.Description = model.Description;
            dataModel.Author = model.Author;
            dataModel.Changed = DateTime.Now;
            try
            {
                _pagesContext.PageTypes.Update(dataModel);
                _pagesContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }

            return View(model);
        }

        /// <summary>
        /// Load page types with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public async Task<JsonResult> LoadPages(DTParameters param)
        {
            var filtered = await _pagesContext.PageTypes.GetPagedAsDtResultAsync(param);
            return Json(filtered);
        }

        /// <summary>
        /// Delete page type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete page type!", success = false });
            var page = _pagesContext.PageTypes.FirstOrDefault(x => x.Id.Equals(Guid.Parse(id)));
            if (page == null) return Json(new { message = "Fail to delete page type!", success = false });

            try
            {
                _pagesContext.PageTypes.Remove(page);
                _pagesContext.SaveChanges();
                return Json(new { message = "Page type  was delete with success!", success = true });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Json(new { message = "Fail to delete page type!", success = false });
        }
    }
}