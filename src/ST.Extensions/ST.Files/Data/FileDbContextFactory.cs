using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Files.Data
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
