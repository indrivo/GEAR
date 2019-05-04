using ST.DynamicEntityStorage.Abstractions.Enums;

namespace ST.DynamicEntityStorage.Abstractions.Helpers
{
    public class Filter
    {
        public Criteria Criteria { get; set; }
        public string Parameter { get; set; }
        public object Value { get; set; }
    }
}
