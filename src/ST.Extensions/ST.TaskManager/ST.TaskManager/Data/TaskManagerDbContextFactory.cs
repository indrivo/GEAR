using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.TaskManager.Data
{
    public class TaskManagerExtension : IDesignTimeDbContextFactory<TaskManagerDbContext>
    {
        public TaskManagerDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<TaskManagerDbContext, TaskManagerDbContext>.CreateFactoryDbContext();
        }
    }
}
