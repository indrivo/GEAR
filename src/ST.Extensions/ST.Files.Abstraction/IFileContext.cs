using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Files.Abstraction.Models;

namespace ST.Files.Abstraction
{
    public interface IFileContext : IDbContext
    {
        DbSet<File> Files { get; set; }
    }
}
