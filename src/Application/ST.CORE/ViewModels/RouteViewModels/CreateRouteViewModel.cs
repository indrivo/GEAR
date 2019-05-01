namespace ST.WebHost.ViewModels.RouteViewModels
{
	public class CreateRouteViewModel
	{
		public string Name { get; set; }
		public string Alias { get; set; }
		public bool HaveParent { get; set; }
		public bool HaveChilds { get; set; }
		public string Path { get; set; }
		public string ComponentName { get; set; }
		public string Description { get; set; }
	}
}