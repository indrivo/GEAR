using System.Threading.Tasks;
using ST.Calendar.Abstractions.ExternalProviders;

namespace ST.Calendar.Providers.Outlook
{
    public class OutlookCalendarProvider : IExternalCalendarProvider
    {
        public Task AuthorizeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
