using System.ComponentModel.DataAnnotations;

namespace ST.CORE.ViewModels.GroupViewModels
{
	public class CreateGroupViewModel
	{
		[Required, StringLength(50)]
		public string Name { get; set; }
	}
}