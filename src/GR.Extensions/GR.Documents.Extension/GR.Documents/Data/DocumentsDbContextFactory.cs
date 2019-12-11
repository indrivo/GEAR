using GR.Core.Helpers.DbContexts;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace GR.Documents.Data
{
    public class DocumentsDbContextFactory: IDesignTimeDbContextFactory<DocumentsDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DocumentsDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<DocumentsDbContext, DocumentsDbContext>.CreateFactoryDbContext();
        }
    }
}
