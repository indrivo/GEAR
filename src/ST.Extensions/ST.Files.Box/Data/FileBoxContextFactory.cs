using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Files.Box.Data
{
    public class FileBoxContextFactory : IDesignTimeDbContextFactory<FileBoxDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public FileBoxDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<FileBoxDbContext, FileBoxDbContext>.CreateFactoryDbContext();
        }
    }
}
