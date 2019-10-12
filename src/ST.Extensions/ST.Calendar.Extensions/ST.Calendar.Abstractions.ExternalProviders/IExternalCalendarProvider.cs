using System;
using System.Threading.Tasks;
using ST.Calendar.Abstractions.ExternalProviders.Helpers;
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
        Task<ResultModel> AuthorizeAsync(Guid? userId, bool reset = false);

        /// <summary>
        /// Push new event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        Task<ResultModel> PushEventAsync(GetEventViewModel evt);

        /// <summary>
        /// Sync new changes with provider
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="evtId"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateEventAsync(GetEventViewModel evt, string evtId);

        /// <summary>
        /// Delete event by id
        /// </summary>
        /// <param name="evtId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteEventAsync(string evtId);

        /// <summary>
        /// Get user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<ExternalCalendarUser>> GetUserAsync();
    }
}