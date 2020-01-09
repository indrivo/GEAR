using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions;
using GR.Audit.Abstractions.Helpers;
using GR.Audit.Abstractions.Models;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;

namespace GR.Audit.Contexts
{
    public abstract class TrackerIdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>, ITrackerDbContext
        where TUser : IdentityUser<TKey> where TRole : IdentityRole<TKey> where TKey : IEquatable<TKey>
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        protected TrackerIdentityDbContext(DbContextOptions options) : base(options)
        {
            //Enable tracking for identity db context
            //this.EnableIdentityTracking<TrackerIdentityDbContext<TUser, TRole, TKey>, TUser, TRole, TKey>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Audit tracking entity
        /// </summary>
        public DbSet<TrackAudit> TrackAudits { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Audit track details
        /// </summary>
        public DbSet<TrackAuditDetails> TrackAuditDetails { get; set; }


        /// <summary>
        /// Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual DbSet<T> SetEntity<T>() where T : class, IBaseModel => Set<T>();

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            TrackerFactory.Track(this);
            return base.SaveChanges();
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            TrackerFactory.Track(this);
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Invoke seed
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public abstract Task InvokeSeedAsync(IServiceProvider services);

        /// <summary>
        /// Find by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TIdType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<TEntity>> FindByIdAsync<TEntity, TIdType>(TIdType id)
            where TEntity : class, IBaseModel, IBase<TIdType>
        {
            if (id == null) return new InvalidParametersResultModel<TEntity>();
            var obj = await Set<TEntity>().FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (obj == null) return new NotFoundResultModel<TEntity>();
            return new SuccessResultModel<TEntity>(obj);
        }

        /// <summary>
        /// Remove by id
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TIdType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveByIdAsync<TEntity, TIdType>(TIdType id)
            where TEntity : class, IBaseModel, IBase<TIdType>
        {
            if (id == null) return new InvalidParametersResultModel();
            var objRequest = await FindByIdAsync<TEntity, TIdType>(id);
            if (!objRequest.IsSuccess) return objRequest.ToBase();
            Set<TEntity>().Remove(objRequest.Result);
            return await this.PushAsync();
        }
    }
}