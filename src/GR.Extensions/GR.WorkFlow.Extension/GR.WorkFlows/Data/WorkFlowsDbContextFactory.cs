using GR.Core.Attributes.Documentation;
using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;
using GR.Core.Helpers.Global;

namespace GR.WorkFlows.Data
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Add factory for EF context (used on add new migrations)")]
    public class WorkFlowsDbContextFactory : IDesignTimeDbContextFactory<WorkFlowsDbContext>
    {
        public WorkFlowsDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<WorkFlowsDbContext, WorkFlowsDbContext>.CreateFactoryDbContext();
        }
    }
}
