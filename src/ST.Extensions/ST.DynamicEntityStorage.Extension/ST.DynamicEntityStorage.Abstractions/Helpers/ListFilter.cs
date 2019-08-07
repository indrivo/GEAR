namespace ST.DynamicEntityStorage.Abstractions.Helpers
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
