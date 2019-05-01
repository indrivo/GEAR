using System.Collections.Generic;

namespace ST.WebHost.ViewModels.ProcessViewModels
{
	public class CommandsViewModel
	{
		public ICollection<InputParameters> InputParameters { get; set; }
		public string Name { get; set; }
		public DesignerSettings DesignerSettings { get; set; }
	}
}


