using ST.BaseBusinessRepository;
using ST.Entities.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using ST.CORE.Services.Abstraction;
using ST.DynamicEntityStorage.Abstractions;

namespace ST.CORE.Services
{
	public class MenuService : IMenuService
	{
		/// <summary>
		/// Inject Data Service
		/// </summary>
		private readonly IDynamicService _service;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="service"></param>
		public MenuService(IDynamicService service)
		{
			_service = service;
		}

		/// <inheritdoc />
		/// <summary>
		/// Get menus
		/// </summary>
		/// <param name="menuId"></param>
		/// <param name="roles"></param>
		/// <returns></returns>
		public async Task<ResultModel<IEnumerable<MenuViewModel>>> GetMenus(Guid? menuId, IList<string> roles)
		{
			if (!menuId.HasValue) return default;
			var navbar = await _service.GetByIdSystem<Menu, Menu>(menuId.Value);
			if (!navbar.IsSuccess) return default;
			var search = await _service.GetAll<MenuItem, MenuItem>(x => x.MenuId.Equals(navbar.Result.Id)
																			&& HaveAccess(roles, x.AllowedRoles));
			var menus = search.Result.ToList();

			var data = new List<MenuViewModel>(menus
				.Where(x => x.ParentMenuItemId == null && HaveAccess(roles, x.AllowedRoles))
				.Adapt<IEnumerable<MenuViewModel>>())
				.ToArray();

			if (data == null)
				return default;
			foreach (var t in data)
			{
				t.SubItems = menus.Where(x => x.ParentMenuItemId.Equals(t.Id))
					.Select(x => x.Adapt<MenuViewModel>()).ToArray();

				if (t.SubItems == null) continue;
				{
					for (var j = 0; j < t.SubItems.Length; j++)
					{
						t.SubItems[j].SubItems = menus
							.Where(x => x.ParentMenuItemId.Equals(t.SubItems[j].Id))
							.Select(x => x.Adapt<MenuViewModel>()).ToArray();

						if (t.SubItems[j].SubItems == null) continue;
						{
							for (var m = 0; m < t.SubItems[j].SubItems.Length; m++)
							{
								t.SubItems[j].SubItems[m].SubItems = menus
									.Where(x => x.ParentMenuItemId.Equals(t.SubItems[j].SubItems[m].Id))
									.Select(x => x.Adapt<MenuViewModel>()).ToArray();
							}
						}
					}
				}
			}
			return new ResultModel<IEnumerable<MenuViewModel>>
			{
				IsSuccess = true,
				Result = data
			};
		}

		/// <inheritdoc />
		/// <summary>
		/// Get menu roles
		/// </summary>
		/// <param name="menuId"></param>
		/// <returns></returns>
		public async Task<IEnumerable<string>> GetMenuRoles(Guid menuId)
		{
			var menu = await _service.FirstOrDefault<MenuItem>(x => x.Id == menuId);
			if (!menu.IsSuccess || menu.Result == null) return default;
			var preRoles = menu.Result.AllowedRoles;
			if (string.IsNullOrEmpty(preRoles)) return default;
			var roles = preRoles.Split("#").ToList();
			return roles;
		}

		/// <inheritdoc />
		/// <summary>
		/// Have user access
		/// </summary>
		/// <param name="userRoles"></param>
		/// <param name="menuItemAllowedRoles"></param>
		/// <returns></returns>
		public bool HaveAccess(IList<string> userRoles, string menuItemAllowedRoles)
		{
			if (string.IsNullOrEmpty(menuItemAllowedRoles)) return false;
			if (!userRoles.Any()) return false;
			try
			{
				var menuItemRoles = menuItemAllowedRoles.Split("#");
				if (userRoles.Intersect(menuItemRoles).Any()) return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			return false;
		}

		/// <inheritdoc />
		/// <summary>
		/// Update menu item role access
		/// </summary>
		/// <param name="menuId"></param>
		/// <param name="roles"></param>
		/// <returns></returns>
		public async Task<ResultModel<Guid>> UpdateMenuItemRoleAccess(Guid menuId, IList<string> roles)
		{
			var menu = await _service.FirstOrDefault<MenuItem>(x => x.Id == menuId);
			if (!menu.IsSuccess || menu.Result == null) return default;
			var model = menu.Result;
			if (!roles.Contains("Administrator")) roles.Add("Administrator");
			model.AllowedRoles = string.Join("#", roles.ToArray());

			return await _service.UpdateSystem(model);
		}
	}
}
