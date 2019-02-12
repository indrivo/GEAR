using NLog;
using ST.BaseBusinessRepository;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Logs.Abstraction;

namespace ST.Logs.Services
{
    public class BpmnLogger
    {
        private readonly ILogProvider _logProvider;

        public BpmnLogger(ILogProvider logProvider)
        {
            _logProvider = logProvider;
        }

        /// <summary>
        ///     Add log
        /// </summary>
        /// <param name="logInfo"></param>
        public void AddLog(LogEventInfo logInfo)
        {
            _logProvider.AddLog(logInfo);
        }

        /// <summary>
        ///     Get logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> GetLogs(EntityViewModel model)
        {
            return _logProvider.GetLogs(model);
        }
    }
}