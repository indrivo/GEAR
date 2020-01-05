using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Helpers.ConnectionStrings;
using GR.DynamicEntityStorage.Abstractions;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;
using GR.Entities.Data;
using GR.Forms.Abstractions;
using GR.Identity.Data;
using GR.PageRender.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GR.WebApplication.Services
{
    public static class ExportManager
    {
        /// <summary>
        /// CreateZipArchive data async
        /// </summary>
        /// <returns></returns>
        public static async Task<(MemoryStream, string, string)> ExportAsync()
        {
            var entitiesDbContext = IoC.Resolve<EntitiesDbContext>();
            var dynamicService = IoC.Resolve<IDynamicService>();
            var applicationDbContext = IoC.Resolve<ApplicationDbContext>();
            var formContext = IoC.Resolve<IFormContext>();
            var pageContext = IoC.Resolve<IDynamicPagesContext>();
            var dynamicEntities = entitiesDbContext.Table
                .Where(x => !x.IsPartOfDbContext)
                .Include(x => x.TableFields);

            //var entityFrameWorkEntities = entitiesDbContext.Table
            //    .Where(x => x.IsPartOfDbContext)
            //    .Include(x => x.TableFields);

            var dynamicData = new Dictionary<string, IEnumerable<object>>();
            var frameworkData = new Dictionary<string, IEnumerable<object>>();

            //Extract values from from dynamic entities
            foreach (var entity in dynamicEntities)
            {
                var req = await dynamicService.Table(entity.Name).GetAllWithInclude<object>();
                if (req.IsSuccess)
                {
                    dynamicData.Add(entity.Name, req.Result);
                }
            }

            //Extract values from Entity FrameWork DbSet declarations
            //foreach (var entity in entityFrameWorkEntities)
            //{
            //    var req = await dynamicService.Table(entity.Name).GetAll<object>();
            //    if (req.IsSuccess)
            //    {
            //        frameworkData.Add(entity.Name, req.Result);
            //    }
            //}

            var zipStream = ExportDataIo.CreateZipArchive(new Dictionary<string, MemoryStream>
            {
                {
                    "forms.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(formContext.Forms
                        .Include(x => x.Columns)
                        .Include(x => x.Rows)
                        .Include(x => x.Fields)
                        .Include(x => x.Stages)
                        .Include(x => x.Settings).ToList())))
                },
                {
                    "pages.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(pageContext.Pages
                        .Include(x => x.PageScripts)
                        .Include(x => x.PageStyles)
                        .Include(x => x.PageType)
                        .Include(x => x.Settings).ToArray())))
                },
                {
                    "dynamicEntities.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(dynamicEntities)))
                },
                {
                    "templates.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(pageContext.Templates.ToList())))
                },
                {
                    "roles.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(applicationDbContext.Roles.ToList())))
                },
                {
                    "tenants.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(applicationDbContext.Tenants.ToList())))
                },
                {
                    "users.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(applicationDbContext.Users.ToList())))
                },
                {
                    "userRoles.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(applicationDbContext.UserRoles.ToList())))
                },
                {
                    "rolePermissions.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(applicationDbContext.RolePermissions.ToList())))
                },
                {
                    "blocksCategories.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(pageContext.BlockCategories.ToList())))
                },
                {
                    "blocks.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(pageContext.Blocks.ToList())))
                },
                {
                    "dynamicEntitiesData.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(dynamicData)))
                },
                {
                    "frameworkEntitiesData.json", new MemoryStream(Encoding.ASCII.GetBytes(Serialize(frameworkData)))
                }
            });
            var date = DateTime.Now;
            return (zipStream, "application/octet-stream", $"export_system_{date.Minute}_{date.Hour}_{date.Day}_{date.Month}_{date.Year}.zip");
        }

        /// <summary>
        /// Import async
        /// </summary>
        /// <param name="memStream"></param>
        /// <returns></returns>
        public static ResultModel Import(MemoryStream memStream)
        {
            var result = new ResultModel();

            ExportDataIo.Decompress(memStream, async zip =>
           {
               var context = IoC.Resolve<EntitiesDbContext>();
               var tableService = IoC.Resolve<ITablesService>();
               var dynamicContext = IoC.Resolve<IDynamicService>();
               var dynamicValues = await zip.Entries.GetDataFromZipArchiveEntry<IDictionary<string, IEnumerable<object>>>("dynamicEntitiesData.json");
               if (dynamicValues == null) return;

               //Import dynamic entities
               var dynamicEntities = await zip.Entries.GetDataFromZipArchiveEntry<List<TableModel>>("dynamicEntities.json");
               if (dynamicEntities == null) return;
               foreach (var entity in dynamicEntities)
               {
                   if (await context.Table.AnyAsync(x => x.Name == entity.Name && x.TenantId == entity.TenantId))
                       continue;
                   await context.Table.AddAsync(entity);
                   tableService.CreateSqlTable(entity, context.GetConnectionString());
                   foreach (var tableField in entity.TableFields)
                   {
                       tableService.AddFieldSql(new CreateTableFieldViewModel
                       {
                           Name = tableField.Name,
                           DisplayName = tableField.DisplayName,
                           AllowNull = tableField.AllowNull,
                           Id = tableField.Id,
                           TableId = entity.Id,
                           DataType = tableField.DataType,
                           Description = tableField.Description
                       }, entity.Name, context.GetConnectionString(), true, entity.EntityType);
                   }
                   if (dynamicValues.ContainsKey(entity.Name))
                   {
                       await dynamicContext.Table(entity.Name).AddRange(dynamicValues[entity.Name]);
                   }
               }
               await context.SaveChangesAsync();
           });

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string Serialize<TObject>(TObject obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return string.Empty;
        }
    }
}
