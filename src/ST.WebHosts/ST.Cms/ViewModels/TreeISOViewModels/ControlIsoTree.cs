using System;
using System.Collections.Generic;

namespace ST.Cms.ViewModels.TreeISOViewModels
{
	public sealed class ControlRootTree
	{
		public Guid Id { get; set; }
		public string Number { get; set; }
		public string Name { get; set; }
		public ICollection<ControlSecondLevel> SecondLevels { get; set; }
		public string CollapseSelectors { get; set; }
	}

	public class ControlLevel
	{
		public Guid ParentId { get; set; }
		public Guid Id { get; set; }
		public string Number { get; set; }
		public string Name { get; set; }
	}

	public sealed class ControlSecondLevel : ControlLevel
	{
		public string Goal { get; set; }
		public ICollection<ControlThirdLevel> ThirdLevels { get; set; }
		public string CollapseSelectors { get; set; }
	}

	public sealed class ControlThirdLevel : ControlLevel
	{
		public string Content { get; set; }
		public ControlRisks ControlRisks { get; set; }
		public ControlActivities ControlActivities { get; set; }
		public ControlDocuments ControlDocuments { get; set; }
		public ControlDetails ControlDetails { get; set; }
	}

	public sealed class ControlResponsible
	{
		public Guid ControlDetailId { get; set; }
		public Guid Id { get; set; }
		public Guid NomPersonId { get; set; }
		public string Initials { get; set; }
		public string FullName { get; set; }
		public string BackgroundColor { get; set; }
	}

	public sealed class ControlDetails
	{
		public Guid? ControlDetailId { get; set; }
		public bool Applicability { get; set; }
		public bool Implemented { get; set; }
		public string Details { get; set; }
		public string Comments { get; set; }
		public string Motivation { get; set; }
		public IEnumerable<ControlResponsible> Responsibles { get; set; }
	}

	public sealed class ControlRisks
	{
		public int TotalRisks { get; set; }
	}

	public sealed class ControlActivities
	{
		public int OpenActivities { get; set; }
		public int ClosedActivities { get; set; }
	}

	public sealed class ControlDocuments
	{
		public int TotalDocuments { get; set; }
	}
}
