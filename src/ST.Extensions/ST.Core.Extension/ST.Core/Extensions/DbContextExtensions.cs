using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Abstractions;
using ST.Core.Helpers;

namespace ST.Core.Extensions
{
    public static class DbContextExtensions
    {
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
            if (!local.IsNull())
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
        /// Add scoped context factory
        /// </summary>
        /// <typeparam name="TIContext"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedContextFactory<TIContext, TContext>(this IServiceCollection services)
            where TContext : DbContext, TIContext
            where TIContext : class, IDbContext
        {
            if (!typeof(TIContext).IsInterface)
                throw new Exception($"{nameof(TIContext)} must be an interface in extension {nameof(AddScopedContextFactory)}");

            TIContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TContext>();
                IoC.RegisterScopedService<TIContext, TContext>(context);
                return context;
            }

            services.AddScoped(ContextFactory);
            return services;
        }
    }
}
