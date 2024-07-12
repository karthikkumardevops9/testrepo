using Dapper;
using MSRecordsEngine.Services.Interface;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using MSRecordsEngine.Models;
using MSRecordsEngine.Entities;
using System.Collections.Generic;
using System.Text;
using System;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using MSRecordsEngine.Models.FusionModels;
using System.Globalization;

namespace MSRecordsEngine.Services
{
    public class DashboardService : IDashboardService
    {
        private IDbConnection CreateConnection(string connectionString)
         => new SqlConnection(connectionString);

        private CommonControllersService<DashboardService> _commonService;

        public DashboardService(CommonControllersService<DashboardService> commonService)
        {
            _commonService = commonService;
        }

        public async Task<bool> CheckDashboardNameDuplicate(int DId, string Name, int UId, string ConnectionString)
        {
            using (var con = CreateConnection(ConnectionString))
            {
                var query = "select * from SLUserdashboard Where name = @name and Id <> @id and UserId = @userid";
                var param = new DynamicParameters();
                param.Add("name", Name);
                param.Add("id", DId);
                param.Add("userid", UId);
                var dt = await con.QueryAsync(query, param);
                return dt.Any();
            }
        }

        public async Task<int> InsertDashbaord(string ConnectionString, string Name, int UId, string Json)
        {
            int DashboardId = 0;
            using (var conn = CreateConnection(ConnectionString))
            {
                var queryParam = new DynamicParameters();
                queryParam.Add("Name", Name);
                queryParam.Add("UserId", UId);
                queryParam.Add("Json", Json);

                DashboardId = await conn.QuerySingleAsync<int>(DashboardQueries.DashboardInsertQ, queryParam);
            }

            return DashboardId;
        }

        public async Task<string> GetDashboardListHtml(string ConnectionString, int UserId)
        {
            string dashboardListHtml = string.Empty;

            var htmlDashboard = new StringBuilder();
            var dashboardList = new List<SLUserDashboard>();
            using (var con = CreateConnection(ConnectionString))
            {
                var res = await con.QueryAsync<SLUserDashboard>(DashboardQueries.DashboardGetListQ, new { UserId = UserId });
                if (res != null)
                {
                    dashboardList = res.ToList();
                }
            }
            foreach (var item in dashboardList)
            {
                if (Convert.ToBoolean(item.IsFav))
                {
                    htmlDashboard.Append(string.Format("<li class='hasSubs' dashbard-id='{0}'><a><xmp class='xmpDashbaordNames'>{1}</xmp></a><i class='fa fa-star staricon' aria-hidden='true'></i></li>", item.ID, item.Name));
                }
                else
                {
                    htmlDashboard.Append(string.Format("<li class='hasSubs' dashbard-id='{0}'><a><xmp class='xmpDashbaordNames'>{1}</xmp></a></li>", item.ID, item.Name));
                }
            }

            dashboardListHtml = htmlDashboard.ToString();
            return dashboardListHtml;
        }

        public async Task<List<TableModel>> GetTableNames(object tableIds, string ConnectionString)
        {
            var query = DashboardQueries.TableNameQ.Replace("[TableIds]", Convert.ToString(tableIds));
            var tbList = new List<TableModel>();
            using (var con = CreateConnection(ConnectionString))
            {
                var res = await con.QueryAsync<TableModel>(query);
                tbList = res.ToList();
            }
            return tbList;
        }

        public async Task<SLUserDashboard> GetDashbaordId(int Id, string ConnectionString)
        {
            var ud = new SLUserDashboard();
            using (var conn = CreateConnection(ConnectionString))
            {
                ud = await conn.QueryFirstOrDefaultAsync<SLUserDashboard>(DashboardQueries.DashboardGetIdBaseQ, new { DashboardId = Id });
            }

            return ud;
        }

