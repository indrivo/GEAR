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
    }
}
