using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ST.Calendar.Abstractions.Models;
using ST.Core.Helpers;
using ST.Core.Extensions;

namespace ST.Calendar.Abstractions.ExternalProviders
{
    public class CalendarExternalTokenProvider : ICalendarExternalTokenProvider
    {
        /// <summary>
        /// Token attribute name
        /// </summary>
        private const string TokenAttr = "auth_token";

        #region Injectable

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly ICalendarDbContext _context;
        #endregion

        public CalendarExternalTokenProvider(ICalendarDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Set token async
        /// </summary>
        /// <typeparam name="TTokenFormat"></typeparam>
        /// <param name="provider"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<TTokenFormat>> GetTokenAsync<TTokenFormat>(string provider, Guid? user) where TTokenFormat : class
        {
            var response = new ResultModel<TTokenFormat>();
            if (provider.IsNullOrEmpty() || !user.HasValue) return response;

            var request = await _context.ExternalProviderTokens
                .FirstOrDefaultAsync(x => x.Attribute.Equals(TokenAttr)
                    && x.ProviderName.Equals(provider) && x.UserId.Equals(user));
            if (request == null) return response;

            response.IsSuccess = true;
            response.Result = request.Value.Deserialize<TTokenFormat>();
            return response;
        }

        /// <summary>
        /// Set token 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetTokenAsync(ExternalProviderToken token)
        {
            var response = new ResultModel();
            if (token == null) return response;
            if (token.ProviderName.IsNullOrEmpty() || token.UserId.Equals(Guid.Empty)) return response;
            var request = await _context.ExternalProviderTokens
                .FirstOrDefaultAsync(x => x.Attribute.Equals(TokenAttr)
                                          && x.ProviderName.Equals(token.ProviderName) && x.UserId.Equals(token.UserId));
            if (request == null)
            {
                token.Attribute = TokenAttr;
                await _context.ExternalProviderTokens.AddAsync(token);
            }
            else
            {
                request.Value = token.Value;
                _context.Update(request);
            }
            return await _context.PushAsync();
        }

        /// <summary>
        /// Delete key
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteTokenAsync(string provider, Guid? user)
        {
            var response = new ResultModel();
            var get = await _context.ExternalProviderTokens
                .FirstOrDefaultAsync(x => x.Attribute.Equals(TokenAttr) && x.ProviderName.Equals(provider) && x.UserId.Equals(user));
            if (get == null) return response;
            _context.ExternalProviderTokens.Remove(get);
            return await _context.PushAsync();
        }
    }
}
