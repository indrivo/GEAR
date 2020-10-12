using System;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Logger.Abstractions;
using GR.Logger.Abstractions.ViewModels;
using Seq.Api;

namespace GR.Logger
{
    public class LoggerService : ILoggerService
    {
        private SeqConnection _connection;
        private readonly TimeSpan _timeOut;

        #region Injectable

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        public LoggerService(IMapper mapper)
        {
            _mapper = mapper;
            _timeOut = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Get logs with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<LogEventViewModel>> GetLogsWithPaginationAsync(DTParameters parameters)
        {
            var connectionCheck = await GetConnectionAsync();
            if (!connectionCheck.IsSuccess) return null;
            //TODO: Write implementation of logger
            var events = await _connection.Events.ListAsync(count: parameters.Draw * parameters.Length);
            var result = events.AsAsyncQueryable().GetPagedAsDtResult(parameters);
            return _mapper.Map<DTResult<LogEventViewModel>>(result);
        }

        #region Helpers

        private async Task<ResultModel<SeqConnection>> GetConnectionAsync()
        {
            if (_connection == null)
                _connection = new SeqConnection("http://localhost:5341");
            await _connection.EnsureConnectedAsync(_timeOut);

            return new SuccessResultModel<SeqConnection>(_connection);
        }

        #endregion
    }
}
