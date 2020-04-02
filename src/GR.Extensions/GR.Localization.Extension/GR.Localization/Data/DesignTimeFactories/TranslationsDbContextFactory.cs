using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Localization.Data.DesignTimeFactories
{
    public class TranslationsDbContextFactory : IDesignTimeDbContextFactory<TranslationsDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public TranslationsDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<TranslationsDbContext, TranslationsDbContext>.CreateFactoryDbContext();
        }
    }
}