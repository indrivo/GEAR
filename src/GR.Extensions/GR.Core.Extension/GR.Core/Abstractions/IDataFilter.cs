using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GR.Core.Abstractions
{
    public interface IDataFilter
    {
        IEnumerable<TEntity> Filter<TEntity, TContext>(TContext context, string search, string sortOrder, int start, int length,
            out int totalCount, Func<TEntity, bool> dbSearch = null) where TEntity : class, IBaseModel where TContext : DbContext;


        IEnumerable<TEntity> FilterAbstractEntity<TEntity, TAbstractContext>(TAbstractContext context, string search,
            string sortOrder, int start, int length,
            out int totalCount, Func<TEntity, bool> dbSearch = null) where TEntity : class, IBaseModel
            where TAbstractContext : IDbContext;
    }
}
