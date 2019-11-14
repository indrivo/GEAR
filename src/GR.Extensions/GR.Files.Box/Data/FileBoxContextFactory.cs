using Microsoft.EntityFrameworkCore.Design;
using GR.Core.Helpers.DbContexts;

namespace GR.Files.Box.Data
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
