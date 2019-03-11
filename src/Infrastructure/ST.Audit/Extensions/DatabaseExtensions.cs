﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ST.Audit.Attributes;
using ST.Audit.Contexts;
using ST.Audit.Enums;
using ST.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ST.Audit.Extensions
{
    public static class DatabaseExtensions
    {
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
                var match = IsTrackable(genericType).Item1;
                if (match) result.Add(genericType);
            }

            return result;
        }

        /// <summary>
        /// Audit tracker for context who inherit from TrackerDbContext
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T EnableTracking<T>(this T context) where T : TrackerDbContext
        {
            context.ChangeTracker.Tracked += async (sender, eventArgs) =>
           {
               if (eventArgs.Entry.State == EntityState.Unchanged) return;
               var (audit, entry) = Track(eventArgs);

               if (audit == null) return;
               await context.AddAsync(audit);
               var propId = entry.GetType().GetProperty("Id");
               if (propId != null)
               {
                   var objId = propId.GetValue(entry).ToString().ToGuid();
                   var check = context.Find(entry.GetType(), objId);
                   if (check != null)
                   {
                       try
                       {
                           // context.Attach(entry);
                       }
                       catch (Exception e)
                       {
                           Console.WriteLine(e);
                       }
                   }
               }
           };

            return context;
        }

        /// <summary>
        /// Audit tracker for Identity context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TUser"></typeparam>
        /// <typeparam name="TRole"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T EnableIdentityTracking<T, TUser, TRole, TKey>(this T context)
            where T : TrackerIdentityDbContext<TUser, TRole, TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
        {
            context.ChangeTracker.Tracked += async (sender, eventArgs) =>
            {
                if (eventArgs.Entry.Entity.GetType().Name == "ApplicationRole")
                {
                    var v = 5;
                }
                if (eventArgs.Entry.State == EntityState.Unchanged) return;

                var (audit, entry) = Track(eventArgs);
                if (audit == null) return;
                await context.AddAsync(audit);
                try
                {
                    var objId = ((dynamic)entry).Id;
                    if (objId != null)
                    {
                        var check = context.Find(entry.GetType(), objId);
                        if (check != null)
                        {
                            //context.Attach(entry);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            };
            return context;
        }

        /// <summary>
        /// Check and get track able entity
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        private static (TrackAudit, object) Track(EntityEntryEventArgs eventArgs)
        {
            //If is trackable entity return
            if (eventArgs.Entry.Entity is TrackAudit || eventArgs.Entry.Entity is TrackAuditDetails) return (null, null);

            var (isTrackable, trackOption) = IsTrackable(eventArgs.Entry.Entity.GetType());

            if (!isTrackable) return (null, null);
            try
            {
                dynamic entry = eventArgs.Entry.Entity;
                var audit = new TrackAudit
                {
                    TypeFullName = eventArgs.Entry.Entity.GetType().FullName,
                    Changed = DateTime.Now,
                    Created = DateTime.Now,
                    Author = "System",
                    ModifiedBy = "System",
                    Version = 1,
                    TrackEventType = GetRecordState(eventArgs.Entry.State),
                    DatabaseContextName = eventArgs.Entry.Context.GetType().FullName
                };
                var currentVersion = Convert.ToInt32(eventArgs.Entry.Entity.GetType().GetProperty("Version")?.GetValue(eventArgs.Entry.Entity).ToString());
                var propertyId = eventArgs.Entry.Entity.GetType().GetProperty("Id");
                if (propertyId != null)
                {
                    audit.RecordId = propertyId.GetValue(eventArgs.Entry.Entity).ToString().ToGuid();
                    audit.Version = ++currentVersion;
                    var versionProperty = entry.GetType().GetProperty("Version");
                    if (versionProperty != null)
                    {
                        versionProperty.SetValue(entry, audit.Version);
                    }
                }

                var tenantIdProp = eventArgs.Entry.Entity.GetType().GetProperty("TenantId");
                if (tenantIdProp != null)
                {
                    audit.TenantId = tenantIdProp.GetValue(eventArgs.Entry.Entity)?.ToString()?.ToGuid();
                }

                var auditDetails = new List<TrackAuditDetails>();

                var propertyUserName = eventArgs.Entry.Entity.GetType().GetProperty("ModifiedBy");
                if (propertyUserName != null)
                {
                    audit.UserName = propertyUserName.GetValue(eventArgs.Entry.Entity)?.ToString();
                }

                if (trackOption.Equals(TrackEntityOption.Selected))
                {
                    auditDetails.AddRange(eventArgs.Entry.Entity.GetType().GetProperties().Where(IsFieldTrackable)
                        .Select(x => new TrackAuditDetails
                        {
                            Created = DateTime.Now,
                            Changed = DateTime.Now,
                            ModifiedBy = audit.UserName,
                            PropertyName = x.Name,
                            PropertyType = x.PropertyType.FullName,
                            Value = x.GetValue(eventArgs.Entry.Entity)?.ToString()
                        }));
                }
                else
                {
                    auditDetails.AddRange(eventArgs.Entry.Entity.GetType().GetProperties().Where(IsFieldNotIgnored)
                        .Select(x => new TrackAuditDetails
                        {
                            Created = DateTime.Now,
                            Changed = DateTime.Now,
                            ModifiedBy = audit.UserName,
                            PropertyName = x.Name,
                            PropertyType = x.PropertyType.FullName,
                            Value = x.GetValue(eventArgs.Entry.Entity)?.ToString()
                        }));
                }

                audit.AuditDetailses = auditDetails;
                return (audit, entry);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (null, null);
            }
        }

        /// <summary>
        /// Get record state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static TrackEventType GetRecordState(EntityState state)
        {
            var recState = TrackEventType.Updated;
            switch (state)
            {
                case EntityState.Detached:
                    break;

                case EntityState.Unchanged:
                    break;

                case EntityState.Deleted:
                    recState = TrackEventType.PermanentDeleted;
                    break;

                case EntityState.Modified:
                    recState = TrackEventType.Updated;
                    break;

                case EntityState.Added:
                    recState = TrackEventType.Added;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            return recState;
        }

        /// <summary>
        /// Is entity track able
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static (bool, TrackEntityOption) IsTrackable(MemberInfo type)
        {
            var attrs = Attribute.GetCustomAttributes(type);
            foreach (var attr in attrs)
            {
                if (!(attr is TrackEntity bind)) continue;
                if (!bind.Option.Equals(TrackEntityOption.Ignore))
                {
                    return (true, bind.Option);
                }

                break;
            }

            return (default(bool), default(TrackEntityOption));
        }

        /// <summary>
        /// Is property track able
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsFieldTrackable(PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes(true);
            foreach (var attr in attrs)
            {
                if (!(attr is TrackField bind)) continue;
                if (bind.Option.Equals(TrackFieldOption.Allow))
                {
                    return true;
                }

                break;
            }

            return false;
        }

        /// <summary>
        /// Check if field is ignored
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static bool IsFieldNotIgnored(PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes(true);
            foreach (var attr in attrs)
            {
                if (!(attr is TrackField bind)) continue;
                if (bind.Option.Equals(TrackFieldOption.Ignore))
                {
                    return false;
                }

                break;
            }

            return true;
        }

        /// <summary>
        /// Parse string to Guid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string id)
        {
            return Guid.Parse(id);
        }
    }
}