using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using ST.Entities.Data;
using ST.Identity.Abstractions;
using ST.PageRender.Abstractions;

namespace ST.Cms.TagHelpers
{
	[HtmlTargetElement("list")]
	public class ListTagHelper : TagHelper
	{
		/// <summary>
		/// View model id
		/// </summary>
		public Guid? ViewModelId { get; set; }

		/// <summary>
		/// Entity id
		/// </summary>
		public Guid? EntityId { get; set; }

		/// <summary>
		/// Use search
		/// </summary>
		public bool UseSearch { get; set; }

		/// <summary>
		/// Title
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Description
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Content
		/// </summary>
		private string Content { get; set; } = string.Empty;

		/// <summary>
		/// Inject context
		/// </summary>
		private readonly EntitiesDbContext _dbContext;

		/// <summary>
		/// Inject page context
		/// </summary>
		private readonly IDynamicPagesContext _pagesContext;

		/// <summary>
		/// Inject User Manager
		/// </summary>
		private readonly UserManager<ApplicationUser> _userManager;

		/// <summary>
		/// Inject http context
		/// </summary>
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContextAccessor"></param>
		/// <param name="userManager"></param>
		public ListTagHelper(EntitiesDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IDynamicPagesContext pagesContext)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_pagesContext = pagesContext;
			_httpContextAccessor = httpContextAccessor;
		}


		/// <summary>
		/// Get html data
		/// </summary>
		/// <param name="context"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			var childs = (await output.GetChildContentAsync()).GetContent();
			if (!string.IsNullOrEmpty(childs))
			{
				Content = childs;
			}
			var content = await GetHtmlList();
			output.Content.SetHtmlContent(content.ToString());
		}


		/// <summary>
		/// Get html list
		/// </summary>
		/// <returns></returns>
		private async Task<StringBuilder> GetHtmlList()
		{
			var listInstance = Guid.NewGuid();
			var result = new StringBuilder();
			result.AppendFormat(@"
			<div class='card'>
				<div class='card-body'>
				<h4 class='card-title'>{1}</h4>
				<h6 class='card-subtitle'>{2}</h6>
				<div class='table-responsive'>
					{3}
					<table class='table table-striped table-bordered' id='{0}'>
						<thead>
							<tr>
								{4}
							</tr>
						</thead>
						<tbody></tbody>
						<tfoot></tfoot>
					</table>
				</div>
			</div>
		</div>", listInstance, Title, Description, Content, await GetColumns());

			return result;
		}

		/// <summary>
		/// Get list columns
		/// </summary>
		/// <returns></returns>
		private async Task<string> GetColumns()
		{
			var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
			var columns = new StringBuilder();
			if (ViewModelId != null)
			{
				var viewModel = _pagesContext.ViewModels.Include(x => x.ViewModelFields).FirstOrDefault(x => x.Id == ViewModelId.Value);
				if (viewModel != null)
				{
					foreach (var field in viewModel.ViewModelFields.OrderBy(x => x.Order))
					{
						columns.AppendFormat("<th>{0}</th>", field.Name);
					}
					columns.Append("<th>Actions</th>");
				}
			}

			return columns.ToString();
		}
	}
}
