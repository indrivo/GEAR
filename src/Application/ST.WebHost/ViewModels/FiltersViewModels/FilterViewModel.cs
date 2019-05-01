namespace ST.WebHost.ViewModels.FiltersViewModels
{
	public class FilterViewModel
	{
		public int Page { get; set; }
		public int PageSize { get; set; }
		public bool IsSortable { get; set; }
		public int Column { get; set; }
		public bool IsAscended { get; set; }
		public string Key { get; set; }
		public bool IsDeleted { get; set; }
		public bool ShowAll { get; set; }
	}
}