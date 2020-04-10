using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;

namespace GR.Identity.Groups.Infrastructure.Data
{
    public class GroupsDbContextFactory : IDesignTimeDbContextFactory<GroupsDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public GroupsDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<GroupsDbContext, GroupsDbContext>.CreateFactoryDbContext();
        }
    }
}