using GR.Core;
using GR.Core.Attributes;
using GR.Core.Helpers;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace GR.PageRender.Razor.Controllers
{
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
        public JsonResult LoadPages(DTParameters param)
        {
            var filtered = _pagesContext.FilterAbstractContext<PageType>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount);

            var finalResult = new DTResult<PageType>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count()
            };
            return Json(finalResult);
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