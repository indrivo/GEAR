namespace ST.CORE.ViewModels.ProcessViewModels
{
    public class Conditions
    {
		public string Type { get; set; }
		public Action Action { get; set; }
		public string ResultOnPreExecution { get; set; }
		public bool ConditionInversion { get; set; }
	}
}
