using GR.Core;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using GR.Identity.Data.Groups;
using GR.Identity.Data.Permissions;
using GR.Identity.Groups.Razor.ViewModels.GroupViewModels;
using GR.Identity.Permissions.Abstractions.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Groups.Razor.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private ApplicationDbContext Context { get; }

        private IGroupRepository<ApplicationDbContext, GearUser> GroupRepository { get; }

        public GroupsController(ApplicationDbContext context,
            IGroupRepository<ApplicationDbContext, GearUser> groupRepository)
        {
            Context = context;
            GroupRepository = groupRepository;
        }

        /// <summary>
        /// Get list of groups
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmGroupRead)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GroupsList(DTParameters param)
        {
            var filtered = GetGroupsFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);
            var finalResult = new DTResult<AuthGroup>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

        /// <summary>
        /// Get groups filtered
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private List<AuthGroup> GetGroupsFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = Context.AuthGroups.Where(p =>
                search == null || p.Name != null &&
                p.Name.ToLower().Contains(search.ToLower()) || p.Author != null &&
                p.Author.ToLower().Contains(search.ToLower()) || p.ModifiedBy != null &&
                p.ModifiedBy.ToString().ToLower().Contains(search.ToLower()) || p.Created != null &&
                p.Created.ToString(CultureInfo.InvariantCulture).ToLower().Contains(search.ToLower())).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "name":
                    result = result.OrderBy(a => a.Name).ToList();
                    break;

                case "created":
                    result = result.OrderBy(a => a.Created).ToList();
                    break;

                case "author":
                    result = result.OrderBy(a => a.Author).ToList();
                    break;

                case "modifiedBy":
                    result = result.OrderBy(a => a.ModifiedBy).ToList();
                    break;

                case "changed":
                    result = result.OrderBy(a => a.Changed).ToList();
                    break;

                case "isDeleted":
                    result = result.OrderBy(a => a.IsDeleted).ToList();
                    break;

                case "name DESC":
                    result = result.OrderByDescending(a => a.Name).ToList();
                    break;

                case "created DESC":
                    result = result.OrderByDescending(a => a.Created).ToList();
                    break;

                case "author DESC":
                    result = result.OrderByDescending(a => a.Author).ToList();
                    break;

                case "modifiedBy DESC":
                    result = result.OrderByDescending(a => a.ModifiedBy).ToList();
                    break;

                case "changed DESC":
                    result = result.OrderByDescending(a => a.Changed).ToList();
                    break;

                case "isDeleted DESC":
                    result = result.OrderByDescending(a => a.IsDeleted).ToList();
                    break;

                default:
                    result = result.AsQueryable().ToList();
                    break;
            }

            return result.ToList();
        }

        // GET: Groups/Create
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmGroupCreate)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmGroupCreate)]
        public IActionResult Create(CreateGroupViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var authGroup = new AuthGroup
            {
                Name = model.Name,
                Author = User.Identity.Name,
                ModifiedBy = User.Identity.Name
            };
            try
            {
                Context.AuthGroups.Add(authGroup);
                Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "The group already exists.");
            }

            return View(model);
        }

        // GET: Groups/Edit/5
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmGroupUpdate)]
        public IActionResult Edit(Guid? id)
        {
            if (!id.HasValue) return NotFound();

            var authGroup = Context.AuthGroups.FirstOrDefault(x => x.Id == id.Value);
            if (authGroup == null)
                return NotFound();
            return View(authGroup);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmGroupUpdate)]
        public IActionResult Edit([Bind("Name,Id", "Version")] AuthGroup authGroup)
        {
            //if (id != authGroup.Id) return NotFound();

            if (!ModelState.IsValid) return View(authGroup);
            try
            {
                authGroup.ModifiedBy = User.Identity.Name;

                Context.AuthGroups.Update(authGroup);
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthGroupExists(authGroup.Id)) return NotFound();
            }

            return RedirectToAction("Index");
        }

        // POST: Groups/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmGroupDelete)]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (!id.HasValue)
            {
                return Json(false);
            }

            var isUsed = await Context.AuthGroups.AnyAsync(x => x.Id == id);
            if (isUsed)
            {
                return Json(false);
            }

            var group = await Context.AuthGroups.Where(x => x.Id == id).ToListAsync();
            if (!group.Any())
            {
                return Json(false);
            }

            try
            {
                Context.RemoveRange(group);
                Context.SaveChanges();
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        private static bool AuthGroupExists(Guid id)
        {
            return true;
        }

        [HttpGet]
        public JsonResult CheckGroupName(string groupName)
        {
            if (groupName == null)
            {
                return Json(null);
            }

            var result = Context.AuthGroups.FirstAsync(group => group.Name == groupName);
            return Json(result != null);
        }
    }
}