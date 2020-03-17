namespace GR.Localization.Abstractions.Models
{
    public class TranslationItem
    {
        public virtual Language Language { get; set; }
        public virtual string Identifier { get; set; }

        public virtual string Value { get; set; }

        public virtual Translation Translation { get; set; }
    }
}
