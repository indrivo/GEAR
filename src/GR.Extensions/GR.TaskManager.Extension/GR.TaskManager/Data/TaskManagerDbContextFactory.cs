using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.TaskManager.Data
{
    public class TaskManagerExtension : IDesignTimeDbContextFactory<TaskManagerDbContext>
    {
        public TaskManagerDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<TaskManagerDbContext, TaskManagerDbContext>.CreateFactoryDbContext();
        }
    }
}
