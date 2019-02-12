using NLog;
using ST.BaseBusinessRepository;
using ST.Entities.ViewModels.DynamicEntities;

namespace ST.Logs.Abstraction
{
    public interface ILogProvider
    {
        void AddLog(LogEventInfo logInfo);

        ResultModel<EntityViewModel> GetLogs(EntityViewModel model);
    }
}