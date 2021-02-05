using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace LogFilterWeb.Utility
{
    public static class Extensions
    {
        public static IEnumerable<DateTimeOffset> EachDay(DateTimeOffset start, DateTimeOffset end)
        {
            for (var day = start.Date; day.Date <= end.Date; day = day.AddDays(1))
                yield return day;
        }

        public static T ReadCookie<T>(this ControllerBase controller, string key) where T : new()
        {
            var cookieData = controller.HttpContext.Request.Cookies[key];
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                // DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind
            };

            if (cookieData == null)
            {
                return new T();
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(cookieData, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while trying to parse {key} cookie: {ex.Message}", ex);
                controller.HttpContext.Response.Cookies.Delete(key);
                return new T();
            }
        }
    }

    public class CamelCaseParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            // PascalCase -> camelCase
            var source = value as string;
            if (source == null)
            {
                return null;
            }

            if (source.Length == 1)
            {
                return source[0].ToString().ToLower();
            }

            return source[0].ToString().ToLower() + source.Substring(1);
        }
    }

    public static class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, SortOrder sortOrder, IComparer<object> comparer = null)
        {
            switch (sortOrder)
            {
                case SortOrder.Desc:
                    return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);

                case SortOrder.Unknown:
                case SortOrder.Asc:

                default:
                    return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
            }
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, SortOrder sortOrder, IComparer<object> comparer = null)
        {
            switch (sortOrder)
            {
                case SortOrder.Desc:
                    return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);

                case SortOrder.Unknown:
                case SortOrder.Asc:

                default:
                    return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
            }
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "OrderByDescending", propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenBy", propertyName, comparer);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyName, IComparer<object> comparer = null)
        {
            return CallOrderedQueryable(query, "ThenByDescending", propertyName, comparer);
        }

        /// <summary>
        /// Builds the Queryable functions using a TSource property name.
        /// </summary>
        public static IOrderedQueryable<T> CallOrderedQueryable<T>(this IQueryable<T> query, string methodName, string propertyName,
                IComparer<object> comparer = null)
        {
            var param = Expression.Parameter(typeof(T), "x");

            var body = propertyName.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);

            return comparer != null
                ? (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param),
                        Expression.Constant(comparer)
                    )
                )
                : (IOrderedQueryable<T>)query.Provider.CreateQuery(
                    Expression.Call(
                        typeof(Queryable),
                        methodName,
                        new[] { typeof(T), body.Type },
                        query.Expression,
                        Expression.Lambda(body, param)
                    )
                );
        }
    }

}
