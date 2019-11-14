namespace GR.Core.Razor.TagHelpersStructures
{
    public sealed class HtmlAttribute
    {
        public HtmlAttribute()
        {

        }

        public HtmlAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}=\"{Value}\" ";
        }
    }
}