        public async Task<int> DeleteDashboard(int DashboardId, string ConnectionString)
        {
            int resId = 0;
            using (var con = CreateConnection(ConnectionString))
            {
               resId = await con.QuerySingleAsync<int>(DashboardQueries.DashboardDeleteQ, new { DashboardId = DashboardId });
            }
            return resId;
        }

        public async Task<int> UpdateDashbaordName(string Name, int Id, string ConnectionString)
        {
            int resId = 0;
            using (var con = CreateConnection(ConnectionString))
            {
                var queryParam = new DynamicParameters();
                queryParam.Add("Name", Name);
                queryParam.Add("DashboardId", Id);

                resId = await con.QuerySingleAsync<int>(DashboardQueries.DashboardUpdateNameQ, queryParam);
            }
            return resId;
        }

        public async Task<int> UpdateDashboardJson(string Json, int Id, string ConnectionString)
        {
            int DashboardId = 0;
            using (var con = CreateConnection(ConnectionString))
            {
                var queryParam = new DynamicParameters();
                queryParam.Add("Json", Json);
                queryParam.Add("DashboardId", Id);

                DashboardId = await con.QuerySingleAsync<int>(DashboardQueries.DashboardUpdateJsonQ, queryParam);
            }
            return DashboardId;
        }
         
        public async Task<List<CommonDropdown>> TrackableTable(Passport passport)
        {
            var pTableList = new List<CommonDropdown>();
            using (var con = CreateConnection(passport.ConnectionString))
            {
                var tableList = await con.QueryAsync<Table>(DashboardQueries.TrackingTableQ);
                foreach (var item in tableList)
                {
                    if (passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Smead.Security.Permissions.Permission.View))
                    {
                        var ob = new CommonDropdown();
                        ob.Id = item.TableId;
                        ob.Name = item.TableName;
                        ob.UserName = item.UserName;
                        pTableList.Add(ob);
                    }
                }
            }
            return pTableList;
        }

