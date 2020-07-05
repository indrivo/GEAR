using System.Collections.Generic;
using GR.Calendar.Abstractions.Enums;
using GR.Core.Extensions;

namespace GR.Calendar.Abstractions.Models.ViewModels
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
        public Dictionary<string, Dictionary<int, Dictionary<string, string>>> Helpers => new Dictionary<string, Dictionary<int, Dictionary<string, string>>>
        {
            { nameof(EventAcceptance), GetHelperDetails(typeof(EventAcceptance).GetEnumDefinition()) },
            { nameof(EventPriority), GetHelperDetails(typeof(EventPriority).GetEnumDefinition()) },
            { nameof(CalendarTimeLineType), GetHelperDetails(typeof(CalendarTimeLineType).GetEnumDefinition()) },
        };



        /// <summary>
        /// Get helper details
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Dictionary<string, string>> GetHelperDetails(Dictionary<int, string> source)
        {
            var dict = new Dictionary<int, Dictionary<string, string>>();
            foreach (var item in source)
            {
                var details = new Dictionary<string, string>
                {
                    {"systemName", item.Value},
                    {"translationKey", $"system_calendar_{item.Value.ToLowerInvariant()}" }
                };

                dict.Add(item.Key, details);
            }

            return dict;
        }
    }
}
