using ST.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST.CORE.Models.AuditViewModels
{
	public class TrackAuditEnumViewModel
	{
		public string ActionUrl { get; set; }

		public TrackAudit Track { get; set; }
	}
}
