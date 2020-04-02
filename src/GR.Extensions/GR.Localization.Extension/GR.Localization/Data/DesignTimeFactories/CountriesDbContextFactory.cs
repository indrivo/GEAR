using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Localization.Data.DesignTimeFactories
{
    public class CountriesDbContextFactory : IDesignTimeDbContextFactory<CountriesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CountriesDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<CountriesDbContext, CountriesDbContext>.CreateFactoryDbContext();
        }
    }
}