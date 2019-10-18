using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GR.Process.Razor.ViewModels.ProcessViewModels
{
	public class ProcessSchemeViewModel
	{
		public ICollection<ActorViewModel> Actors { get; set; }
		public ICollection<ParametersViewModel> Parameters { get; set; }
		public ICollection<CommandsViewModel> Commands { get; set; }
		public ICollection<TimersViewModel> Timers { get; set; }
		public ICollection<ActivitiesViewModel> Activities { get; set; }
		public ICollection<TransitionsViewModel> Transitions { get; set; }
		public ICollection<LocalizationViewModel> Localization { get; set; }
		public ICollection<CodeActionsViewModel> CodeActions { get; set; }
		public AdditionalParamsViewModel AdditionalParams { get; set; }
		public dynamic StartingTransition { get; set; }
		public ICollection<ParametersForSerialize> ParametersForSerialize { get; set; }
		public ICollection<dynamic> PersistenceParameters { get; set; }
		public dynamic DefiningParametersString { get; set; }
		public dynamic RootSchemeCode { get; set; }
		public dynamic RootSchemeId { get; set; }
		public bool IsObsolete { get; set; }
		public Guid Id { get; set; }
		public bool IsSubprocessScheme { get; set; }
		public dynamic AllowedActivities { get; set; }
		public string Name { get; set; }
		public DesignerSettings DesignerSettings { get; set; }
		public override string ToString()
		{
			var model = JsonConvert.SerializeObject(this);
			return model;
		}
	}
}
