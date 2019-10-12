using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Calendar.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class CalendarDbContextDbContextFactory : IDesignTimeDbContextFactory<CalendarDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CalendarDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<CalendarDbContext, CalendarDbContext>.CreateFactoryDbContext();
        }
    }
}
