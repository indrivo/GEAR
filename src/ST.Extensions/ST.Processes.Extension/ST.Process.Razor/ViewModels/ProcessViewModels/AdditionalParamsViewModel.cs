using System.Collections.Generic;

namespace ST.Process.Razor.ViewModels.ProcessViewModels
{
	public class AdditionalParamsViewModel
	{
		public ICollection<dynamic> Rules { get; set; }
		public ICollection<string> TimerTypes { get; set; }
		public ICollection<Conditions> Conditions { get; set; }
		public ICollection<dynamic> Actions { get; set; }
		public ICollection<string> Usings { get; set; }
		public ICollection<string> Types { get; set; }
	}
}
