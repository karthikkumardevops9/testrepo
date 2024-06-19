using Leadtools.ImageProcessing.SpecialEffects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MSRecordsEngine.Services
{
    public static class EntityExtensions
    {
        public static object GetJsonListForGrid<TSource>(this IQueryable<TSource> pEntityList, string pSort, int pPage, int pPageSize, string ShortPropertyName)
        {
            int pageIndex = Convert.ToInt32(pPage) - 1;
            // int pageSize = rows;
            int totalRecords = pEntityList.Count();
            int totalPages = (int)Math.Round(Math.Truncate(Math.Ceiling(totalRecords / (float)pPageSize)));

            if (pSort.ToUpper() == "DESC")
            {
                pEntityList = pEntityList.OrderByField(ShortPropertyName, false);
                pEntityList = pEntityList.Skip(pageIndex * pPageSize).Take(pPageSize);
            }
            else
            {
                pEntityList = pEntityList.OrderByField(ShortPropertyName, true);
                pEntityList = pEntityList.Skip(pageIndex * pPageSize).Take(pPageSize);
            }
            var jsonData = new
            {
                total = totalPages,
                page = pPage,
                records = totalRecords,
                rows = pEntityList
            };

            return jsonData;
        }

        public static object GetJsonListForGrid1<TSource>(this List<TSource> pEntityList, string pSort, int pPage, int pPageSize)
        {
            int pageIndex = Convert.ToInt32(pPage) - 1;
            // int pageSize = rows;
            int totalRecords = pEntityList.Count;
            int totalPages = (int)Math.Round(Math.Truncate(Math.Ceiling(totalRecords / (float)pPageSize)));

            var jsonData = new
            {
                total = totalPages,
                page = pPage,
                records = totalRecords,
                rows = pEntityList
            };

            return jsonData;
        }

        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            var types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }

        public static SelectList CreateSelectListFromList<T>(this List<T> pEntityList, string pIdField, string pNameField, int? pIntSelectValue)
        {
            return pEntityList.CreateSelectListFromList(pIdField, pNameField, pIntSelectValue, pNameField, ListSortDirection.Ascending);
        }

        public static SelectList CreateSelectListFromList<T>(this List<T> pEntityList, string pIdField, string pNameField, int? pIntSelectValue, string pNameFieldSort, ListSortDirection pListSortDirection)
        {

            var result = (from e in pEntityList.AsEnumerable()
                          select new
                          {
                              Id = e.GetType().GetProperty(pIdField, BindingFlags.Instance | BindingFlags.Public).GetValue(e, null),
                              Name = e.GetType().GetProperty(pNameField, BindingFlags.Instance | BindingFlags.Public).GetValue(e, null),
                              Sequence = e.GetType().GetProperty(pNameFieldSort, BindingFlags.Instance | BindingFlags.Public).GetValue(e, null)
                          }).Distinct().ToList();

            if (pListSortDirection == ListSortDirection.Ascending)
            {
                result = result.OrderBy(p => p.Sequence).ToList();
            }
            else
            {
                result = result.OrderByDescending(p => p.Sequence).ToList();
            }

            var oSelectList = !pIntSelectValue.HasValue ? new SelectList(result, "Id", "Name") : new SelectList(result, "Id", "Name", pIntSelectValue.Value);
            return oSelectList;
        }

        public static SelectList CreateSelectList<T>(this IQueryable<T> pEntityList, string pIdField, string pNameField, int? pIntSelectValue)
        {
            return pEntityList.CreateSelectList(pIdField, pNameField, pIntSelectValue, pNameField, ListSortDirection.Ascending);
        }
        public static SelectList CreateSelectList<T>(this IQueryable<T> pEntityList, string pIdField, string pNameField, int? pIntSelectValue, string pNameFieldSort, ListSortDirection pListSortDirection)
        {

            var result = (from e in pEntityList.AsEnumerable()
                          select new
                          {
                              Id = e.GetType().GetProperty(pIdField, BindingFlags.Instance | BindingFlags.Public).GetValue(e, null),
                              Name = e.GetType().GetProperty(pNameField, BindingFlags.Instance | BindingFlags.Public).GetValue(e, null),
                              Sequence = e.GetType().GetProperty(pNameFieldSort, BindingFlags.Instance | BindingFlags.Public).GetValue(e, null)
                          }).Distinct().ToList();



            if (pListSortDirection == ListSortDirection.Ascending)
            {
                result = result.OrderBy(p => p.Sequence).ToList();
            }
            else
            {
                result = result.OrderByDescending(p => p.Sequence).ToList();
            }

            var oSelectList = !pIntSelectValue.HasValue ? new SelectList(result, "Id", "Name") : new SelectList(result, "Id", "Name", pIntSelectValue.Value);
            return oSelectList;
        }

        private static StringBuilder _htmlStringBuilder = new StringBuilder();


        public static string Text(this object pObjValue)
        {
            try
            {
                if (object.ReferenceEquals(pObjValue, DBNull.Value))
                {
                    return "";
                }
                else
                {
                    return pObjValue.ToString();
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static int IntValue(this object pObjValue)
        {
            try
            {
                return Convert.ToInt32(pObjValue);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static double DoubleValue(this object pObjValue)
        {
            try
            {
                return Convert.ToDouble(pObjValue);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    public class CommonControllersService<T>
    {
        public ILogger<T> Logger { get; }
        public IConfiguration Config { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public Microservices Microservices;

        public CommonControllersService(ILogger<T> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor, Microservices microservices)
        {
            Logger = logger;
            Config = config;
            HttpContextAccessor = httpContextAccessor;
            Microservices = microservices;
        }

        public string GetClientIpAddress()
        {
            var context = HttpContextAccessor.HttpContext;
            if (context == null) return "Unable to determine client IP address.";

            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unable to determine client IP address.";
        }
    }
}
