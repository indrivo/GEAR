using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.Entities.Models.Pages;

namespace ST.PageRender.Razor.Services.Abstractions
{
	public interface IMenuService
	{
		/// <summary>
		/// Get menus
		/// </summary>
		/// <param name="menuId"></param>
		/// <param name="roles"></param>
		/// <returns></returns>
		Task<ResultModel<IEnumerable<MenuViewModel>>> GetMenus(Guid? menuId, IList<string> roles);

		/// <summary>
		/// Get Menu Roles
		/// </summary>
		/// <param name="menuId"></param>
		/// <returns></returns>
		Task<IEnumerable<string>> GetMenuRoles(Guid menuId);

		/// <summary>
		/// Update menu item role access
		/// </summary>
		/// <param name="menuId"></param>
		/// <param name="roles"></param>
		/// <returns></returns>
		Task<ResultModel<Guid>> UpdateMenuItemRoleAccess(Guid menuId, IList<string> roles);

		/// <summary>
		/// Have Access to view menu
		/// </summary>
		/// <param name="userRoles"></param>
		/// <param name="menuItemAllowedRoles"></param>
		/// <returns></returns>
		bool HaveAccess(IList<string> userRoles, string menuItemAllowedRoles);
	}
}
