namespace ST.Identity.Abstractions
{
    public interface ILocalService
    {
        string GetAppName(string stage);
        void SetAppName(string app, string name);
    }
}