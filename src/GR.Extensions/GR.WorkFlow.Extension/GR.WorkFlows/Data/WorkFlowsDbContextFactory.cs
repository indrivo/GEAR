using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.WorkFlows.Data
{
    public class WorkFlowsDbContextFactory : IDesignTimeDbContextFactory<WorkFlowsDbContext>
    {
        public WorkFlowsDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<WorkFlowsDbContext, WorkFlowsDbContext>.CreateFactoryDbContext();
        }
    }
}
