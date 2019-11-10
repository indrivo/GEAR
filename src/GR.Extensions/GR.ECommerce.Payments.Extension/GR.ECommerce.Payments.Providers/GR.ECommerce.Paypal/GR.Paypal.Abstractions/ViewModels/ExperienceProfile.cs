using Newtonsoft.Json;

namespace GR.Paypal.Abstractions.ViewModels
{
    public class ExperienceProfile
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("temporary")] public bool Temporary { get; set; }
        [JsonProperty("input_fields")] public InputFields InputFields { get; set; }
    }

    public class InputFields
    {
        [JsonProperty("no_shipping")] public int NoShipping { get; set; }
    }
}