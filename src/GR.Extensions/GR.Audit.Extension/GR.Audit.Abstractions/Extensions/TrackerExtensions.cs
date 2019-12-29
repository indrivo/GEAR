using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions.Enums;
using GR.Audit.Abstractions.Helpers;
using GR.Audit.Abstractions.Models;
using GR.Core.Extensions;

namespace GR.Audit.Abstractions.Extensions
{
    public static class TrackerExtensions
    {
        public static TrackAudit GetTrackAuditFromDictionary(this Dictionary<string, object> keys,
            string contextName, Guid? tenantId, Type objectType, TrackEventType eventType)
        {
            if (keys == null) return null;
            var details = new List<TrackAuditDetails>();
            var audit = new TrackAudit
            {
                Created = DateTime.Now,
                Changed = DateTime.Now,
                DatabaseContextName = contextName,
                TenantId = tenantId,
                TypeFullName = objectType?.FullName,
                TrackEventType = eventType,
                Version = 1
            };

            if (keys.ContainsKey("Id"))
            {
                audit.RecordId = keys["Id"].ToString().ToGuid();
            }

            if (eventType == TrackEventType.Updated)
            {
                if (keys.ContainsKey("Version"))
                {
                    try
                    {
                        var version = Convert.ToInt32(keys["Version"]);
                        audit.Version = version;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }

            foreach (var prop in keys)
            {
                try
                {
                    var detailObject = new TrackAuditDetails
                    {
                        PropertyName = prop.Key,
                        PropertyType = prop.Value?.GetType().FullName,
                        Value = prop.Value?.ToString(),
                        TenantId = tenantId
                    };

                    details.Add(detailObject);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            audit.AuditDetailses = details;
            return audit;
        }

        /// <summary>
        /// Get track audit from object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="contextName"></param>
        /// <param name="tenantId"></param>
        /// <param name="objectType"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static TrackAudit GetTrackAuditFromObject(this object obj,
            string contextName, Guid? tenantId, Type objectType, TrackEventType eventType)
        {
            return GetTrackAuditFromDictionary(GetDictionary(obj), contextName, tenantId, objectType, eventType);
        }


        /// <summary>
        /// Store audit in context
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="audit"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool Store<TContext>(this TrackAudit audit, TContext context) where TContext : DbContext, ITrackerDbContext
        {
            try
            {
                context.Add(audit);
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Get tracked entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetTrackedEntities<T>(this T context) where T : DbContext
        {
            var result = new List<Type>();
            var dbSets = context.GetType().GetProperties();
            foreach (var dbSet in dbSets)
            {
                var genericType = dbSet.PropertyType.GenericTypeArguments.FirstOrDefault();
                if (genericType == null) continue;
                var match = TrackerFactory.IsTrackable(genericType).Item1;
                if (match) result.Add(genericType);
            }

            return result;
        }

        /// <summary>
        /// Implement from object to dictionary
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetDictionary<TEntity>(TEntity model)
        {
            var dictionary = new Dictionary<string, object>();
            try
            {
                dictionary = model.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(model, null));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return dictionary;
        }

        /// <summary>
        /// Audit tracker for context who inherit from TrackerDbContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T EnableTracking<T>(this T context) where T : DbContext, ITrackerDbContext
        {
            context.ChangeTracker.Tracked += async (sender, eventArgs) =>
           {
               try
               {
                   if (eventArgs.Entry.State == EntityState.Unchanged) return;
                   var auditResponse = TrackerFactory.Audit(eventArgs.Entry);

                   if (!auditResponse.IsSuccess) return;
                   await context.AddAsync(auditResponse.Result.Item1);
               }
               catch (Exception e)
               {
                   Console.WriteLine(e);
               }
           };

            return context;
        }
    }
}