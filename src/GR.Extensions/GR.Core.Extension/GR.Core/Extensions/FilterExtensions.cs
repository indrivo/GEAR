using GR.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using GR.Core.Helpers.Filters.Enums;
using GR.Core.Helpers.Pagination;

namespace GR.Core.Extensions
{
    public static class FilterExtensions
    {
        /// <summary>
        /// Filter source by regular expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="regexExp"></param>
        /// <returns></returns>
        public static IQueryable<TSource> FilterSourceByRegEx<TSource>(this IQueryable<TSource> source, string regexExp)
        {
            var props = typeof(TSource).GetTypeProprietiesByType(typeof(string)).ToList();
            if (!props.Any()) return source;
            Expression<Func<TSource, bool>> predicate = null;
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var anyMethod = typeof(string).GetMethod("Any", new[] { typeof(string) });
            var match = typeof(Regex).GetMethod(nameof(Regex.Match));
            if (anyMethod == null || match == null) return source;
            foreach (var prop in props)
            {
                var property = Expression.Property(parameter, prop.Name);
                var right = Expression.Call(Expression.Constant(regexExp), match);
                var body = Expression.Call(property, anyMethod, right);
                var predicateExpression = Expression.Lambda<Func<TSource, bool>>(body, parameter);
                predicate = predicate == null
                    ? predicateExpression
                    : Expression.Lambda<Func<TSource, bool>>(Expression.Or(predicate.Body, predicateExpression.Body), parameter);
            }

            return predicate == null ? source : source.Where(predicate);
        }

        /// <summary>
        /// Filter objects by text expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="toSearch"></param>
        /// <returns></returns>
        public static IQueryable<TSource> FilterSourceByTextExpression<TSource>(this IQueryable<TSource> source, string toSearch)
        {
            var predicate = CreateTextFilterExpression<TSource>(toSearch);

            //if (toSearch.IsNumeric())
            //{
            //    var number = Convert.ToDouble(toSearch);
            //    var numberFilterPredicate = CreateNumberFilterExpression<TSource>(number);

            //    if (numberFilterPredicate != null)
            //    {
            //        predicate = predicate == null
            //            ? numberFilterPredicate
            //            : Expression.Lambda<Func<TSource, bool>>(Expression.Or(predicate.Body, numberFilterPredicate.Body), predicate.Parameters[0]);
            //    }
            //}

            return predicate == null ? source : source.Where(predicate);
        }

        /// <summary>
        /// Create expression for filter text
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        private static Expression<Func<TSource, bool>> CreateTextFilterExpression<TSource>(string text)
        {
            var props = typeof(TSource).GetTypeProprietiesByType(typeof(string)).ToList();
            if (!props.Any()) return null;
            Expression<Func<TSource, bool>> predicate = null;
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            if (containsMethod == null || toLowerMethod == null) return null;
            foreach (var prop in props)
            {
                var property = Expression.Property(parameter, prop.Name);
                var left = Expression.Call(property, toLowerMethod);
                var right = Expression.Call(Expression.Constant(text), toLowerMethod);
                var body = Expression.Call(left, containsMethod, right);
                var predicateExpression = Expression.Lambda<Func<TSource, bool>>(body, parameter);
                predicate = predicate == null
                    ? predicateExpression
                    : Expression.Lambda<Func<TSource, bool>>(Expression.Or(predicate.Body, predicateExpression.Body), parameter);
            }

            return predicate;
        }

        /// <summary>
        /// Filter by number proprieties
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="toSearch"></param>
        /// <returns></returns>
        public static IQueryable<TSource> FilterSourceByNumberExpression<TSource>(this IQueryable<TSource> source, double toSearch)
        {
            var predicate = CreateNumberFilterExpression<TSource>(toSearch);
            return predicate == null ? source : source.Where(predicate);
        }

        /// <summary>
        /// Create expression for filter numbers
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        private static Expression<Func<TSource, bool>> CreateNumberFilterExpression<TSource>(double number)
        {
            var props = typeof(TSource).GetNumberProprieties().Where(x => x.GetSetMethod() != null).ToList();
            if (!props.Any()) return null;
            Expression<Func<TSource, bool>> predicate = null;
            var parameter = Expression.Parameter(typeof(TSource), "x");
            foreach (var prop in props)
            {
                var property = Expression.Property(parameter, prop.Name);
                var num = Convert.ChangeType(number, prop.PropertyType);
                var body = Expression.Equal(property, Expression.Constant(num));
                var predicateExpression = Expression.Lambda<Func<TSource, bool>>(body, parameter);
                predicate = predicate == null
                    ? predicateExpression
                    : Expression.Lambda<Func<TSource, bool>>(Expression.Or(predicate.Body, predicateExpression.Body), parameter);
            }

            return predicate;
        }

        /// <summary>
        /// Get number proprieties
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetNumberProprieties(this Type type)
        {
            var types = new List<Type>
            {
                typeof(double), typeof(int), typeof(decimal), typeof(long), typeof(float)
            };
            var response = new List<PropertyInfo>();
            foreach (var t in types)
            {
                var props = type.GetTypeProprietiesByType(t).ToList();
                response.AddRange(props);
            }

            return response;
        }

