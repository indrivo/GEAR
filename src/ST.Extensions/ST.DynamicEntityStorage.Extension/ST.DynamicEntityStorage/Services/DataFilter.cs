﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.DynamicEntityStorage.Abstractions.Extensions;

namespace ST.DynamicEntityStorage.Services
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
            return context.Filter<TEntity>(search, sortOrder, start, length,
                out totalCount, dbSearch);
        }
    }
}
