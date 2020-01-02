using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Data;
using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace GR.Forms.Razor.TagHelpers
{
	[HtmlTargetElement("GearForm")]
	public class FormTagHelper : TagHelper
	{
		/// <summary>
		/// Inject localizer
		/// </summary>
		private readonly IStringLocalizer _localizer;
		/// <summary>
		/// Inject context
		/// </summary>
		private readonly EntitiesDbContext _dbContext;
		/// <summary>
		/// Inject User Manager
		/// </summary>
		private readonly UserManager<GearUser> _userManager;

		/// <summary>
		/// Inject http context
		/// </summary>
		private readonly IHttpContextAccessor _httpContextAccessor;

		/// <summary>
		/// Title
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Entity id
		/// </summary>
		public Guid? EntityId { get; set; } = null;

		/// <summary>
		/// Form Id
		/// </summary>
		public Guid? FormId { get; set; } = null;

		/// <summary>
		/// Entity name
		/// </summary>
		public string EntityName { get; set; } = null;


		/// <summary>
		/// Form Style
		/// </summary>
		public FormStyle FormStyle { get; set; } = FormStyle.StandardTemplate;


		/// <summary>
		/// Object on edit
		/// </summary>
		public object AspFor { get; set; }

		/// <summary>
		/// Action
		/// </summary>
		public string AspAction { get; set; } = null;

		/// <summary>
		/// Controller
		/// </summary>
		public string AspController { get; set; } = null;


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="httpContextAccessor"></param>
		/// <param name="userManager"></param>
		/// <param name="localizer"></param>
		public FormTagHelper(EntitiesDbContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<GearUser> userManager, IStringLocalizer localizer)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		/// <inheritdoc />
		/// <summary>
		/// Get html data
		/// </summary>
		/// <param name="context"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			var child = (await output.GetChildContentAsync()).GetContent();
			var body = await PerformTemplate();
			body.Append(child);
			output.Content.SetHtmlContent(body.ToString());
		}

		/// <summary>
		/// Get Input Body
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private async Task<string> GetInputBody(TableModelField field)
		{
			//TODO: On config type render inout body
			var configurations = await _dbContext.TableFieldConfigValues
				.Include(x => x.TableFieldConfig)
				.Where(x => x.TableModelFieldId == field.Id).ToListAsync();

			var content = string.Empty;
			switch (field.DataType)
			{
				case "nvarchar":
					{
						content = FormField.GetInputTextField(field.Id.ToString());
					}
					break;
			}

			if (configurations.Any())
			{
				//Ignore
			}
			return content;
		}

		/// <summary>
		/// Get system fields
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private string GetSystemFields(TableModel table)
		{
			var builder = new StringBuilder();
			builder.Append(FormField.GetHiddenField(nameof(TableModel), table.Id.ToString()));
			builder.Append(FormField.GetHiddenField("__form_token__", Guid.NewGuid().ToString()));
			return builder.ToString();
		}

		/// <summary>
		/// Get table field html body
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private async Task<string> GetTableFieldBody(TableModelField field)
		{
			return string.Format(FormField.FieldBodyTemplate, field.Id, await GetInputBody(field), field.Description, field.DisplayName ?? field.Name);
		}

		/// <summary>
		/// Perform fields
		/// </summary>
		/// <returns></returns>
		private async Task<string> PerformFields()
		{
			var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
			var builder = new StringBuilder();
			if ((EntityName != null) || (EntityId != null && EntityId != Guid.Empty))
			{
				var table = _dbContext.Table.Include(x => x.TableFields)
					.FirstOrDefault(x => ((!string.IsNullOrEmpty(EntityName) && x.Name == EntityName)
					|| (EntityId != null && EntityId != Guid.Empty && x.Id == EntityId)) && x.TenantId == user.TenantId);

				if (table == null) return builder.ToString();
				foreach (var field in table.TableFields)
				{
					builder.Append(await GetTableFieldBody(field));
				}
				builder.Append(GetSystemFields(table));
			}
			else if (FormId != null && FormId != Guid.Empty)
			{

			}
			return builder.ToString();
		}

		/// <summary>
		/// Perform template
		/// </summary>
		/// <returns></returns>
		private async Task<StringBuilder> PerformTemplate()
		{
			var builder = new StringBuilder();
			var template = FormUtil.GetFormBodyByFormStyle(FormStyle);
			var fields = await PerformFields();
			builder.AppendFormat(template, Title, fields, _localizer["save"], _localizer["reset"], _localizer["back"]);
			return builder;
		}
	}
}
