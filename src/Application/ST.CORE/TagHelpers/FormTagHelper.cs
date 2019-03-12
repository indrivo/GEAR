using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ST.Entities.Data;
using ST.Identity.Data.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST.CORE.TagHelpers
{
	[HtmlTargetElement("dform")]
	public class FormTagHelper : TagHelper
	{
		/// <summary>
		/// Inject context
		/// </summary>
		private readonly EntitiesDbContext _dbContext;
		/// <summary>
		/// Inject User Manager
		/// </summary>
		private readonly UserManager<ApplicationUser> _userManager;

		/// <summary>
		/// Inject http context
		/// </summary>
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Entity id
		/// </summary>
		public Guid? EntityId { get; set; }

		/// <summary>
		/// Form Id
		/// </summary>
		public Guid? FormId { get; set; }

		/// <summary>
		/// Entity name
		/// </summary>
		public string EntityName { get; set; }


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContextAccessor"></param>
		/// <param name="userManager"></param>
		public FormTagHelper(EntitiesDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
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


			output.Content.SetHtmlContent("");
		}
	}
}
