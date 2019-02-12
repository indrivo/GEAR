namespace ST.CORE.Models.ProcessViewModels
{
	public class CodeActionsViewModel
	{
		public string ActionCode { get; set; }
		public string Type { get; set; }
		public bool IsGlobal { get; set; }
		public bool IsAsync { get; set; }
		public string Usings { get; set; }
		public string Name { get; set; }
		public DesignerSettings DesignerSettings { get; set; }
	}
}
