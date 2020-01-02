
namespace GR.Forms.Razor.TagHelpers
{
	public enum FormStyle
	{
		StandardTemplate, EditTemplate
	}

	public static class FormUtil
	{
		/// <summary>
		/// Get form body
		/// </summary>
		/// <param name="formStyle"></param>
		/// <returns></returns>
		public static string GetFormBodyByFormStyle(FormStyle formStyle)
		{
			return typeof(FormBody).GetField(formStyle.ToString())?.GetValue(new FormBody())?.ToString();
		}
	}

	public class FormBody
	{
		/// <summary>
		/// Edit template
		/// </summary>
		public const string EditTemplate = "";

		/// <summary>
		/// Form template
		/// </summary>
		public const string StandardTemplate = @"
<div class='card card-outline-primary'>
	<div class='card-header'>
		<h4 class='mb-0 text-white'>{0}</h4>
	</div>
	<div class='card-body'>
		<form asp-action='' role='form'>
			<div class='form-body'>
				<div asp-validation-summary='ModelOnly' class='text-danger'></div>
				<div class='row p-t-20'>
					{1}
				</div>
			</div>
			<div class='row button-group'>
				<div class='col-lg-2 col-md-3'>
					<button type='submit' class='btn btn-block btn-success'> <i class='fa fa-check'></i> {2}</button>
				</div>
				<div class='col-lg-2 col-md-3'>
					<button type='reset' class='btn btn-block  btn-inverse'> {3}</button>
				</div>
				<div class='col-lg-2 col-md-3'>
					<a asp-action='Index' class='btn btn-block btn-link'> {4}</a>
				</div>
			</div>
		</form>
	</div>
</div>
";
	}

	public static class FormField
	{
		public const string FieldBodyTemplate = @"
					<div class='col-md-12'>
						<div class='form-group'>
							<label class='control-label' for='{0}'>{3}</label>
							{1}
							<span asp-validation-for='{0}' class='text-danger'></span><br />
							<small>{2}</small>
						</div>
					</div>";


        /// <summary>
        /// Get text input
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetInputTextField(string name, string value = null)
		{
			return string.Format("<input class='form-control' value='{1}' type='text' name='{0}' id='{0}' />", name, value);
		}

		/// <summary>
		/// Get hidden input
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetHiddenField(string name, string value = null)
		{
			return string.Format("<input class='form-control' value='{1}' type='hidden' name='{0}' id='{0}' />", name, value);
		}
	}
}
