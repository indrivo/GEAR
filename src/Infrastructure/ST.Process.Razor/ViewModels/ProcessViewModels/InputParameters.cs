namespace ST.Process.Razor.ViewModels.ProcessViewModels
{
	public class InputParameters
	{
		public string Name { get; set; }
		public bool IsRequired { get; set; }
		public string DefaultValue { get; set; }
		public Parameter Parameter { get; set; }
	}
}
