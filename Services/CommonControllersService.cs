using Leadtools.ImageProcessing.SpecialEffects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MSRecordsEngine.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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

        public string GetConnectionString(Databas DBToOpen, bool includeProvider)
        {
            string sConnect = string.Empty;
            if (includeProvider)
                sConnect = string.Format("Provider={0}; ", DBToOpen.DBProvider);

            sConnect += string.Format("Data Source={0}; Initial Catalog={1}; ", DBToOpen.DBServer, DBToOpen.DBDatabase);

            if (!string.IsNullOrEmpty(DBToOpen.DBUserId))
            {
                sConnect += string.Format("User Id={0}; Password={1};", DBToOpen.DBUserId, DBToOpen.DBPassword);
            }
            else
            {
                sConnect += "Persist Security Info=True;Integrated Security=SSPI;";
                // sConnect = Keys.DefaultConnectionString(True, DBToOpen.DBDatabase)
            }

            return sConnect;
        }

        public string GetConvertCultureDate(string strDate, string cShortDatePattern, string timeOffSetVal, bool bWithTime = false, bool bConvertToLocalTimeZone = true, bool bDetectTime = false)
        {
            DateTime dtPreFormat;
            if (string.IsNullOrWhiteSpace(strDate))
                return strDate;

            try
            {
                dtPreFormat = DateTime.Parse(strDate);
            }
            catch (Exception)
            {
                dtPreFormat = DateTime.ParseExact(strDate, cShortDatePattern + " hh:mm:ss tt", CultureInfo.InvariantCulture);
            }

            return GetConvertCultureDate(dtPreFormat, cShortDatePattern, timeOffSetVal, bWithTime, bConvertToLocalTimeZone, bDetectTime);
        }

        public string InjectWhereIntoSQL(string sSQL, string sNewWhere, string sOperator = "AND")
        {
            string sInitWhere = string.Empty;
            string sInitOrderBy = string.Empty;
            string sInitSelect = string.Empty;
            string sRetVal = sSQL;

            sSQL = NormalizeString(sSQL);
            int iPos = sSQL.IndexOf(" WHERE ", StringComparison.OrdinalIgnoreCase);

            if (iPos > 0)
            {
                sInitSelect = sSQL.Substring(0, iPos).Trim();
                sInitWhere = sSQL.Substring(iPos + " WHERE ".Length).Trim();

                iPos = sInitWhere.IndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);
                if (iPos > 0)
                {
                    sInitOrderBy = sInitWhere.Substring(iPos + " ORDER BY ".Length).Trim();
                    sInitWhere = sInitWhere.Substring(0, iPos).Trim();
                }
            }
            else
            {
                iPos = sSQL.IndexOf(" ORDER BY ", StringComparison.OrdinalIgnoreCase);
                if (iPos > 0)
                {
                    sInitOrderBy = sSQL.Substring(iPos + " ORDER BY ".Length).Trim();
                    sInitSelect = sSQL.Substring(0, iPos).Trim();
                }
                else
                {
                    sInitSelect = sSQL.Trim();
                }
            }

            sRetVal = sInitSelect;

            if (!string.IsNullOrEmpty(sInitWhere))
            {
                if (!string.IsNullOrEmpty(sNewWhere.Trim()))
                {
                    sRetVal += " WHERE (" + ParenEncloseStatement(sInitWhere) + " " + sOperator + " " + ParenEncloseStatement(sNewWhere) + ")";
                }
                else
                {
                    sRetVal += " WHERE " + ParenEncloseStatement(sInitWhere);
                }
            }
            else if (!string.IsNullOrEmpty(sNewWhere.Trim()))
            {
                sRetVal += " WHERE " + ParenEncloseStatement(sNewWhere);
            }

            if (!string.IsNullOrEmpty(sInitOrderBy))
                sRetVal += " ORDER BY " + sInitOrderBy;

            return sRetVal;
        }
        public bool ShowColumn(DataColumn col, int crumblevel, string parentField)
        {
            switch (Convert.ToInt32(col.ExtendedProperties["columnvisible"]))
            {
                case 3:  // Not visible
                    {
                        return false;
                    }
                case 1:  // Visible on level 1 only
                    {
                        if (crumblevel != 0)
                            return false;
                        break;
                    }
                case 2:  // Visible on level 2 and below only
                    {
                        if (crumblevel < 1)
                            return false;
                        break;
                    }
                case 4:  // Smart column- not visible in a drill down when it's the parent.
                    {
                        if (crumblevel > 0 & (parentField.ToLower() ?? "") == (col.ColumnName.ToLower() ?? ""))
                        {
                            return false;
                        }

                        break;
                    }
            }
            if (col.ColumnName.ToLower() == "formattedid")
                return false;
            // If col.ColumnName.ToLower = "id" Then Return False
            if (col.ColumnName.ToLower() == "attachments")
                return false;
            if (col.ColumnName.ToLower() == "slrequestable")
                return false;
            if (col.ColumnName.ToLower() == "itemname")
                return false;
            if (col.ColumnName.ToLower() == "pkey")
                return false;
            if (col.ColumnName.ToLower() == "dispositionstatus")
                return false;
            if (col.ColumnName.ToLower() == "processeddescfieldnameone")
                return false;
            if (col.ColumnName.ToLower() == "processeddescfieldnametwo")
                return false;
            if (col.ColumnName.ToLower() == "rownum")
                return false;
            return true;
        }

        private string NormalizeString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            s = s.Replace("\t", " ");
            s = s.Replace("\r", " ");
            s = s.Replace("\n", " ");

            while (s.Contains("  "))
                s = s.Replace("  ", " ");

            return s;
        }

        private string ParenEncloseStatement(string sSQL)
        {
            if (string.IsNullOrEmpty(sSQL))
                return sSQL;

            bool bInString = false;
            int iParenCount = 0;
            int iMaxParenCount = 0;
            bool bDoEnclose = false;

            for (int iIndex = 0; iIndex < sSQL.Length; iIndex++)
            {
                char sCurChar = sSQL[iIndex];

                if (sCurChar == '"')
                {
                    bInString = !bInString;
                }

                if (!bInString)
                {
                    if (sCurChar == '(')
                    {
                        iParenCount++;
                    }
                    else if (sCurChar == ')')
                    {
                        iParenCount--;
                    }
                }

                if (iParenCount > iMaxParenCount)
                {
                    iMaxParenCount = iParenCount;
                }

                if (iParenCount == 0 && iIndex > 0 && iIndex < sSQL.Length - 1 && iMaxParenCount > 0)
                {
                    bDoEnclose = true;
                    break;
                }
            }

            if (iMaxParenCount == 0)
            {
                bDoEnclose = true;
            }

            if (bDoEnclose)
            {
                return "(" + sSQL + ")";
            }
            else
            {
                return sSQL;
            }
        }

        public string GetConvertCultureDate(DateTime dtPreFormat, string cShortDatePattern, string timeOffSetVal, bool bWithTime = false, bool bConvertToLocalTimeZone = true, bool bDetectTime = false)
        {
            string dtCultureFormat = string.Empty;
            if (dtPreFormat == default)
                return dtCultureFormat;
            if (bDetectTime)
                bWithTime = IncludesTime(dtPreFormat);

            if (bWithTime)
            {
                try
                {
                    if (bConvertToLocalTimeZone)
                        dtPreFormat = ToClientTimeDate(dtPreFormat.ToUniversalTime(), timeOffSetVal);
                    dtCultureFormat = dtPreFormat.ToString(cShortDatePattern + " hh:mm:ss tt", CultureInfo.InvariantCulture);
                }
                // dtCultureFormat = Date.Parse(strDate, New CultureInfo("en-US")).ToString(Keys.GetCultureCookies().DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture)
                catch (Exception)
                {
                    if (bConvertToLocalTimeZone)
                    {
                        dtCultureFormat = ToClientTimeDate(dtPreFormat.ToUniversalTime(), timeOffSetVal).ToShortDateString();
                    }
                    else
                    {
                        dtCultureFormat = dtPreFormat.ToShortDateString();
                    }
                }
            }
            else
            {
                try
                {
                    dtCultureFormat = dtPreFormat.ToString(cShortDatePattern, CultureInfo.InvariantCulture);
                }
                // dtCultureFormat = Date.Parse(strDate, New CultureInfo("en-US")).ToString(Keys.GetCultureCookies().DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture)
                catch (Exception)
                {
                    dtCultureFormat = dtPreFormat.ToShortDateString();
                }
            }

            return dtCultureFormat;
        }

        private bool IncludesTime(DateTime dt)
        {
            var standartTime = new TimeSpan(0, 0, 0, 0, 0);
            int intval = dt.TimeOfDay.CompareTo(standartTime);
            return intval != 0;
        }

        private DateTime ToClientTimeDate(DateTime dt, string timeOffSetVal)
        {
            if (!string.IsNullOrEmpty(timeOffSetVal))
            {
                // read the value from session
                if (timeOffSetVal is not null)
                {
                    int offset = int.Parse(timeOffSetVal.ToString());
                    dt = dt.AddMinutes(-1 * offset);

                    return dt;
                }
            }
            // if there is no offset in session return the datetime in server timezone
            return dt.ToLocalTime();
        }

    }
}