        public async Task<List<CommonDropdown>> AuditTable(Passport passport)
        {
            var pTableList = new List<CommonDropdown>();
            using (var con = CreateConnection(passport.ConnectionString))
            {
                var tableList = await con.QueryAsync<Table>(DashboardQueries.AuditTableQ);
                foreach (var item in tableList)
                {
                    if (passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Smead.Security.Permissions.Permission.View))
                    {
                        var ob = new CommonDropdown();
                        ob.Id = item.TableId;
                        ob.Name = item.TableName;
                        ob.UserName = item.UserName;
                        pTableList.Add(ob);
                    }
                }
            }
            return pTableList;
        }

        public async Task<List<CommonDropdown>> Users(string connectionString)
        {
            var pTableList = new List<CommonDropdown>();
            using (var con = CreateConnection(connectionString))
            {
                var uList = await con.QueryAsync<CommonDropdown>(DashboardQueries.UserListQ);
                pTableList = uList.ToList();
            }
            return pTableList;
        }

        public async Task<List<ChartModel>> GetBarPieChartData(string tableName, int viewId, string columnName, Passport passport)
        {
            var _query = new Query(passport);
            var @params = new Parameters(viewId, passport);
            @params.fromChartReq = true;
            @params.IsMVCCall = true;
            _query.RefineSQL(@params);
            var chartList = new List<ChartModel>();

            bool isDateTime = await CheckIsDatetimeColumn(Convert.ToString(tableName), Convert.ToString(columnName), passport.ConnectionString);

            if (columnName.ToString().Contains(tableName))
            {
                columnName = "tbl.[" + columnName.ToString().Split(".")[1] + "]";
            }
            else
            {
                columnName = "tbl.[" + columnName + "]";
            }

            if (isDateTime)
            {
                columnName = "Convert(nvarchar (12), " + columnName + ", 111 )";
            }

            var sql = "select " + columnName + " As [X],  count(*) as [Y] from (" + @params.SQL + ") as tbl Group by " + columnName;

            using (var con = CreateConnection(passport.ConnectionString))
            {
                var res = await con.QueryAsync<ChartModel>(sql);
                chartList = res.ToList();
            }
            return chartList;

        }

        public async Task<List<ChartModel>> GetTrackedChartData(object tableIds, object filter, object period, string connectionString)
        {
            var query = string.Empty;
            if (filter.Equals("hour"))
            {
                query = DashboardQueries.TrackedChartQuery.Replace("[Filter]", "Convert(varchar , TransactionDateTime, 1) + ' ' +cast(datepart(hour,TransactionDateTime)as nvarchar) + ':00'").Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }
            else if (filter.Equals("day"))
            {
                query = DashboardQueries.TrackedChartQuery.Replace("[Filter]", "CONVERT(varchar , TransactionDateTime, 1)").Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }
            else if (filter.Equals("week"))
            {
                // Me.Query = Me.TrackedChartQuery.Replace("[Filter]", "DATENAME(week,TransactionDateTime) + 'th week/' + DATENAME(YEAR,TransactionDateTime)").Replace("[TableIds]", tableIds).Replace("[Period]", period)
                query = DashboardQueries.TrackedChartQueryWeek.Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }
            else
            {
                query = DashboardQueries.TrackedChartQuery.Replace("[Filter]", "cast(month(TransactionDateTime) as nvarchar) + '/' +  cast(year(TransactionDateTime) as nvarchar)").Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }

            var list = new List<ChartModel>();

            using (var con = CreateConnection(connectionString))
            {
                var res = await con.QueryAsync<ChartModel>(query);
                list = res.ToList();
            }
            return list;
        }

        public async Task<List<ChartOperatinModelRes>> GetOperationChartData(string tableIds, string usersIds, string AuditTypeIds, string period, string filter, string connectionString)
        {
            string AuditTypeQuery = "";
            string query = string.Empty;

            if (!string.IsNullOrEmpty(Convert.ToString(AuditTypeIds)))
            {
                AuditTypeQuery = " and sl.ActionType in (" + AuditTypeIds + ") ";
            }

            if (filter.Equals("hour"))
            {
                query = DashboardQueries.OperationChartQuery.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
                query = query.Replace("[Filter]", "Convert(varchar , UpdateDateTime, 1) + ' ' +cast(datepart(hour,UpdateDateTime) as nvarchar) + ':00'");
            }
            else if (filter.Equals("day"))
            {
                query = DashboardQueries.OperationChartQuery.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
                query = query.Replace("[Filter]", "CONVERT(varchar , UpdateDateTime, 1)");
            }
            else if (filter.Equals("week"))
            {
                // Me.Query = Me.Query.Replace("[Filter]", "DATENAME(week,UpdateDateTime) + 'th week/' + DATENAME(YEAR,UpdateDateTime)")
                query = DashboardQueries.OperationChartQueryWeek.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
            }
            else
            {
                query = DashboardQueries.OperationChartQuery.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
                query = query.Replace("[Filter]", "cast(month(UpdateDateTime) as nvarchar) + '/' +  cast(year(UpdateDateTime) as nvarchar)");
            }

            var retChart = new List<ChartOperatinModelRes>();
            using (var con = CreateConnection(connectionString))
            {
                var auditTypelst = new Auditing().GetAuditTypeList();
                var result = await con.QueryAsync<ChartOperatinModel>(query);
                var res = result.ToList();
                var xValue = (from i in res
                              select i.X).Distinct().ToList();
                var cAuditType = (from i in res
                                  select i.AuditType).Distinct().ToList();
                var cAuditTypelst = (from c in cAuditType
                                     join au in auditTypelst on c equals au.Value
                                     select new EnumModel() { Value = c, Name = au.Name }).Distinct().ToList();

                foreach (var auditType in cAuditTypelst)
                {
                    var chartOperatinModelRes = new ChartOperatinModelRes
                    {
                        AuditType = auditType.Name
                    };

                    foreach (var x in xValue)
                    {
                        var chartData = new ChartModel
                        {
                            X = x,
                            Y = 0
                        };

                        foreach (var item in res)
                        {
                            if (item.X == x && item.AuditType == auditType.Value)
                            {
                                chartData.Y = item.Y;
                                break; // Exit inner loop early since we found the match
                            }
                        }

                        chartOperatinModelRes.Data.Add(chartData);
                    }

                    retChart.Add(chartOperatinModelRes);
                }

            }
            return retChart;
        }

        public async Task<List<ChartModel>> GetTimeSeriesChartData(string tableName, string columnName, int viewId, string period, string filter, Passport passport)
        {
            var list = new List<ChartModel>();
            string filterSql = "";
            var _query = new Query(passport);
            var @params = new Parameters(viewId, passport);
            @params.fromChartReq = true;
            @params.IsMVCCall = true;
            _query.RefineSQL(@params);

            if (columnName.ToString().Contains(tableName))
            {
                columnName = "[" + columnName.ToString().Split(".")[1] + "]";
            }

            if (filter.Equals("hour"))
            {
                filterSql = "Convert(varchar , " + columnName + ", 1) + ' ' +cast(datepart(hour," + columnName + ") as nvarchar) + ':00'";
            }
            else if (filter.Equals("day"))
            {
                filterSql = "CONVERT(varchar , " + columnName + ", 1)";
            }
            // ElseIf filter.Equals("week") Then
            // filterSql = "DATENAME(week," + columnName + ") + 'th week/' + DATENAME(YEAR," + columnName + ")"
            else
            {
                filterSql = "cast(month(" + columnName + ") as nvarchar) + '/' + cast(year(" + columnName + ") as nvarchar)";
            }

            DashboardQueries.PeriodQuery = DashboardQueries.PeriodQuery.Replace("[Period]", period.ToString());
            string query = "";
            if (filter.Equals("week"))
            {

                query = DashboardQueries.PeriodQuery + @" Declare @temptbl table (X nvarchar(12), Y int, WK int);
                                        Insert Into @temptbl(X,Y,WK)
                                        SELECT CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday," + columnName + ")+1," + columnName + "), 6)  AS [X], COUNT(*) AS [Y],DATEPART(WEEK," + columnName + ") AS WK FROM (" + @params.SQL + @") as tbl 
                                        WHERE " + columnName + @">=@fromdate 
                                        AND " + columnName + "<= @todate GROUP BY CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday," + columnName + ")+1," + columnName + "), 6),DATEPART(WEEK," + columnName + ") order by DATEPART(WEEK," + columnName + @") desc
                                        Declare @WeekFrom nvarchar(12) = CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,@fromdate)+1,@fromdate), 6)
                                        Declare @WeekTo nvarchar(12) = CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,@todate)+1,@todate), 6)
                                                                                     
                                        Select  Case X  when @WeekFrom  then  CONVERT(nvarchar, @fromdate, 6) 
                                        when @WeekTo then CONVERT(nvarchar, @todate, 6) else X end As X, Y
                                        from @temptbl order by WK desc";


            }
            else
            {
                query = DashboardQueries.PeriodQuery + " SELECT " + filterSql + " AS [X], COUNT(*) AS [Y] FROM (" + @params.SQL + @") as tbl 
                                    WHERE " + columnName + @">=@fromdate 
                                    AND " + columnName + "<= @todate GROUP BY " + filterSql + " order by " + filterSql + " asc";
            }

            using (var con = CreateConnection(passport.ConnectionString))
            {
                var res = await con.QueryAsync<ChartModel>(query);
                list = res.ToList();
            }
            return list;
        }

        public async Task<int> GetBarPieChartDataCount(string tableName, string columnName,string connectionString)
        {
            string col = AddSquareBracket(Convert.ToString(columnName));
            var sqlQuery = DashboardQueries.ChartQueryCount.Replace("[FieldName]", col).Replace("[TableName]", Convert.ToString(tableName));
            int res = 0;
            using(var con = CreateConnection(connectionString))
            {
                res = await con.ExecuteScalarAsync<int>(sqlQuery);   
            }
            return res;
        }

        public async Task<int> GetTimeSeriesChartDataCount(string tableName, string columnName, string period, string filter,string connectionString)
        {
            var sqlQuery = DashboardQueries.TimeSeriesChartQueryCount.Replace("[FieldName]", "[" + columnName + "]").Replace("[TableName]", "[" + tableName + "]").Replace("[Period]", period);
            using (var con = CreateConnection(connectionString)) {
                var result = await con.QueryAsync(sqlQuery);
                return result.Count();
            }
        }

        public async Task<int> GetTrackedChartDataCount(string tableIds, string filter, string period,string connectionString)
        {
            var sqlQuery = DashboardQueries.TrackedChartQueryCount.Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            using (var con = CreateConnection(connectionString)) 
            {
                var result = await con.QueryAsync(sqlQuery);
                return result.Count();
            }
        }

        public async Task<int> GetOperationChartDataCount(string tableIds, string usersIds, string AuditTypeIds, string period, string filter,string connectionString)
        {
            string AuditTypeQuery = "";
            int count = 0;
            if (!string.IsNullOrEmpty(Convert.ToString(AuditTypeIds)))
            {
                AuditTypeQuery = " and sl.ActionType in (" + AuditTypeIds + ") ";
            }
            var sqlQuery = DashboardQueries.OperationChartQueryCount.Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[UserIds]", Convert.ToString(usersIds)).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", Convert.ToString(period));
            using (var con = CreateConnection(connectionString))
            {
                var result = await con.QueryAsync(sqlQuery);
                count = result.Count();
            }
            return count;
        }

        public List<CommonDropdown> GetViewColumnOnlyWithType(string tableName, int viewId,string shortDatePattern, Passport passport)
        {
            var DateTypeColumns = new List<CommonDropdown>();

            var list = ViewColumns(viewId,shortDatePattern,passport);
            foreach (CommonDropdown column in list)
            {
                var GridColumnEntity = new CommonDropdown();
                string sFieldType = "";

                // sometime column not found so try catch are handle this exception
                try
                {
                    sFieldType = Navigation.GetFieldDataType(tableName, column.FieldName, passport);
                }
                catch (Exception)
                {

                }

                if (sFieldType.Equals("System.DateTime"))
                {
                    GridColumnEntity.Id = column.Id;
                    GridColumnEntity.Name = column.Name;
                    GridColumnEntity.FieldName = column.FieldName;
                    DateTypeColumns.Add(GridColumnEntity);
                }
            }

            return DateTypeColumns;

        }

        public  List<CommonDropdown> ViewColumns(int ViewId,string ShortDatePattern,Passport passport)
        {
            var lis = new List<CommonDropdown>();

            var _query = new Query(passport);
            var @params = new Parameters(ViewId, passport);
            @params.QueryType = queryTypeEnum.Schema;
            @params.Culture = new CultureInfo("en-US");
            @params.Scope = ScopeEnum.Table;
            @params.Culture.DateTimeFormat.ShortDatePattern = ShortDatePattern;

            _query.FillData(@params);

            foreach (var dc in @params.Data.Columns)
            {

                if (_commonService.ShowColumn((DataColumn)dc, 0, @params.ParentField) == true)
                {
                    var data_col = (DataColumn)dc;
                    // don't show column if the lookuptyp is 1 and it is not a dropdown.
                    if (Convert.ToBoolean(data_col.ExtendedProperties["lookuptype"]) == true && Convert.ToBoolean(data_col.ExtendedProperties["dropdownflag"]) == true)
                    {

                    }
                    // don't show column
                    else
                    {
                        string name = data_col.ExtendedProperties["heading"].ToString();
                        foreach (RecordsManage.ViewColumnsRow viewColumn in @params.ViewColumns.Rows)
                        {
                            if ((viewColumn.Heading ?? "") == (name ?? ""))
                            {
                                var obj = new CommonDropdown();
                                obj.Id = viewColumn.Id;
                                obj.Name = viewColumn.Heading;
                                obj.FieldName = viewColumn.FieldName;
                                lis.Add(obj);
                                break;
                            }
                        }
                    }
                }
            }
            return lis;
        }

        public async Task<List<ChartModel>> GetBarPieChartData(string tableName, object columnName,string connectionString)
        {

            bool isDateTime = await CheckIsDatetimeColumn(Convert.ToString(tableName), Convert.ToString(columnName),connectionString);
            string sqlQuery = "";
            var list = new List<ChartModel>();
            string col = AddSquareBracket(Convert.ToString(columnName));
            if (isDateTime == true)
            {
                // Convert(nvarchar (12), DateHired, 111 )
                sqlQuery = DashboardQueries.ChartQuery.Replace("[FieldName]", " Convert(nvarchar (12), " + columnName + ", 111 ) ").Replace("[TableName]", tableName);
            }
            else
            {
                sqlQuery = sqlQuery.Replace("[FieldName]", col).Replace("[TableName]", tableName);
            }

            using (var con = CreateConnection(connectionString))
            {
                var res = await con.QueryAsync<ChartModel>(sqlQuery);
                list = res.ToList();
            }
            return list;
        }

        public async Task<List<ChartModel>> GetTimeSeriesChartData(string tableName, string columnName, string period, string filter,string connectionString)
        {
            var list = new List<ChartModel>();
            var sqlQuery = DashboardQueries.TimeSeriesChartQuery.Replace("[FieldName]", Convert.ToString(columnName)).Replace("[TableName]", Convert.ToString(tableName)).Replace("[Period]", Convert.ToString(period));

            if (filter.Equals("hour"))
            {
                sqlQuery = sqlQuery.Replace("[Filter]", "Convert(varchar , " + columnName + ", 1) + ' ' +cast(datepart(hour," + columnName + ") as nvarchar) + ':00'");
            }
            else if (filter.Equals("day"))
            {
                sqlQuery = sqlQuery.Replace("[Filter]", "CONVERT(varchar , " + columnName + ", 1)");
            }
            else if (filter.Equals("week"))
            {
                sqlQuery = sqlQuery.Replace("[Filter]", "cast(month(" + columnName + ") as nvarchar) + '/' +  cast(year(" + columnName + ") as nvarchar)");
            }
            else
            {
                sqlQuery = sqlQuery.Replace("[Filter]", "cast(month(" + columnName + ") as nvarchar) + '/' +  cast(year(" + columnName + ") as nvarchar)");
            }

            using(var con = CreateConnection(connectionString))
            {
                var res = await con.QueryAsync<ChartModel>(sqlQuery);
                list = res.ToList();
            }
            return list;
        }

        #region Private Method
        private async Task<bool> CheckIsDatetimeColumn(string tableName, string column, string ConnectionString)
        {
            try
            {
                string query = @"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE 
                               TABLE_NAME = @tablename AND COLUMN_NAME = @column";

                var data = new DataTable();
                using (var conn = CreateConnection(ConnectionString))
                {
                    var queryParam = new DynamicParameters();
                    queryParam.Add("tablename", tableName);
                    queryParam.Add("column", column);
                    var result = await conn.QueryFirstOrDefaultAsync<string>(query, queryParam);

                    return result == "datetime";
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string AddSquareBracket(string column)
        {
            string col;
            string[] strArr;
            strArr = column.Split(".");

            if (strArr.Length > 1)
            {
                col = "[" + strArr[0] + "].[" + strArr[1] + "]";
            }
            else
            {
                col = "[" + strArr[0] + "]";
            }
            return col;
        }

        private async Task<DataTable> GetViewColumns(int viewId,string connectionString)
        {
            var data = new DataTable();
            using (var conn = CreateConnection(connectionString))
            {
                var reader = await conn.ExecuteReaderAsync(DashboardQueries.ViewColumnQ,new { ViewId = viewId});
                if (reader != null)
                    data.Load(reader);
            }
            return data;
        }
        #endregion
    }
}