using System.Collections.Generic;
using ST.Calendar.Abstractions.Enums;
using ST.Core.Extensions;

namespace ST.Calendar.Abstractions.Models.ViewModels
{
    public class GetEventWithHelpersViewModel
    {
        /// <summary>
        /// Events
        /// </summary>
        public IEnumerable<GetEventViewModel> Events { get; set; } = new List<GetEventViewModel>();

        /// <summary>
        /// Helpers
        /// </summary>
        public Dictionary<string, Dictionary<int, string>> Helpers => new Dictionary<string, Dictionary<int, string>>
        {
            { nameof(EventAcceptance), typeof(EventAcceptance).GetEnumDefinition() },
            { nameof(EventPriority), typeof(EventPriority).GetEnumDefinition() },
            { nameof(CalendarTimeLineType), typeof(CalendarTimeLineType).GetEnumDefinition() },
        };
    }
}
