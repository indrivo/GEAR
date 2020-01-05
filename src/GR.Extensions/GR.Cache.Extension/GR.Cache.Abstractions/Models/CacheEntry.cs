namespace GR.Cache.Abstractions.Models
{
    public sealed class CacheEntry
    {
        public CacheEntry()
        {

        }

        public CacheEntry(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public object Value { get; set; }
    }
}
