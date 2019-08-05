﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ST.Core.Helpers.DbContexts;

namespace ST.Forms.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Context factory design
    /// </summary>
    public class FormDbContextFactory : IDesignTimeDbContextFactory<FormDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public FormDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<FormDbContext, FormDbContext>.CreateFactoryDbContext();
        }
    }
}
