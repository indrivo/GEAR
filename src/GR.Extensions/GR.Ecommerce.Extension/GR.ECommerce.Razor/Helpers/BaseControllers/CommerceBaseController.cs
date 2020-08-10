﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Razor.BaseControllers;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Extensions;
using GR.ECommerce.Abstractions.Helpers;

namespace GR.ECommerce.Razor.Helpers.BaseControllers
{
    [Authorize]
    public abstract class CommerceBaseController<TEntity, TViewModel> : BaseGearController
        where TEntity : BaseModel
        where TViewModel : TEntity
    {
        /// <summary>
        /// Inject commerce db context
        /// </summary>
        protected readonly ICommerceContext Context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        protected CommerceBaseController(ICommerceContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public abstract IActionResult Index();


        /// <summary>
        /// Create view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new item
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Create([Required] TViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            await Context.Set<TEntity>().AddAsync(model);
            var dbResult = await Context.PushAsync();
            if (dbResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AppendResultModelErrors(dbResult.Errors);
            return View(model);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Edit([Required] Guid? id)
        {
            if (id == null) return NotFound();
            var model = await Context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();
            return View(model.Adapt<TViewModel>());
        }

        /// <summary>
        /// Update item
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Edit(TViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddCommerceError(CommerceErrorKeys.InvalidModel);
                return View(model);
            }

            Context.Update<TEntity>(model);
            var dbResult = await Context.PushAsync();
            if (dbResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AppendResultModelErrors(dbResult.Errors);
            return View(model);
        }


        /// <summary>
        /// Ajax ordered list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult OrderedList(DTParameters param)
        {
            var filtered = Context.Set<TEntity>().GetPagedAsDtResultAsync(param).GetAwaiter().GetResult();
            return Json(filtered);
        }
    }
}
