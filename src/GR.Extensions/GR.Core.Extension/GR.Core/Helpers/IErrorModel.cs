namespace GR.Core.Helpers
{
    public interface IErrorModel
    {
        string Key { get; set; }
        string Message { get; set; }
        string ToString();
    }
}