using System;
using System.Threading.Tasks;
using ST.Calendar.Abstractions.Models;
using ST.Core.Helpers;

namespace ST.Calendar.Abstractions
{
    public interface ICalendarExternalTokenProvider
    {
        Task<ResultModel<TTokenFormat>> GetTokenAsync<TTokenFormat>(string provider, Guid? user) where TTokenFormat : class;
        Task<ResultModel> SetTokenAsync(ExternalProviderToken token);
    }
}