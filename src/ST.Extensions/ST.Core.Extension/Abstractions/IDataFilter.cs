using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ST.Core.Abstractions
{
    public interface IDataFilter
    {
        IEnumerable<TEntity> Filter<TEntity, TContext>(TContext context, string search, string sortOrder, int start, int length,
            out int totalCount, Func<TEntity, bool> dbSearch = null) where TEntity : class, IBaseModel where TContext : DbContext;
    }
}
