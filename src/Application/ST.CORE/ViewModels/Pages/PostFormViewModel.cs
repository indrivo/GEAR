using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST.CORE.ViewModels.Pages
{
	public class PostFormViewModel
	{
		public Dictionary<string, string> Data { get; set; }
		public Guid FormId { get; set; }
	}
}
