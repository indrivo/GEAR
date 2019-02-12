using NLog;
using ST.BaseBusinessRepository;
using ST.Entities.Data;
using ST.Entities.ViewModels.DynamicEntities;
using ST.Logs.Abstraction;

namespace ST.Logs.Providers
{
    public class DbLogProvider : ILogProvider
    {
        private readonly EntitiesDbContext _context;

        public DbLogProvider(EntitiesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        ///     Add log
        /// </summary>
        /// <param name="logInfo"></param>
        public void AddLog(LogEventInfo logInfo)
        {
            
        }

        /// <summary>
        ///     Get logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel<EntityViewModel> GetLogs(EntityViewModel model)
        {
            var result = _context.ListEntitiesByParams(model);
            return result;
        }
    }
}