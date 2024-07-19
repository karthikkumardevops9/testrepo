using System;
using MSRecordsEngine.Services;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
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
using MSRecordsEngine.Services.Interface;
using MSRecordsEngine.Models.FusionModels;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private IDbConnection CreateConnection(string connectionString)
         => new SqlConnection(connectionString);
        private readonly CommonControllersService<DashBoardController> _commonService;
        private readonly IDashboardService _dashboardService;
        private readonly ILayoutModelService _layoutModelService;       
        public DashBoardController(CommonControllersService<DashBoardController> commonService,IDashboardService dashboardService,ILayoutModelService layoutModelService)
        {
            _commonService = commonService;
            _dashboardService = dashboardService;
            _layoutModelService = layoutModelService;
        }

        [Route("GetDashboardList")]
        [HttpPost]
        public async Task<DashboardModel> GetDashboardList(DashBoardParam param)
        {
            DashboardModel result = new DashboardModel();
            try
            {
                result.DashboardListHtml = await _dashboardService.GetDashboardListHtml(param.ConnectionString, param.UserId);
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
        public DashboardCommonModel GetWorkGroupTableMenu(GetWorkGroupTableMenuParam param)
        {
            DashboardCommonModel res = new DashboardCommonModel();
            var itemList = new List<TableItem>();
            try
            {
                itemList = Navigation.GetWorkGroupMenu(param.WorkGroupId, param.Passport).OrderBy(x => x.UserName).ToList();
                res.ListAsString = JsonConvert.SerializeObject(itemList);
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
        public async Task<DashboardCommonModel> SetDashboardDetails(SetDashboardDetailsParam param)
        {
            DashboardCommonModel result = new DashboardCommonModel();
            try
            {
                var checkExist = await _dashboardService.CheckDashboardNameDuplicate(0, param.Name, param.UserId, param.ConnectionString);
                if (checkExist)
                {
                    result.ErrorMessage = "Already exists name.";
                }
                else
                {
                    var resId = await _dashboardService.InsertDashbaord(param.ConnectionString, param.Name, param.UserId, "");
                    if (resId > 0)
                    {
                        result.ErrorMessage = "Added successfully";
                        result.ud = await _dashboardService.GetDashbaordId(resId, param.ConnectionString);
                        result.DashboardListHtml = await _dashboardService.GetDashboardListHtml(param.ConnectionString, param.UserId);
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
        public DashboardCommonModel GetViewMenu(GetViewMenuParams param)
        {
            DashboardCommonModel result = new DashboardCommonModel();
            var viewByList = new List<ViewItem>();
            try
            {
                viewByList = Navigation.GetViewsByTableName(param.TableName, param.passport).OrderBy(x => x.ViewName).ToList();
                result.ListAsString = JsonConvert.SerializeObject(viewByList);
            }
            catch (Exception ex)
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return result;
        }

        [Route("GetDashboardDetails")]
        [HttpPost]
        public async Task<DashboardCommonModel> GetDashboardDetails(GetDashboardDetailParam param)
        {
            DashboardCommonModel result = new DashboardCommonModel();
            try
            {
                result.ud = await _dashboardService.GetDashbaordId(param.DashboardId,param.ConnectionString);
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
            try
            {
                lis =  _dashboardService.ViewColumns(param.ViewId, param.ShortDatePattern, param.Passport);
            }
            catch (Exception ex) 
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.Passport.DatabaseName} CompanyName: {param.Passport.License.CompanyName}");
            }

            return JsonConvert.SerializeObject(lis);
        }

        [Route("GetViewColumnOnlyDate")]
        [HttpPost]
        public string GetViewColumnOnlyDate(GetViewColumnMenuParam param)
        {
            var lis = new List<CommonDropdown>();
            try
            {
                lis = _dashboardService.GetViewColumnOnlyWithType(param.TableName,param.ViewId, param.ShortDatePattern, param.Passport);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.Passport.DatabaseName} CompanyName: {param.Passport.License.CompanyName}");
            }

            return JsonConvert.SerializeObject(lis);
        }

        [Route("SetDashboardJson")]
        [HttpPost]
        public async Task<DashboardCommonModel> SetDashboardJson(SetDashboardJsonParam param)
        {
            DashboardCommonModel result = new DashboardCommonModel();
            try
            {
                var resId = await _dashboardService.UpdateDashboardJson(param.Json,param.DashboardId,param.ConnectionString);
                if (resId > 0)
                {
                    result.Msg = "Update successfully";
                    result.ud = await _dashboardService.GetDashbaordId(resId, param.ConnectionString);
                }
                else
                {
                    result.isError = true;
                    result.Msg = "Fail to update";
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
                pTableList = await _dashboardService.TrackableTable(passport);
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
                    var uList = await _dashboardService.Users(passport.ConnectionString);
                    uList = uList.OrderBy(x => x.Name).ToList();
                    result.Users = JsonConvert.SerializeObject(uList);

                    pTablesList = await _dashboardService.AuditTable(passport);
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
        public async Task<DashboardCommonModel> RenameDashboardName(RenameDashboardNameParam param)
        {
            DashboardCommonModel result = new DashboardCommonModel();
            try
            {
                using(var con = CreateConnection(param.ConnectionString))
                {
                    if(await _dashboardService.CheckDashboardNameDuplicate(param.DashboardId, param.Name, param.UserId, param.ConnectionString))
                    {
                        result.ErrorMessage = "Already exists name.";
                    }
                    else
                    {
                        var resId = await _dashboardService.UpdateDashbaordName(param.Name, param.DashboardId, param.ConnectionString);
                        if (resId > 0)
                        {
                            result.ErrorMessage = "Update successfully";
                            result.ud = await _dashboardService.GetDashbaordId(resId, param.ConnectionString);
                            result.DashboardListHtml = await _dashboardService.GetDashboardListHtml(param.ConnectionString, param.UserId);
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
        public async Task<DashboardCommonModel> DeleteDashboard(DeleteDashboardParam param)
        {
            DashboardCommonModel result = new DashboardCommonModel();
            try
            {
                var resId = await _dashboardService.DeleteDashboard(param.DashboardId, param.ConnectionString);
                if (resId > 0)
                {
                    result.Msg = "Delete Successfully";
                }
                else
                {
                    result.isError = true;
                    result.Msg = "Fail to delete dashboard";
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
                        var TableNames = await _dashboardService.GetTableNames(TableIds,param.Passport.ConnectionString);
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
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.Passport.DatabaseName} CompanyName: {param.Passport.License.CompanyName}");
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

                var chartList = await _dashboardService.GetBarPieChartData(TableName, Convert.ToInt32(ParentView), ColumnName,param.passport);
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
                var TableNames = await _dashboardService.GetTableNames(TableIds,param.passport.ConnectionString);

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
                var list = await _dashboardService.GetTrackedChartData(TableIds, Filter, pe, param.passport.ConnectionString);
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
                var TableNames = await _dashboardService.GetTableNames(TableIds,param.passport.ConnectionString);
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

                var list = await _dashboardService.GetOperationChartData(TableIds, UserIds, AuditTypeId, Period, Filter,param.passport.ConnectionString);
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
                var list = await _dashboardService.GetTimeSeriesChartData(parseJson["TableName"].ToString(), ColumnName, Convert.ToInt32(ParentView), parseJson["Period"].ToString(), parseJson["Filter"].ToString(),param.passport);
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

        [Route("GetTaskList")]
        [HttpPost]
        public ChartDataResModel GetTaskList(widgetDataParam param)
        {
            var model = new ChartDataResModel();
            try
            {
                model.JsonString = param.widgetObjectJson;
                model.TaskList = _layoutModelService.ExecuteDashboardTasksbar(param.passport);
            }
            catch (Exception ex) { 
                model.isError= true;
                model.Msg = "";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        } 

        [Route("UpdateFavouriteDashboard")]
        [HttpPost]
        public async Task<DashboardCommonModel> UpdateFavouriteDashboard(UpdateFavDashboardParam param)
        {
            var result = new DashboardCommonModel();
            try
            {
                using (var context = new TABFusionRMSContext(param.ConnectionString))
                {
                    var ud = await context.UserDashboards.FirstOrDefaultAsync(x => x.ID.Equals(param.DashboardId));
                    ud.IsFav = param.IsFav;
                    context.Entry(ud).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                }
                result.DashboardListHtml = await _dashboardService.GetDashboardListHtml(param.ConnectionString, param.UserId);
            }
            catch(Exception ex)
            {
                result.isError = true;
                result.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("CountBarChartData")]
        [HttpGet]
        public async Task<CountDataReturn> CountBarChartData(string WidgetObjectJson,string ConnectionString)
        {
            var result = new CountDataReturn();
            result.JsonString = WidgetObjectJson;

            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(WidgetObjectJson);
                var TableName = parseJson["TableName"].ToString();
                var ColumnName = parseJson["Column"].ToString();
                var list = await _dashboardService.GetBarPieChartDataCount(TableName, ColumnName, ConnectionString);
                result.Count = list;
            }
            catch(Exception ex)
            {
                result.isError = true;
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return result;
        }

        [Route("CountTimeSeriesChartData")]
        [HttpPost]
        public async Task<CountDataReturn> CountTimeSeriesChartData(widgetDataParam param)
        {
            var result = new CountDataReturn();
            result.JsonString = param.widgetObjectJson;
            try
            {
                using(var context = new TABFusionRMSContext(param.passport.ConnectionString))
                {
                    var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                    var ParentView = Convert.ToInt16(parseJson["ParentView"].ToString());
                    var ViewName = await context.Views.Where(x => x.Id == ParentView).Select(x => x.ViewName).FirstOrDefaultAsync();
                    if (ViewName.Length > 0)
                    {
                        var ress = param.passport.CheckPermission(ViewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.View);
                        if (ress == false)
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }

                    var TableName = parseJson["TableName"].ToString();
                    var ColumnName = parseJson["Column"].ToString();
                    var Period = parseJson["Period"].ToString();
                    var Filter = parseJson["Filter"].ToString();

                    var Count = await _dashboardService.GetTimeSeriesChartDataCount(TableName, ColumnName, Period, Filter,param.passport.ConnectionString);
                    result.Count = Count;
                }
            }
            catch(Exception ex)
            {
                result.isError = true;
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return result;
        }

        [Route("CountOperationsData")]
        [HttpPost]
        public async Task<CountDataReturn> CountOperationsData(widgetDataParam param)
        {
            var res = new CountDataReturn();
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                string TableIds = this.ConvertArrayToString((JArray)parseJson["Objects"]);
                var TableNames = await _dashboardService.GetTableNames(TableIds,param.passport.ConnectionString);
                bool Permission = true;
                foreach (TableModel item in TableNames)
                {
                    if (param.passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View) == false)
                    {
                        Permission = false;
                        break;
                    }
                }

                if (Permission == false)
                {
                    return res;
                }

                string UserIds = this.ConvertArrayToString((JArray)parseJson["Users"]);
                string AuditTypeId = this.ConvertArrayToString((JArray)parseJson["AuditType"]);
                var Period = parseJson["Period"].ToString();
                var Filter = parseJson["Filter"].ToString();
                var count = await _dashboardService.GetOperationChartDataCount(TableIds, UserIds, AuditTypeId, Period, Filter, param.passport.ConnectionString);
                res.Count = count;
            }
            catch (Exception ex) 
            {
                res.isError = true;
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return res;
        }

        [Route("CountTrackedData")]
        [HttpPost]
        public async Task<CountDataReturn> CountTrackedData(widgetDataParam param)
        {
            var result = new CountDataReturn();
            result.JsonString = param.widgetObjectJson;
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                string TableIds = this.ConvertArrayToString((JArray)parseJson["Objects"]);
                var TableNames = await _dashboardService.GetTableNames(TableIds, param.passport.ConnectionString);
                bool Permission = true;
                foreach (TableModel item in TableNames)
                {
                    if (param.passport.CheckPermission(item.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View) == false)
                    {
                        Permission = false;
                        break;
                    }
                }
                if (Permission == false)
                {
                    return result;
                }
                var Filter = parseJson["Filter"].ToString();
                var pe = parseJson["Period"].ToString();
                var Count = await _dashboardService.GetTrackedChartDataCount(TableIds, Filter, pe, param.passport.ConnectionString);
                result.Count = Count;
                return result;
            }
            catch(Exception ex)
            {
                result.isError = true;
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return result;
        }

        [Route("CheckDataGridPermission")]
        [HttpPost]
        public ChartDataResModel CheckDataGridPermission(widgetDataParam param) 
        {
            var model = new ChartDataResModel();
            model.JsonString = param.widgetObjectJson;
            model.Permission = true;
            try
            {
                var parseJson = JsonConvert.DeserializeObject<JObject>(param.widgetObjectJson);
                var ParentView = parseJson["ParentView"].ToString();
                int workgroupId = Convert.ToInt32(parseJson["WorkGroup"].ToString());
                var workgroup = Navigation.GetWorkGroups(param.passport).OrderBy(x => x.WorkGroupName).ToList();
                var workgroupname = workgroup.FirstOrDefault(x => x.ID == workgroupId).WorkGroupName;
                if(workgroupname == null)
                {
                    return model;
                }
                // check workgroup permission
                if (!param.passport.CheckPermission(workgroupname, Smead.Security.SecureObject.SecureObjectType.WorkGroup, Permissions.Permission.Access))
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
                return model;

            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        #region Private Method
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
        #endregion
    }
}
