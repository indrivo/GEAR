using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ST.Core.Abstractions
{
    public interface IDbContext
    {
        /// <summary>
        /// Set entity for work
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbSet<T> SetEntity<T>() where T : class, IBaseModel;

        /// <summary>
        /// Save changes on database
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Save changes async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    }
}
