using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.Files.Data
{
    public class FileServiceExtension : IDesignTimeDbContextFactory<FileDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public FileDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<FileDbContext, FileDbContext>.CreateFactoryDbContext();
        }
    }
}
