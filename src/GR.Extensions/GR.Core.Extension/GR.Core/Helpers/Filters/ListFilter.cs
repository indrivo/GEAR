namespace GR.Core.Helpers.Filters
{
    public class ListFilter : Filter
    {
        public string SearchValue { get; set; }

        public void SetValue()
        {
            Value = SearchValue;
        }
    }
}
