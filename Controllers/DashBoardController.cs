using System;
using MSRecordsEngine.Services;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using MSRecordsEngine.Entities;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Models;
using MSRecordsEngine.RecordsManager;
using Smead.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Entity;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private IDbConnection CreateConnection(string connectionString)
         => new SqlConnection(connectionString);
        private readonly CommonControllersService<DashBoardController> _commonService;
        private string ChartQuery = "select [FieldName] As [X],  count(*) as [Y] from [TableName] Group by [FieldName]";
        private string ChartQueryCount = "select count(*) as [Y] from [TableName]";
        // Private PeriodQuery = "DECLARE @currentDate DATE,@fromdate DATE, @todate DATE, @period INT
        // SELECT @currentDate = GETDATE(), @period = [Period]
        // IF(@period = 1) SELECT @fromdate = CONVERT(varchar, DATEADD(DAY,-14, @currentDate), 1), @todate = CONVERT(varchar, DATEADD(DAY,-7, @currentDate), 1)
        // ELSE IF(@period = 2) SELECT @fromdate = CONVERT(varchar, DATEADD(MONTH,-2, @currentDate), 1),  @todate = CONVERT(varchar, DATEADD(MONTH,-1, @currentDate), 1)
        // ELSE SELECT @fromdate = CONVERT(varchar, DATEADD(MONTH, -6, @currentDate), 1), @todate = CONVERT(varchar, DATEADD(MONTH, -3, @currentDate), 1)                 "

        private string PeriodQuery = @"DECLARE @currentDate date,@fromdate date,@todate date,@period int
                           SELECT @currentDate = GETDATE(),@period = [Period]
                           IF (@period = 1) SELECT @fromdate = CONVERT(varchar, DATEADD(DAY, -6, @currentDate), 1),@todate = @currentDate
                           ELSE IF (@period = 2) Begin SET DATEFIRST 1 SELECT @fromdate = CONVERT(varchar, DATEADD(DAY, -29, @currentDate), 1), @todate = @currentDate End
                           ELSE SELECT @fromdate = CONVERT(varchar, DATEADD(DAY, -89, @currentDate), 1), @todate = @currentDate";


        private string TimeSeriesChartQuery;

        private string TimeSeriesChartQueryCount;

        private string OperationChartQuery;

        private string OperationChartQueryWeek;

        private string OperationChartQueryCount;

        private string TrackedChartQuery;

        private string TrackedChartQueryWeek;


        private string TrackedChartQueryCount;

        private string GridQuery = "select [Fields] from [TableName]";

        private string DashboardInsertQ = @"Declare @InsertedId table(Id int)
                                          Insert Into SLUserDashboard (Name, UserID, Json) output Inserted.Id into @InsertedId  values(@Name, @UserID, @Json)  
                                          select Id from @InsertedId";

        private string DashboardUpdateJsonQ = @"Declare @UpdatedId table(Id int)
                                              update SLUserDashboard set Json = @Json output Inserted.Id into @UpdatedId where Id = @DashboardId
                                              select Id from @UpdatedId";

        private string DashboardUpdateNameQ = @"Declare @UpdatedId table(Id int)
                                              update SLUserDashboard set Name = @Name output Inserted.Id into @UpdatedId where Id = @DashboardId
                                              select Id from @UpdatedId";

        private string DashboardGetIdBaseQ = "select ID, Name, Json, IsFav from SLUserDashboard where ID = @DashboardId";

        private string DashboardGetListQ = "select ID, Name, Json, isnull(IsFav, 0) as IsFav from SLUserDashboard where UserID=@UserId order by isnull(IsFav, 0) desc, Name";

        private string DashboardDeleteQ = @"Declare @InsertedId table(Id int)
                                          Delete SLUserDashboard output Deleted.Id into @InsertedId  where ID = @DashboardId  
                                          select Id from @InsertedId";

        private string TrackingTableQ = @"select t.TableId, t.TableName,  t.UserName from tables as t
                                        inner join vwTablesAll as v
                                        on t.TableName = v.TABLE_NAME COLLATE Latin1_General_CI_AS
                                        where t.trackable = 1";

        private string AuditTableQ = @"select t.TableId, t.TableName,  t.UserName from tables as t
                                        inner join vwTablesAll as v
                                        on t.TableName = v.TABLE_NAME COLLATE Latin1_General_CI_AS
                                        where t.AuditUpdate = 1";

        private string ViewColumnQ = @"select Id, Heading as Name, FieldName, ViewsId, ColumnNum from ViewColumns where viewsid = @ViewId
                                     order by ColumnNum";

        private string UserListQ = "select UserId As Id, UserName As SID, FullName as Name from SecureUser where UserId > 0";

        private string TableNameQ = "select TableName from tables where tableId in ([TableIds])";

        public DashBoardController(CommonControllersService<DashBoardController> commonService)
        {
            _commonService = commonService;
            TimeSeriesChartQuery = PeriodQuery + @" SELECT [Filter] AS [X], COUNT(*) AS [Y] FROM [TableName] WHERE 
                                                            [FieldName] >= @fromdate AND [FieldName] <= @todate GROUP BY [Filter]";
            TimeSeriesChartQueryCount = PeriodQuery + @" SELECT COUNT(*) AS [Y] FROM [TableName] WHERE 
                                                                [FieldName] >= @fromdate AND [FieldName] <= @todate ";
            OperationChartQuery = PeriodQuery + @" SELECT  [Filter] AS [X], COUNT(*)  AS [Y], sl.actiontype AS AuditType FROM SLAuditUpdates  AS sl INNER JOIN Tables AS t
                                            ON sl.TableName = t.TableName WHERE t.TableId IN ([TableIds]) AND sl.OperatorsId IN ([UserIds]) [AuditType]
                                            AND (CONVERT(varchar , UpdateDateTime, 1) >= @fromdate AND CONVERT(varchar , UpdateDateTime, 1) <= @todate) 
                                            GROUP BY [Filter] , sl.ActionType order by [Filter] desc";
            OperationChartQueryWeek = PeriodQuery + @" Declare @temptbl table (X nvarchar(12), Y int, WK int, AuditType int);
                                                Insert Into @temptbl(X,Y,WK, AuditType)
                                                SELECT
                                                  CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,UpdateDateTime)+1,UpdateDateTime), 6)  AS X,
                                                  COUNT(*) AS [Y],
                                                  DATEPART(WEEK,UpdateDateTime) AS WK,
                                                  sl.actiontype AS AuditType
                                                FROM
                                                  SLAuditUpdates AS sl
                                                  INNER JOIN Tables AS t ON sl.TableName = t.TableName
                                                WHERE
                                                  t.TableId IN ([TableIds])
                                                  AND sl.OperatorsId IN ([UserIds])
                                                  [AuditType] 
                                                  AND (
                                                    CONVERT(varchar, UpdateDateTime, 1) >= @fromdate
                                                    AND CONVERT(varchar, UpdateDateTime, 1) <= @todate
                                                  )
                                                GROUP BY
                                                  CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,UpdateDateTime)+1,UpdateDateTime), 6),
                                                  DATEPART(WEEK,UpdateDateTime),
                                                  sl.actiontype
                                                order by
                                                  DATEPART(WEEK,UpdateDateTime) desc
                                                
                                                
                                                Declare @WeekFrom nvarchar(12) = CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,@fromdate)+1,@fromdate), 6)
                                                Declare @WeekTo nvarchar(12) = CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,@todate)+1,@todate), 6)
                                                                                             
                                                Select  Case X  when @WeekFrom  then  CONVERT(nvarchar, @fromdate, 6) 
                                                when @WeekTo then CONVERT(nvarchar, @todate, 6) else X end As X, Y, AuditType
                                                from @temptbl order by WK desc";

            OperationChartQueryCount = PeriodQuery + @" SELECT  COUNT(*)  AS [Y] FROM SLAuditUpdates  AS sl INNER JOIN Tables AS t ON sl.TableName = t.TableName WHERE t.TableId IN ([TableIds]) AND sl.OperatorsId IN ([UserIds]) [AuditType]
                                            AND (CONVERT(varchar , UpdateDateTime, 1) >= @fromdate AND CONVERT(varchar , UpdateDateTime, 1) <= @todate) ";

            TrackedChartQuery = PeriodQuery + @" SELECT [Filter] AS X, COUNT(*) AS Y FROM TrackingHistory  AS th
                                           INNER JOIN tables t ON t.tableName = th.TrackedTable WHERE t.tableId IN ([TableIds]) AND 
                                           (CONVERT(varchar , TransactionDateTime, 1) >= @fromdate AND CONVERT(varchar , TransactionDateTime, 1) <= @todate)
                                           GROUP BY [Filter] order by [Filter] desc";
            TrackedChartQueryWeek = PeriodQuery + @" Declare @temptbl table (X nvarchar(12), Y int, WK int);
                                             Insert Into @temptbl(X,Y,WK)
                                             Select
                                               CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,TransactionDateTime)+1,TransactionDateTime), 6)  AS X,
                                               COUNT(*) AS Y, DATEPART(WEEK,TransactionDateTime) AS WK
                                             From TrackingHistory AS th INNER JOIN tables t ON t.tableName = th.TrackedTable
                                             Where t.tableId IN ([TableIds])
                                               AND ( CONVERT(varchar, TransactionDateTime, 1) >= @fromdate AND CONVERT(varchar, TransactionDateTime, 1) <= @todate )
                                             Group By CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,TransactionDateTime)+1,TransactionDateTime), 6), DATEPART(WEEK,TransactionDateTime)
                                             order by DATEPART(WEEK,TransactionDateTime) desc
                                             
                                             Declare @WeekFrom nvarchar(12) = CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,@fromdate)+1,@fromdate), 6)
                                             Declare @WeekTo nvarchar(12) = CONVERT(nvarchar, DATEAdd(day, -DATEPART(weekday,@todate)+1,@todate), 6)
                                             
                                             Select  Case X  when @WeekFrom  then  CONVERT(nvarchar, @fromdate, 6) 
                                             		when @WeekTo then CONVERT(nvarchar, @todate, 6) else X end As X, Y
                                             from @temptbl order by WK desc";
            TrackedChartQueryCount = PeriodQuery + " SELECT  COUNT(*) AS Y FROM TrackingHistory  AS th   INNER JOIN tables t ON t.tableName = th.TrackedTable WHERE t.tableId IN ([TableIds]) AND (CONVERT(varchar , TransactionDateTime, 1) >= @fromdate AND CONVERT(varchar , TransactionDateTime, 1) <= @todate) ";
        }

        [Route("GetDashboardList")]
        [HttpPost]
        public async Task<DashBoardReturn> GetDashboardList(DashBoardParam param)
        {
            DashBoardReturn result = new DashBoardReturn();
            try
            {
                result.DashboardListHtml = await GetDashboardListHtml(param.ConnectionString, param.UserId);
            }
            catch (Exception ex)
            {
                result.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("GetWorkGroupTableMenu")]
        [HttpPost]
        public ReturnWorkGroupTableMenu GetWorkGroupTableMenu(GetWorkGroupTableMenuParam param)
        {
            ReturnWorkGroupTableMenu res = new ReturnWorkGroupTableMenu();
            var itemList = new List<TableItem>();
            try
            {
                itemList = Navigation.GetWorkGroupMenu(param.WorkGroupId, param.Passport).OrderBy(x => x.UserName).ToList();
                res.WorkGroupMenuString = JsonConvert.SerializeObject(itemList);
            }
            catch (Exception ex)
            {
                res.isError = true;
                res.Msg = "Oops an error occurred.  Please contact your administrator.";
                res.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.Passport.DatabaseName} CompanyName: {param.Passport.License.CompanyName}");
            }
            return res;
        }

        [Route("SetDashboardDetails")]
        [HttpPost]
        public async Task<SetDashboardDetailsResonse> SetDashboardDetails(SetDashboardDetailsParam param)
        {
            SetDashboardDetailsResonse result = new SetDashboardDetailsResonse();
            try
            {
                var checkExist = await CheckDashboardNameDuplicate(0, param.Name, param.UserId, param.ConnectionString);
                if (checkExist)
                {
                    result.ErrorMessage = "Already exists name.";
                }
                else
                {
                    var resId = await InsertDashbaord(param.ConnectionString, param.Name, param.UserId, "");
                    if (resId > 0)
                    {
                        result.ErrorMessage = "Added successfully";
                        result.ud = await GetDashbaordId(resId, param.ConnectionString);
                        result.DashboardListHtml = await GetDashboardListHtml(param.ConnectionString, param.UserId);
                    }
                    else
                    {
                        result.isError = true;
                        result.Msg = "Fail to add new dashboard";
                    }
                }
            }
            catch (Exception ex)
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("AddEditChartPartial")]
        [HttpPost]
        public string AddEditChartPartial(Passport passport)
        {
            var dropM = new List<WorkGroupItem>();
            try
            {
                dropM = Navigation.GetWorkGroups(passport).OrderBy(x => x.WorkGroupName).ToList();
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return JsonConvert.SerializeObject(dropM);
        }

        [Route("GetViewMenu")]
        [HttpPost]
        public GetViewMenuReturns GetViewMenu(GetViewMenuParams param)
        {
            GetViewMenuReturns result = new GetViewMenuReturns();
            var viewByList = new List<ViewItem>();
            try
            {
              viewByList = Navigation.GetViewsByTableName(param.TableName, param.passport).OrderBy(x => x.ViewName).ToList();
               result.ViewsTbNameString = JsonConvert.SerializeObject(viewByList);
            }
            catch (Exception ex)
            {
                result.isError = true;
                result.Msg = "";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return result;
        }

        [Route("GetDashboardDetails")]
        [HttpPost]
        public async Task<GetDashboardDetailsReturn> GetDashboardDetails(GetDashboardDetailParam param)
        {
            GetDashboardDetailsReturn result = new GetDashboardDetailsReturn();
            try
            {
                result.ud = await GetDashbaordId(param.DashboardId,param.ConnectionString);
            }
            catch (Exception ex) 
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("GetViewColumnMenu")]
        [HttpPost]
        public string GetViewColumnMenu(GetViewColumnMenuParam param) 
        {
            var lis = new List<CommonDropdown>();
            //var cList = new List<CommonDropdown>();
            try
            {
                var _query = new Query(param.Passport);
                var @params = new Parameters(param.ViewId, param.Passport);
                @params.QueryType = queryTypeEnum.Schema;
                @params.Culture = param.culture;
                @params.Scope = ScopeEnum.Table;
                @params.Culture.DateTimeFormat.ShortDatePattern = param.ShortDatePattern;
                // Me.dateFormat = Keys.GetUserPreferences().sPreferedDateFormat.ToString().Trim().ToUpper
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
                                    obj.Name = viewColumn.Heading; // dc.ExtendedProperties("heading").ToString
                                    obj.FieldName = viewColumn.FieldName;
                                    lis.Add(obj);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.Passport.DatabaseName} CompanyName: {param.Passport.License.CompanyName}");
            }

            return JsonConvert.SerializeObject(lis);
        }

        [Route("SetDashboardJson")]
        [HttpPost]
        public async Task<SetDashboardJsonReturn> SetDashboardJson(SetDashboardJsonParam param)
        {
            SetDashboardJsonReturn result = new SetDashboardJsonReturn();
            try
            {
                using(var con = CreateConnection(param.ConnectionString))
                {
                    var queryParam = new DynamicParameters();
                    queryParam.Add("Json", param.Json);
                    queryParam.Add("DashboardId", param.DashboardId);

                    var resId = await con.QuerySingleAsync<int>(DashboardUpdateJsonQ,queryParam);
                    if(resId > 0)
                    {
                        result.Msg = "Update successfully";
                        result.ud = await GetDashbaordId(resId, param.ConnectionString);
                    }
                    else
                    {
                        result.isError = true;
                        result.Msg = "Fail to update";
                    }
                }
            }
            catch (Exception ex) 
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("AddEditTrackedPartial")]
        [HttpPost]
        public async Task<string> AddEditTrackedPartial(Passport passport)
        {
            var pTableList = new List<CommonDropdown>();
            try
            {
                using(var con = CreateConnection(passport.ConnectionString))
                {
                    var tableList = await con.QueryAsync<Table>(TrackingTableQ);
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
            }
            catch (Exception ex) {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return JsonConvert.SerializeObject(pTableList);
        }

        [Route("AddEditOperationPartial")]
        [HttpPost]
        public async Task<AddEditOperationReturn> AddEditOperationPartial(Passport passport)
        {
            var result = new AddEditOperationReturn();
            try
            {
                var pTablesList = new List<CommonDropdown>();
                using(var con = CreateConnection(passport.ConnectionString))
                {
                    var uList = await con.QueryAsync<CommonDropdown>(UserListQ);
                    uList = uList.OrderBy(x=>x.Name).ToList();
                    result.Users = JsonConvert.SerializeObject(uList);

                    var tableList = await con.QueryAsync<Table>(AuditTableQ);
                    foreach (var item in tableList)
                    {
                        if (passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Smead.Security.Permissions.Permission.View))
                        {
                            var ob = new CommonDropdown();
                            ob.Id = item.TableId;
                            ob.Name = item.TableName;
                            ob.UserName = item.UserName;
                            pTablesList.Add(ob);
                        }
                    }
                    pTablesList = pTablesList.OrderBy(x=>x.Name).ToList();
                    result.AuditTable = JsonConvert.SerializeObject(pTablesList);
                }
                var auditTypeList = new Auditing().GetAuditTypeList();
                auditTypeList.Sort((x, y) => x.Name.ToLower().CompareTo(y.Name.ToLower()));
                result.AuditTypeList = auditTypeList;
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return result;
        }

        [Route("RenameDashboardName")]
        [HttpPost]
        public async Task<RenameDashboardNameReturn> RenameDashboardName(RenameDashboardNameParam param)
        {
            RenameDashboardNameReturn result = new RenameDashboardNameReturn();
            try
            {
                using(var con = CreateConnection(param.ConnectionString))
                {
                    if(await CheckDashboardNameDuplicate(param.DashboardId, param.Name, param.UserId, param.ConnectionString))
                    {
                        result.ErrorMessage = "Already exists name.";
                    }
                    else
                    {
                        var queryParam = new DynamicParameters();
                        queryParam.Add("Name",param.Name);
                        queryParam.Add("DashboardId", param.DashboardId);

                        var resId = await con.QuerySingleAsync<int>(DashboardUpdateNameQ, queryParam);
                        
                        if(resId > 0)
                        {
                            result.ErrorMessage = "Update successfully";
                            result.ud = await GetDashbaordId(resId, param.ConnectionString);
                            result.DashboardListHtml = await GetDashboardListHtml(param.ConnectionString,param.UserId);
                        }
                        else
                        {
                            result.isError = true;
                            result.Msg = "Fail to add new dashboard";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("DeleteDashboard")]
        [HttpPost]
        public async Task<DeleteDashboardReturn> DeleteDashboard(DeleteDashboardParam param)
        {
            DeleteDashboardReturn result = new DeleteDashboardReturn();
            try
            {
                using(var con = CreateConnection(param.ConnectionString))
                {
                    var resId = await con.QuerySingleAsync<int>(DashboardDeleteQ,new { DashboardId = param.DashboardId });
                    if(resId > 0)
                    {
                        result.Msg = "Delete Successfully";
                    }
                    else
                    {
                        result.isError = true;
                        result.Msg = "Fail to delete dashboard";
                    }
                }
            }
            catch (Exception ex) 
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("ValidatePermission")]
        [HttpPost]
        public async Task<ValidPermissionReturn> ValidatePermission(ValidPermissionParam param)
        {
            var model = new ValidPermissionReturn();
            model.JsonString = param.WidgetList;
            var updatedWidgetList = new List<object>();
            try
            {
                var parseJsonList = JsonConvert.DeserializeObject<List<object>>(param.WidgetList);

                foreach (JObject parseJson in parseJsonList)
                {
                    var asdf = parseJson[""];
                    if (parseJson["WidgetType"].ToString() == "CHART_1" || parseJson["WidgetType"].ToString() == "CHART_2")
                    {
                        string TableIds = ConvertArrayToString((JArray)(parseJson["Objects"]));
                        var TableNames = await GetTableNames(TableIds,param.Passport.ConnectionString);
                        bool Permission = true;
                        foreach (TableModel item in TableNames)
                        {
                            if (param.Passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View) == false)
                            {
                                Permission = false;
                                break;
                            }
                        }
                        parseJson["permission"] = Permission;
                    }
                    else
                    {
                        // check table permission 
                        string TableName = parseJson["TableName"].ToString();
                        int ParentView = Convert.ToInt32(parseJson["ParentView"].ToString());
                        int WorkGroupId = Convert.ToInt32(parseJson["WorkGroup"].ToString());
                        bool permission = true;
                        var ViewName = "";
                        
                        using(var context = new TABFusionRMSContext(param.Passport.ConnectionString))
                        {
                            ViewName = await context.Views.Where(x => x.Id == ParentView).Select(x => x.ViewName).FirstOrDefaultAsync();
                        }

                        // 'It is already getting all permission workgroup list so do not need to check permission
                        if (Navigation.GetWorkGroups(param.Passport).Select(x => x.ID == WorkGroupId).ToList().Count() == 0)
                            permission = false;

                        if (permission == true)
                        {
                            if (string.IsNullOrEmpty(TableName) == false)
                            {
                                permission = param.Passport.CheckPermission(parseJson["TableName"].ToString(), Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View);
                                if (permission == true)
                                {
                                    if (string.IsNullOrEmpty(ViewName) == false)
                                    {
                                        permission = param.Passport.CheckPermission(ViewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View);
                                    }
                                }
                            }
                            else if (string.IsNullOrEmpty(ViewName) == false)
                            {
                                permission = param.Passport.CheckPermission(ViewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View);
                            }
                        }
                        parseJson["permission"] = permission;
                    }
                    updatedWidgetList.Add(parseJson);
                }
                model.JsonString = JsonConvert.SerializeObject(updatedWidgetList);
            }
            catch (Exception ex)
            {
                
            }
            return model;
        }

        [Route("GetChartData")]
        [HttpPost]
        public async Task<ChartDataResModel> GetChartData(widgetDataParam param)
        {
            var result = new ChartDataResModel();
            result.JsonString = param.widgetObjectJson;
            result.Permission = true;
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                var ParentView = Convert.ToInt16(parseJson["ParentView"].ToString());
                int workgroupId = Convert.ToInt32(parseJson["WorkGroup"].ToString());

                var workgroup = Navigation.GetWorkGroups(param.passport).OrderBy(x => x.WorkGroupName).ToList();
                var workgroupname = workgroup.FirstOrDefault(x => x.ID == workgroupId).WorkGroupName;

                if(workgroupname == null)
                {
                    result.Permission = false;
                    return result;
                }

                if (!param.passport.CheckPermission(workgroupname, Smead.Security.SecureObject.SecureObjectType.WorkGroup, Permissions.Permission.Access))
                {
                    result.Permission = false;
                    return result;
                }
                if (!param.passport.CheckPermission(parseJson["TableName"].ToString(), Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                {
                    result.Permission = false;
                    return result;
                }

                string TableName = parseJson["TableName"].ToString();
                var ColumnName = parseJson["Column"].ToString();

                using (var context = new TABFusionRMSContext(param.passport.ConnectionString))
                {
                    var viewName = await context.Views.Where(x => x.Id == ParentView).Select(x => x.ViewName).FirstOrDefaultAsync();
                    if (string.IsNullOrEmpty(viewName) == false)
                    {
                        var ress = param.passport.CheckPermission(viewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View);
                        if (ress == false)
                        {
                            result.Permission = false;
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }

                    var col = await context.ViewColumns.Where(x => x.FieldName == ColumnName && x.ViewsId == ParentView).ToListAsync();
                    if(col.Count == 0)
                    {
                        result.Permission = false;
                        return result;
                    }
                }

                var chartList = await GetBarPieChartData(TableName, Convert.ToInt32(ParentView), ColumnName,param.passport);
                result.DataString = JsonConvert.SerializeObject(chartList.ToList());

                return result;
            }
            catch(Exception ex)
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return result;
        }

        [Route("GetTrackedData")]
        [HttpPost]
        public async Task<ChartDataResModel> GetTrackedData(widgetDataParam param)
        {
            var result = new ChartDataResModel();
            result.JsonString = param.widgetObjectJson;
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                string TableIds = ConvertArrayToString((JArray)parseJson["Objects"]);
                var TableNames = await GetTableNames(TableIds,param.passport.ConnectionString);

                foreach (TableModel item in TableNames)
                {
                    if (param.passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View) == false)
                    {
                        result.Permission = false;
                        return result;
                    }
                }

                var Filter = parseJson["Filter"].ToString();
                var pe = parseJson["Period"];
                var list = await GetTrackedChartData(TableIds, Filter, pe, param.passport.ConnectionString);
                result.DataString = JsonConvert.SerializeObject(list);

                return result;
            }
            catch (Exception ex)
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return result;
        }

        [Route("GetOperationsData")]
        [HttpPost]
        public async Task<OperationChartDataResModel> GetOperationsData(widgetDataParam param)
        {
            var model = new OperationChartDataResModel();
            model.JsonString = param.widgetObjectJson;
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                string TableIds = ConvertArrayToString((JArray)parseJson["Objects"]);
                var TableNames = await GetTableNames(TableIds,param.passport.ConnectionString);
                model.Permission = true;
                foreach (TableModel item in TableNames)
                {
                    if (param.passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View) == false)
                    {
                        model.Permission = false;
                        break;
                    }
                }
                if (model.Permission == false)
                {
                    return model;
                }
                string UserIds = ConvertArrayToString((JArray)parseJson["Users"]);
                string AuditTypeId = ConvertArrayToString((JArray)parseJson["AuditType"]);
                var Period = parseJson["Period"].ToString();
                var Filter = parseJson["Filter"].ToString();

                var list = await GetOperationChartData(TableIds, UserIds, AuditTypeId, Period, Filter,param.passport.ConnectionString);
                model.DataString = JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        [Route("GetTimeSeriesChartData")]
        [HttpPost]
        public async Task<ChartDataResModel> GetTimeSeriesChartData(widgetDataParam param)
        {
            var model = new ChartDataResModel();
            model.JsonString = param.widgetObjectJson;
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                var ParentView = Convert.ToInt16(parseJson["ParentView"].ToString());
                var ViewName = string.Empty;
                using(var context = new TABFusionRMSContext(param.passport.ConnectionString))
                {
                    ViewName =  await context.Views.Where(x => x.Id == ParentView).Select(x => x.ViewName).FirstOrDefaultAsync();
                }

                int workgroupId = Convert.ToInt32(parseJson["WorkGroup"].ToString());
                var ColumnName = parseJson["Column"].ToString();
                var workgroup = Navigation.GetWorkGroups(param.passport).OrderBy(x => x.WorkGroupName);
                var workgroupname = workgroup.ToList().FirstOrDefault(x => x.ID == workgroupId).WorkGroupName;

                if (string.IsNullOrEmpty(workgroupname) || !param.passport.CheckPermission(workgroupname, Smead.Security.SecureObject.SecureObjectType.WorkGroup, Permissions.Permission.Access))
                {
                    model.Permission = false;
                    return model;
                }
                // check table permission 
                if (!param.passport.CheckPermission(parseJson["TableName"].ToString(), Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                {
                    model.Permission = false;
                    return model;
                }

                // check view permission
                if (ViewName.Length > 0)
                {
                    model.Permission = param.passport.CheckPermission(ViewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View);
                    if (model.Permission == false)
                    {
                        return model;
                    }
                }
                else
                {
                    return model;
                }

                using (var context = new TABFusionRMSContext(param.passport.ConnectionString))
                {
                    var col = await context.ViewColumns.Where(x => x.FieldName == ColumnName && x.ViewsId == ParentView).ToListAsync();
                    if(col.Count == 0)
                    {
                        model.Permission = false;
                        return model;
                    }
                }
                var list = await GetTimeSeriesChartData(parseJson["TableName"].ToString(), ColumnName, Convert.ToInt32(ParentView), parseJson["Period"].ToString(), parseJson["Filter"].ToString(),param.passport);
                model.DataString = JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        [Route("GetWorkGroupListSession")]
        [HttpPost]
        public string GetWorkGroupListSession(Passport passport)
        {
            var list = new List<WorkGroupItem>();
            try
            {
                list = Navigation.GetWorkGroups(passport).OrderBy(x => x.WorkGroupName).ToList();
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {passport.DatabaseName} CompanyName: {passport.License.CompanyName}");
            }
            return JsonConvert.SerializeObject(list);
        }


        #region Private Method
        private async Task<bool> CheckDashboardNameDuplicate(int DId, string Name,int UId,string ConnectionString)
        {
            using (var con = CreateConnection(ConnectionString))
            {
               var query = "select * from SLUserdashboard Where name = @name and Id <> @id and UserId = @userid";
                var param = new DynamicParameters();
                param.Add("name", Name);
                param.Add("id", DId);
                param.Add("userid",UId);
                var dt = await con.QueryAsync(query,param);
                return dt.Any();
            }
        }
        private async Task<int> InsertDashbaord(string ConnectionString,string Name,int UId, string Json)
        {
            int DashboardId = 0;
            using (var conn = CreateConnection(ConnectionString))
            {
                var queryParam = new DynamicParameters();
                queryParam.Add("Name", Name);
                queryParam.Add("UserId", UId);
                queryParam.Add("Json", Json);

                DashboardId = await conn.QuerySingleAsync<int>(DashboardInsertQ, queryParam);
            }

            return DashboardId;
        }
        private async Task<SLUserDashboard> GetDashbaordId(int Id,string ConnectionString)
        {
            var ud = new SLUserDashboard();
            using (var conn = CreateConnection(ConnectionString))
            {
                ud = await conn.QueryFirstOrDefaultAsync<SLUserDashboard>(DashboardGetIdBaseQ, new { DashboardId = Id });
            }

            return ud;
        }
        private async Task<string> GetDashboardListHtml(string ConnectionString,int UserId)
        {
            string dashboardListHtml = string.Empty;
           
            var htmlDashboard = new StringBuilder();
            var dashboardList = new List<SLUserDashboard>();
            using (var con = CreateConnection(ConnectionString))
            {
                var res = await con.QueryAsync<SLUserDashboard>(DashboardGetListQ, new { UserId = UserId });
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
        private string ConvertArrayToString(JArray arr)
        {
            try
            {
                var appendSt = new StringBuilder();
                for (int a = 0; a <= arr.Count() - 1; a++)
                {
                    if ((arr.Count() - 1).Equals(a))
                    {
                        appendSt.Append("'").Append(arr[a]).Append("'");
                    }
                    else
                    {
                        appendSt.Append("'").Append(arr[a]).Append("'").Append(",");
                    }
                }
                string finalSt = Convert.ToString(appendSt);
                return finalSt;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                return "";
            }

        }
        private async Task<List<TableModel>> GetTableNames(object tableIds, string ConnectionString)
        {
            var query = TableNameQ.Replace("[TableIds]", Convert.ToString(tableIds));
            var tbList  = new List<TableModel>();
            using (var con = CreateConnection(ConnectionString))
            {
                var res = await con.QueryAsync<TableModel>(query);
                tbList = res.ToList();
            }
            return tbList;
        }
        private async Task<List<ChartModel>> GetBarPieChartData(string tableName,int viewId,string columnName,Passport passport)
        {
            var _query = new Query(passport);
            var @params = new Parameters(viewId, passport);
            @params.fromChartReq = true;
            @params.IsMVCCall = true;
            _query.RefineSQL(@params);
            var chartList = new List<ChartModel>();

            bool isDateTime = await CheckIsDatetimeColumn(Convert.ToString(tableName), Convert.ToString(columnName),passport.ConnectionString);

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

            using(var con = CreateConnection(passport.ConnectionString))
            {
                var res = await con.QueryAsync<ChartModel>(sql);
                chartList = res.ToList();
            }
            return chartList;

        }
        private async Task<bool> CheckIsDatetimeColumn(string tableName, string column,string ConnectionString)
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
        private async  Task<List<ChartModel>> GetTrackedChartData(object tableIds, object filter, object period,string connectionString)
        {
            var query = string.Empty;
            if (filter.Equals("hour"))
            {
                query = TrackedChartQuery.Replace("[Filter]", "Convert(varchar , TransactionDateTime, 1) + ' ' +cast(datepart(hour,TransactionDateTime)as nvarchar) + ':00'").Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }
            else if (filter.Equals("day"))
            {
                query = TrackedChartQuery.Replace("[Filter]", "CONVERT(varchar , TransactionDateTime, 1)").Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }
            else if (filter.Equals("week"))
            {
                // Me.Query = Me.TrackedChartQuery.Replace("[Filter]", "DATENAME(week,TransactionDateTime) + 'th week/' + DATENAME(YEAR,TransactionDateTime)").Replace("[TableIds]", tableIds).Replace("[Period]", period)
                query = TrackedChartQueryWeek.Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }
            else
            {
                query = TrackedChartQuery.Replace("[Filter]", "cast(month(TransactionDateTime) as nvarchar) + '/' +  cast(year(TransactionDateTime) as nvarchar)").Replace("[TableIds]", Convert.ToString(tableIds)).Replace("[Period]", Convert.ToString(period));
            }

            var list = new List<ChartModel>();

            using(var con = CreateConnection(connectionString))
            {
                var res = await con.QueryAsync<ChartModel>(query);
                list = res.ToList();
            }
            return list;
        }
        private async Task<List<ChartOperatinModelRes>> GetOperationChartData(string tableIds, string usersIds, string AuditTypeIds, string period, string filter,string connectionString)
        {
            string AuditTypeQuery = "";
            string query = string.Empty;

            if (!string.IsNullOrEmpty(Convert.ToString(AuditTypeIds)))
            {
                AuditTypeQuery = " and sl.ActionType in (" + AuditTypeIds + ") ";
            }

            if (filter.Equals("hour"))
            {
                query = OperationChartQuery.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
                query = query.Replace("[Filter]", "Convert(varchar , UpdateDateTime, 1) + ' ' +cast(datepart(hour,UpdateDateTime) as nvarchar) + ':00'");
            }
            else if (filter.Equals("day"))
            {
                query = OperationChartQuery.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
                query = query.Replace("[Filter]", "CONVERT(varchar , UpdateDateTime, 1)");
            }
            else if (filter.Equals("week"))
            {
                // Me.Query = Me.Query.Replace("[Filter]", "DATENAME(week,UpdateDateTime) + 'th week/' + DATENAME(YEAR,UpdateDateTime)")
                query = OperationChartQueryWeek.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
            }
            else
            {
                query = OperationChartQuery.Replace("[TableIds]", tableIds).Replace("[UserIds]", usersIds).Replace("[AuditType]", AuditTypeQuery).Replace("[Period]", period);
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
        private async Task<List<ChartModel>> GetTimeSeriesChartData(string tableName, string columnName, int viewId, string period, string filter,Passport passport)
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

            PeriodQuery = PeriodQuery.Replace("[Period]", period.ToString());
            string query = "";
            if (filter.Equals("week"))
            {

                query = PeriodQuery + @" Declare @temptbl table (X nvarchar(12), Y int, WK int);
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
                query = PeriodQuery + " SELECT " + filterSql + " AS [X], COUNT(*) AS [Y] FROM (" + @params.SQL + @") as tbl 
                                    WHERE " + columnName + @">=@fromdate 
                                    AND " + columnName + "<= @todate GROUP BY " + filterSql + " order by " + filterSql + " asc";
            }

            using(var con = CreateConnection(passport.ConnectionString))
            {
                var res = await con.QueryAsync<ChartModel>(query);
                list = res.ToList();
            }
            return list;
        }

        #endregion
    }
}
