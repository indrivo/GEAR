using System.Collections.Generic;
using Newtonsoft.Json;

namespace GR.ApplePay.Razor.ViewModels
{
    public class AppleAppSiteAssociationViewModel
    {
        [JsonProperty("activitycontinuation")]
        public ActivityContinuation ActivityContinuation { get; set; }  = new ActivityContinuation();
        [JsonProperty("applinks")]
        public AppLinks AppLinks { get; set; } = new AppLinks();
        [JsonProperty("spotlight-image-search")]
        public SpotlightImageSearch SpotlightImageSearch { get; set; } = new SpotlightImageSearch();
    }

    public class ActivityContinuation
    {
        public IEnumerable<object> Apps { get; set; } = new List<object>();
    }

    public class AppLinks
    {
        public IEnumerable<object> Apps { get; set; } = new List<object>();
        public IEnumerable<object> Details { get; set; } = new List<object>();
    }

    public class SpotlightImageSearch
    {
        public IEnumerable<object> Details { get; set; } = new List<object>();
    }
}
