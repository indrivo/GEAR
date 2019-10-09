using System;
using System.Threading.Tasks;
using ST.Calendar.Abstractions.ExternalProviders;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Helpers;

namespace ST.Calendar.Providers.Outlook
{
    public class OutlookCalendarProvider : IExternalCalendarProvider
    {
        public virtual Task<ResultModel> AuthorizeAsync(Guid? userId)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ResultModel> PushEventAsync(GetEventViewModel evt)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
