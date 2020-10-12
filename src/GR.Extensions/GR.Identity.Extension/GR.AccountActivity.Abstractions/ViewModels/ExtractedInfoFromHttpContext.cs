namespace GR.AccountActivity.Abstractions.ViewModels
{
    public class ExtractedInfoFromHttpContext
    {
        public virtual string Platform { get; set; }
        public virtual string Browser { get; set; }
        public virtual bool IsMobile { get; set; }
    }
}
