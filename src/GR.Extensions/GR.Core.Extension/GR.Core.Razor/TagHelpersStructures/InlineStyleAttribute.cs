namespace GR.Core.Razor.TagHelpersStructures
{
    public class InlineStyleAttribute
    {
        public InlineStyleAttribute()
        {

        }
        public InlineStyleAttribute(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return $"{Name}:{Value};";
        }
    }
}
