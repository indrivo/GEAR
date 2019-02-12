using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST.CORE.ViewModels.Pages
{
	public class CodeViewModel
	{
		public Guid PageId { get; set; }
		public string Code { get; set; }
		public string Path { get; set; }
		public string Type { get; set; }
	}
}
