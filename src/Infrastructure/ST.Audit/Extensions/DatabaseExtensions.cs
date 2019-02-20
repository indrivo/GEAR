using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ST.Audit.Attributes;
using ST.Audit.Contexts;
using ST.Audit.Enums;
using ST.Audit.Models;
using ST.BaseRepository;
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
            context.ChangeTracker.Tracked += (sender, eventArgs) =>
           {
               var model = Track(eventArgs);

                //if (model.Item1 == null) return;
                //await context.AddAsync(model.Item1);
                //var job = model.Item2.GetType().GetProperty("Id").GetValue(model.Item2).ToString().ToGuid();
                //var check = context.Find(model.Item2.GetType(), job);
                //if (check != null && eventArgs.Entry.State.Equals(EntityState.Unchanged))
                //{
                //    try
                //    {
                //        context.Update(model.Item2);
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e);
                //    }
                //}
            };

            return context;
        }
        /// <summary>
        /// Compare prev and next value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //private static (bool, EntityState) Compare(EntityEntryEventArgs data)
        //{
        //	var match = false;
        //	var state = EntityState.Unchanged;
        //	try
        //	{
        //		var prev = data.Entry.OriginalValues;
        //		foreach (var prop in prev.Properties)
        //		{
        //			var obj1 = prop.PropertyInfo.GetValue(prev.ToObject());
        //			var obj2 = data.Entry.Entity.GetType().GetProperty(prop.PropertyInfo.Name).GetValue(data.Entry.Entity);
        //			if (obj1 == obj2) continue;
        //			match = true;
        //			state = EntityState.Modified;
        //		}
        //	}
        //	catch (Exception e)
        //	{
        //		Console.WriteLine(e);
        //	}
        //	return (match, state);
        //}

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
                var model = Track(eventArgs);
                if (model.Item1 == null) return;
                await context.AddAsync(model.Item1);
                var check = context.Find(model.Item2.GetType(), ((BaseModel)model.Item2).Id);
                if (check != null && eventArgs.Entry.State.Equals(EntityState.Unchanged))
                {
                    context.Update(model.Item2);
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

            if (eventArgs.Entry.Context.GetType().ToString().Equals("ST.Identity.Data.ApplicationDbContext"))
            {
                if (eventArgs.Entry.State.Equals(EntityState.Unchanged)) return (null, null);
            }

            var bind = IsTrackable(eventArgs.Entry.Entity.GetType());

            if (!bind.Item1) return (null, null);
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
                    TrackEventType = GetRecordState(eventArgs.Entry.State)
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

                var auditDetails = new List<TrackAuditDetails>();

                var propertyUserName = eventArgs.Entry.Entity.GetType().GetProperty("ModifiedBy");
                if (propertyUserName != null)
                {
                    audit.UserName = propertyUserName.GetValue(eventArgs.Entry.Entity)?.ToString();
                }

                if (bind.Item2.Equals(TrackEntityOption.Selected))
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