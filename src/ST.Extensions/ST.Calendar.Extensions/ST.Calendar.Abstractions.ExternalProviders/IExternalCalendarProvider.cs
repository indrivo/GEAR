using System;
using System.Threading.Tasks;
using ST.Calendar.Abstractions.Models.ViewModels;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions.ExternalProviders
{
    public interface IExternalCalendarProvider : IDisposable
    {
        /// <summary>
        /// Authorize
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> AuthorizeAsync(Guid? userId);

        /// <summary>
        /// Push new event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        Task<ResultModel> PushEventAsync(GetEventViewModel evt);
    }
}