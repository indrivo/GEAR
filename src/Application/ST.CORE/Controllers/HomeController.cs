using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.Audit.Extensions;
using ST.BaseBusinessRepository;
using ST.CORE.Models;
using ST.Entities.Extensions;
using ST.Entities.Models.Actions;
using ST.Entities.Models.Home;
using ST.Entities.Services.Abstraction;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Notifications.Abstraction;

namespace ST.CORE.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		#region Inject

		private readonly IBaseBusinessRepository<ApplicationDbContext> _repository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly INotificationHub _hub;
		private readonly ApplicationDbContext _context;
		private readonly IDynamicEntityDataService _dataService;

		#endregion

		string tableHtml = "";
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="userManager"></param>
		/// <param name="hub"></param>
		public HomeController(IBaseBusinessRepository<ApplicationDbContext> repository,
			UserManager<ApplicationUser> userManager, INotificationHub hub, ApplicationDbContext context, IDynamicEntityDataService dataService)
		{
			_repository = repository;
			_userManager = userManager;
			_hub = hub;
			_context = context;
			_dataService = dataService;
		}

		/// <summary>
		/// Dashboard view
		/// </summary>
		/// <returns></returns>
		[Authorize]
		public async Task<IActionResult> Index()
		{
			HomeViewModel model = new HomeViewModel();
			var list = _context.GetTrackedEntities();
			ViewBag.TotalUsers = _hub.GetOnlineUsers().Count();
			ViewBag.TotalSessions = _hub.GetSessionsCount();

			//ViewBag.User = await _userManager.GetUserAsync(User);
			
			int parentNodeId = 1;
			int currentNodeId = 1;
			var parents = await _dataService.Table("Requirement").GetAll<dynamic>(x => x.ParentId == Guid.Empty);
			if (parents.IsSuccess == true)
			{
				foreach (var item in parents.Result)
				{
					tableHtml += "<tr class=\"treegrid-"+ parentNodeId + "\">" +
						            
									 "<td>" +item.Name+"</td>" +
									 //"<td>" + item.Id + "</td>" +
									 "<td></td>" +
								     "<td></td>" +
									 "<td>" +
								         "<h5 class=\"m-t-30\">Progress<span class=\"pull-right\">25%</span></h5>" +
								     	"<div class=\"progress \">" +
										  "<div class=\"progress-bar bg-danger wow animated progress-animated\" style=\"width: 25%; height:6px;\" role=\"progressbar\"> <span class=\"sr-only\">60% Complete</span> </div>" +
									    "</div>" +
								      "</td>" +
									   "<td>" +
									  "<div class=\"btn-group\" role=\"group\" aria-label=\"Action buttons\">" +
									     "<a class=\"btn btn-info btn-sm\" href=\"@Url.Action(\"Edit\")?id = 1\" data-toggle=\"modal\" data-target=\"#AddType\">Action Add</a>" +
										 "<button type=\"button\" class=\"btn btn-danger btn-sm\" onclick=createAlert('1');>"+
											"Delete"+
										"</button>"+
										 "</div>" +
									  "</td>" +
								 "</tr>";
					var requirementActions = await _dataService.Table("RequirementAction").GetAll<dynamic>(x => x.RequirementId == item.Id);
					 AddHtmlChilds( item.Id, parentNodeId, currentNodeId);
					parentNodeId++;
				}
				
			}
			
			 //tableHtml = "<tr class=\"treegrid-1\">" +
				//				 "<td>4 Organisation</td>" +
				//				 "<td></td>" +
				//				 "<td></td>" +
				//				 "<td>" +
				//				   "<h5 class=\"m-t-30\">Progress<span class=\"pull-right\">25%</span></h5>" +
				//				   	"<div class=\"progress \">" +
				//						  "<div class=\"progress-bar bg-danger wow animated progress-animated\" style=\"width: 25%; height:6px;\" role=\"progressbar\"> <span class=\"sr-only\">60% Complete</span> </div>" +
				//					"</div>" +
				//				 "</td>" +
				//				 "<td>" +
				//				 "<div class=\"btn-group\" role=\"group\" aria-label=\"Action buttons\">" +
				//					 "<a class=\"btn btn-info btn-sm\" href=\"@Url.Action(\"Edit\")?id = 1\" data-toggle=\"modal\" data-target=\"#AddType\">Action Add</a>" +
				//				"</div>" +
				//				 "</td>" +
				//				 "</tr>";
			model.TableHtml = tableHtml;
			return View("Index",model);
		}

		[HttpPost]
		
		public IActionResult CreateRequirementAction(string name/*, IDynamicEntityDataService service*/)
		{
			
			if (string.IsNullOrEmpty(name))
			{
				return Json(new { error = true, message = "Error in name!" });
			}
			var activityType = new RequirementAction
			{
				Name = name,
				Author = User.Identity.Name,				
				IsDeleted = false,
				ModifiedBy = User.Identity.Name,
				RequirementId= Guid.Parse("ca74a4cc-441c-4676-be70-db0fedc49267"),
				Treegrid=7
			};
			try
			{
				 //_dataService.AddSystem(activityType);
				return Json(new { success = true, message = "Create successful!" });
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return Json(new { success = false, message = "Error on create!!" });
			}
		}

		public void AddHtmlActions(Guid parentId, int parentNodeId, int currentId, Guid reqId)
		{
			//var requirementActions =  _dataService.Table("RequirementAction").GetAll<dynamic>(x => x.RequirementId == reqId);

			
		}

		public void AddHtmlChilds( Guid parentId,int parentNodeId,int currentId)/* async Task<string>*/
		{
			
			 

			var requirementChilds =  _dataService.Table("Requirement").GetAll<dynamic>(x => x.ParentId == parentId);

			if (requirementChilds.Result.IsSuccess == true)
			{
				foreach (var item in requirementChilds.Result.Result)
				{
					AddHtmlActions(parentId, parentNodeId, currentId,item.Id);
					currentId++;
					tableHtml +="<tr class=\"treegrid-"+currentId+" treegrid-parent-"+parentNodeId+"\" >"+
						            
					               	"<td>" + item.Name + "</td>" +
									 //"<td>" + item.Id + "</td>" +
									 "<td></td>" +
									 "<td></td>" +
									 "<td>" +
										 "<h5 class=\"m-t-30\">Progress<span class=\"pull-right\">0%</span></h5>" +
										 "<div class=\"progress \">" +
										  "<div class=\"progress-bar bg-danger wow animated progress-animated\" style=\"width: 25%; height:6px;\" role=\"progressbar\"> <span class=\"sr-only\">60% Complete</span> </div>" +
										"</div>" +
									  "</td>" +
									  "<td>" +
									  "<div class=\"btn-group\" role=\"group\" aria-label=\"Action buttons\">" +
										 "<a class=\"btn btn-info btn-sm\" href=\"@Url.Action(\"Edit\")?id = 1\" data-toggle=\"modal\" data-target=\"#AddType\">Action Add</a>" +
										 "<button type=\"button\" class=\"btn btn-danger btn-sm\" onclick=createAlert('1');>" +
											"Delete" +
										"</button>" +
										 "</div>" +
									  "</td>" +
								 "</tr>";

					 AddHtmlChilds( item.Id, item.Treegrid, currentId);
				}
			}

			
		}
		public IActionResult Error() =>
			View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}