namespace ST.CORE.Models.ProcessViewModels
{
	public class Trigger
	{
		public string Type { get; set; }
		public string NameRef { get; set; }
		public CommandsViewModel Command { get; set; }
		public TimersViewModel Timer { get; set; }
	}
}
