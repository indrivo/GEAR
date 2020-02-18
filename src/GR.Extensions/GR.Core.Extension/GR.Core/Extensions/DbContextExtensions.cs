using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using GR.Core.Helpers.ConnectionStrings;
using GR.Core.Helpers.Responses;
using Microsoft.Extensions.Configuration;

namespace GR.Core.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Get db context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DbContext GetContext(this IDbContext context) => (DbContext)context;

        /// <summary>
        /// Get provider type 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static (DbProviderType, string) GetConnectionStringInfo(this IConfiguration configuration)
            => DbUtil.GetConnectionString(configuration);

        /// <summary>
        /// Check if context is disposed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsDisposed(this DbContext context)
        {
            var result = false;

            try
            {
                var typeDbContext = typeof(DbContext);
                var typeInternalContext = typeDbContext.Assembly.GetType("System.Data.Entity.Internal.InternalContext");
                if (typeInternalContext == null) return false;

                var fiInternalContext = typeDbContext.GetField("_internalContext", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fiInternalContext == null) return false;
                var piIsDisposed = typeInternalContext.GetProperty("IsDisposed");
                var ic = fiInternalContext.GetValue(context);

                if (ic == null) return true;
                if (piIsDisposed != null) result = (bool)piIsDisposed.GetValue(ic);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        /// <summary>
        /// Detach local
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="t"></param>
        /// <param name="entryId"></param>
        public static void DetachLocal<T>(this DbContext context, T t, Guid entryId)
            where T : class, IBase<Guid>
        {
            Arg.NotNull(context, nameof(DetachLocal));
            var local = context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));
            if (local != null)
                context.Entry(local).State = EntityState.Detached;

            context.Entry(t).State = EntityState.Modified;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ResultModel Save(this DbContext context)
        {
            var rs = new ResultModel();
            if (context.IsDisposed())
            {
                rs.Errors.Add(new ErrorModel("Error", "Context was disposed!"));
                return rs;
            }

            try
            {
                context.SaveChanges();
                rs.IsSuccess = true;
            }
            catch (Exception e)
            {
                rs.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
            }

            return rs;
        }

        /// <summary>
        /// Save changes async
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<ResultModel> SaveAsync(this DbContext context)
        {
            var rs = new ResultModel();
            if (context.IsDisposed())
            {
                rs.Errors.Add(new ErrorModel("Error", "Context was disposed!"));
                return rs;
            }

            try
            {
                await context.SaveChangesAsync();
                rs.IsSuccess = true;
            }
            catch (Exception e)
            {
                rs.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
                rs.Errors.Add(new ErrorModel(nameof(Exception), e.InnerException?.Message));
            }

            return rs;
        }

        /// <summary>
        /// Save async
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<ResultModel> PushAsync(this IDbContext context)
        {
            return await ((DbContext)context).SaveAsync();
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ResultModel Push(this IDbContext context)
        {
            return ((DbContext)context).Save();
        }

        /// <summary>
        /// Deleted
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="eQuery"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> Deleted<TEntity>(this IQueryable<TEntity> eQuery)
            where TEntity : class, IBaseModel
        {
            return eQuery.AsNoTracking().Where(x => x.IsDeleted);
        }

        /// <summary>
        /// Deleted
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="eQuery"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> NonDeleted<TEntity>(this IQueryable<TEntity> eQuery)
            where TEntity : class, IBaseModel
        {
            return eQuery.AsNoTracking().Where(x => !x.IsDeleted);
        }

        /// <summary>
        /// Disable entry
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ResultModel> DisableRecordAsync<TEntity>(this IDbContext self, Guid? id)
            where TEntity : class, IBaseModel, IBase<Guid>
        {
            if (!id.HasValue) return new InvalidParametersResultModel();
            var dbObject = await self.SetEntity<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (dbObject == null) return new NotFoundResultModel();
            dbObject.IsDeleted = true;
            self.Update(dbObject);
            return await self.PushAsync();
        }

        /// <summary>
        /// Activate entry
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ResultModel> ActivateRecordAsync<TEntity>(this IDbContext self, Guid? id)
            where TEntity : class, IBaseModel, IBase<Guid>
        {
            if (!id.HasValue) return new InvalidParametersResultModel();
            var dbObject = await self.SetEntity<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (dbObject == null) return new NotFoundResultModel();
            dbObject.IsDeleted = false;
            self.Update(dbObject);
            return await self.PushAsync();
        }

        /// <summary>
        /// Remove permanent
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<ResultModel> RemovePermanentRecordAsync<TEntity>(this IDbContext self, Guid? id)
            where TEntity : class, IBaseModel, IBase<Guid>
        {
            if (!id.HasValue) return new InvalidParametersResultModel();
            var dbObject = await self.SetEntity<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (dbObject == null) return new NotFoundResultModel();
            dbObject.IsDeleted = false;
            ((DbContext)self).Remove(dbObject);
            return await self.PushAsync();
        }

        /// <summary>
        /// Find by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<TEntity> FindByIdAsync<TEntity>(this DbSet<TEntity> self, Guid? id)
            where TEntity : class, IBaseModel, IBase<Guid>
        {
            Arg.NotNull(self, nameof(FindByIdAsync));
            if (id == null) return default;
            return await self.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id));
        }
    }
}
