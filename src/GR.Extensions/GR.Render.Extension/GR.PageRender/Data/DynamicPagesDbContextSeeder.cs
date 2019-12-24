using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Models.ViewModels;

namespace GR.PageRender.Data
{
    public static class DynamicPagesDbContextSeeder<TContext> where TContext : DbContext, IDynamicPagesContext
    {
        /// <summary>
        /// Seed with default data
        /// </summary>
        public static async Task SeedAsync(TContext context)
        {
            context.Validate();
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var entity = JsonParser.ReadObjectDataFromJsonFile<SeedData>(Path.Combine(AppContext.BaseDirectory, "Configuration/ViewModelFieldConfigCodes.json"));
                if (entity == null) return;

                if (entity.ViewModelFieldCodes.Any())
                {
                    if (!context.ViewModelFieldCodesCodes.Any())
                    {
                        context.ViewModelFieldCodesCodes.AddRange(entity.ViewModelFieldCodes);
                        var result = await context.SaveAsync();
                        if (!result.IsSuccess)
                        {
                            Debug.WriteLine(result.Errors);
                        }
                    }
                }
            });
        }

        private sealed class SeedData
        {
            public List<ViewModelFieldCode> ViewModelFieldCodes { get; set; }
        }
    }
}
