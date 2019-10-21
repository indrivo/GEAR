using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;

namespace GR.DynamicEntityStorage.Services
{
    public class DataFilter : IDataFilter
    {
        /// <summary>
        /// Filter data from entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="context"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="dbSearch"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Filter<TEntity, TContext>(TContext context, string search, string sortOrder, int start, int length,
            out int totalCount, Func<TEntity, bool> dbSearch = null) where TEntity : class, IBaseModel where TContext : DbContext
        {
            return context.Filter(search, sortOrder, start, length,
                out totalCount, dbSearch);
        }

        /// <summary>
        /// Filter abstract entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TAbstractContext"></typeparam>
        /// <param name="context"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="dbSearch"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> FilterAbstractEntity<TEntity, TAbstractContext>(TAbstractContext context, string search, string sortOrder, int start, int length,
            out int totalCount, Func<TEntity, bool> dbSearch = null) where TEntity : class, IBaseModel where TAbstractContext : IDbContext
        {
            return context.FilterAbstractContext(search, sortOrder, start, length,
                out totalCount, dbSearch);
        }
    }
}
