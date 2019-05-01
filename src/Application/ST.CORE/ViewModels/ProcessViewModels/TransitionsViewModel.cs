using System.Collections.Generic;

namespace ST.WebHost.ViewModels.ProcessViewModels
{
	public class TransitionsViewModel
	{
		public From From { get; set; }
		public To To { get; set; }
		public string Classifier { get; set; }
		public Trigger Trigger { get; set; }
		public ICollection<Conditions> Conditions { get; set; }
		public ICollection<Restrictions> Restrictions { get; set; }
		public string AllowConcatenationType { get; set; }
		public string RestrictConcatenationType { get; set; }
		public string ConditionsConcatenationType { get; set; }
		public bool IsAlwaysTransition { get; set; }
		public bool IsOtherwiseTransition { get; set; }
		public bool IsConditionTransition { get; set; }
		public bool IsFork { get; set; }
		public bool MergeViaSetState { get; set; }
		public bool DisableParentStateControl { get; set; }
		public string ForkType { get; set; }
		public string Name { get; set; }
		public DesignerSettings DesignerSettings { get; set; }
	}
}
