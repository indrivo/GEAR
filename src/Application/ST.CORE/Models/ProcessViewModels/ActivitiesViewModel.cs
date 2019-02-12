using System.Collections.Generic;

namespace ST.CORE.Models.ProcessViewModels
{
	public class ActivitiesViewModel
	{
		public string State { get; set; }
		public bool IsInitial { get; set; }
		public bool IsFinal { get; set; }
		public bool IsForSetState { get; set; }
		public bool IsAutoSchemeUpdate { get; set; }
		public bool HaveImplementation { get; set; }
		public bool HavePreExecutionImplementation { get; set; }
		public ICollection<Implementation> Implementation { get; set; }
		public ICollection<PreExecutionImplementation> PreExecutionImplementation { get; set; }
		public bool IsState { get; set; }
		public string NestingLevel { get; set; }
		public string Name { get; set; }
		public DesignerSettings DesignerSettings { get; set; }
	}
}
