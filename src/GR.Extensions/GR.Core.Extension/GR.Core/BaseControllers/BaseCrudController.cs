using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using GR.Core.Abstractions;
using GR.Core.Helpers;

namespace GR.Core.BaseControllers
{
    public abstract class BaseCrudController<TOperationContext, TEntity, TIdentityContext, TEntityContext, TUser, TRole, TTenant, TNotify> : BaseIdentityController<TIdentityContext, TEntityContext, TUser, TRole, TTenant, TNotify>
        where TUser : IdentityUser, IBaseModel
        where TRole : IdentityRole<string>, IBaseModel
        where TTenant : BaseModel
        where TIdentityContext : DbContext
        where TEntityContext : DbContext
        where TOperationContext : DbContext
        where TEntity : class, IBaseModel
    {
        /// <summary>
        /// Inject data filter
        /// </summary>
        protected readonly IDataFilter DataFilter;

        /// <summary>
        /// Inject localizer
        /// </summary>
        protected readonly IStringLocalizer Localizer;

        protected BaseCrudController(UserManager<TUser> userManager, RoleManager<TRole> roleManager, TIdentityContext applicationDbContext, TEntityContext context, TNotify notify, IDataFilter dataFilter, IStringLocalizer localizer) : base(userManager, roleManager, applicationDbContext, context, notify)
        {
            DataFilter = dataFilter;
            Localizer = localizer;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        public virtual IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load paged list with jquery data table
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult LoadPageItems(DTParameters param)
        {
            var filtered = DataFilter.Filter<TEntity, TOperationContext>(IoC.Resolve<TOperationContext>(), param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).ToList();

            var finalResult = new DTResult<TEntity>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }
    }
}