        /// <summary>
        /// Filter source by multiple filters
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="filtersList"></param>
        /// <returns></returns>
        public static IQueryable<TSource> FilterSourceByFilters<TSource>(this IQueryable<TSource> source, IEnumerable<PageRequestFilter> filtersList)
        {
            var pageRequestFilters = filtersList.ToList();
            if (!pageRequestFilters.Any()) return source;

            foreach (var filter in pageRequestFilters)
            {
                switch (filter.Operator)
                {

                    case Criteria.Equals:
                        source = source.EqualsByProprietyValue(filter.Propriety, filter.Value);
                        break;
                    case Criteria.Contains:
                        source = source.ContainsByProprietyValue(filter.Propriety, filter.Value);
                        break;
                    case Criteria.Greater:
                        source = source.CompareByProprietyValue(filter.Propriety, filter.Value, CompareType.Greater);
                        break;
                    case Criteria.Less:
                        source = source.CompareByProprietyValue(filter.Propriety, filter.Value, CompareType.Less);
                        break;
                    case Criteria.LessOrEqual:
                        source = source.CompareByProprietyValue(filter.Propriety, filter.Value, CompareType.LessOrEqual);
                        break;
                    case Criteria.GreaterOrEqual:
                        source = source.CompareByProprietyValue(filter.Propriety, filter.Value, CompareType.GreaterOrEqual);
                        break;
                    case Criteria.StartWith:
                        source = source.StartWithByProprietyValue(filter.Propriety, filter.Value.ToString());
                        break;
                    case Criteria.EndWith:
                        source = source.EndWithByProprietyValue(filter.Propriety, filter.Value.ToString());
                        break;
                }
            }

            return source;
        }

        /// <summary>
        /// Contains with reflection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> ContainsByProprietyValue<TSource>(this IQueryable<TSource> source, string property, object value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => t.GetStringPropertyValue(property).Contains(value.ToString()));
        }

        /// <summary>
        /// Start with by propriety name
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> StartWithByProprietyValue<TSource>(this IQueryable<TSource> source, string property, string value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => t.GetStringPropertyValue(property).StartsWith(value));
        }

        /// <summary>
        /// Not start with
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> NotStartWithByProprietyValue<TSource>(this IQueryable<TSource> source, string property, string value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => !t.GetStringPropertyValue(property).StartsWith(value));
        }

        /// <summary>
        /// End with by propriety name
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> EndWithByProprietyValue<TSource>(this IQueryable<TSource> source, string property, string value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => t.GetStringPropertyValue(property).EndsWith(value));
        }

        /// <summary>
        /// Not end
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> NotEndWithByProprietyValue<TSource>(this IQueryable<TSource> source, string property, string value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => !t.GetStringPropertyValue(property).EndsWith(value));
        }

        /// <summary>
        /// Equals with reflection
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> EqualsByProprietyValue<TSource>(this IQueryable<TSource> source, string property, object value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => t.GetStringPropertyValue(property) == value.ToString());
        }

        /// <summary>
        /// Not equals by propriety name
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<TSource> NotEqualsByProprietyValue<TSource>(this IQueryable<TSource> source, string property, object value)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;

            return source.Where(t => t.GetStringPropertyValue(property) != value.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static IQueryable<TSource> CompareByProprietyValue<TSource>(this IQueryable<TSource> source, string property, object value, CompareType operation)
        {
            if (property.IsNullOrEmpty() || value == null)
                return source;
            return source.Where(t => CompareObjects(t.GetPropertyValue(property), value, operation));
        }

        #region Helpers

        /// <summary>
        /// Compare 
        /// </summary>
        private static readonly Func<object, object, CompareType, bool> CompareObjects = delegate (object left, object right, CompareType operation)
        {
            if (left == null) return false;
            var output = true;
            switch (left)
            {
                case string str:
                    {
                        var compareResult = string.CompareOrdinal(str, right.ToString());
                        output = CompareObjectsByCompareResult(compareResult, operation);
                    }
                    break;
                case DateTime leftDate:
                    {
                        var valid = DateTime.TryParse(right.ToString(), out var rightDate);
                        output = valid && CompareObjectsByCompareResult(leftDate.CompareTo(rightDate), operation);
                    }
                    break;
                case double doubleNumber:
                    {
                        var valid = double.TryParse(right.ToString(), out var doubleRNumber);
                        output = valid && CompareObjectsByCompareResult(doubleNumber.CompareTo(doubleRNumber), operation);
                    }
                    break;
                case int intNumber:
                    {
                        var valid = int.TryParse(right.ToString(), out var intRNumber);
                        output = valid && CompareObjectsByCompareResult(intNumber.CompareTo(intRNumber), operation);
                    }
                    break;
                case decimal decNumber:
                    {
                        var valid = decimal.TryParse(right.ToString(), out var decRNumber);
                        output = valid && CompareObjectsByCompareResult(decNumber.CompareTo(decRNumber), operation);
                    }
                    break;
                case bool leftBool:
                    {
                        var valid = bool.TryParse(right.ToString(), out var rightBool);
                        output = valid && CompareObjectsByCompareResult(leftBool.CompareTo(rightBool), operation);
                    }
                    break;
            }

            return output;
        };

        /// <summary>
        /// Compare objects by compare result
        /// </summary>
        /// <param name="compareResult"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private static bool CompareObjectsByCompareResult(int compareResult, CompareType operation)
            => operation == CompareType.Less && compareResult < 0
               || operation == CompareType.Greater && compareResult > 0
               || (operation == CompareType.GreaterOrEqual ||
                   operation == CompareType.LessOrEqual) && compareResult == 0;

        #endregion
    }
}
