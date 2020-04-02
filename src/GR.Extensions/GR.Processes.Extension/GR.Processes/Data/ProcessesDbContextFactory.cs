using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Procesess.Data
{
    public class ProcessesDbContextFactory : IDesignTimeDbContextFactory<ProcessesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ProcessesDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<ProcessesDbContext, ProcessesDbContext>.CreateFactoryDbContext();
        }
    }
}
