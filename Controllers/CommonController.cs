using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MSRecordsEngine.Services;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Entities;
using Newtonsoft.Json;
using System.Linq;
using System.Data.Entity;
using MSRecordsEngine.Models;
using Dapper;
using MSRecordsEngine.RecordsManager;
using System.Collections.Generic;
using Smead.Security;
using MSRecordsEngine.Services.Interface;
using System.IO;
using MSRecordsEngine.Imaging;
using NLog.Filters;
using System.Drawing.Printing;
using Leadtools.Document.Unstructured.Highlevel;
using MSRecordsEngine.Properties;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly CommonControllersService<CommonController> _commonService;
        private readonly ITrackingServices _trackingServices;

        private IDbConnection CreateConnection(string connectionString)
            => new SqlConnection(connectionString);
        public CommonController(CommonControllersService<CommonController> commonControllersService, ITrackingServices trackingServices)
        {
            _commonService = commonControllersService;
            _trackingServices = trackingServices;
        }

        [Route("GetGridViewSettings")]
        [HttpGet]
        public async Task<string> GetGridViewSettings(string pGridName, string ConnectionString)
        {
            var jsonObject = string.Empty;

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var lGridColumns = await context.vwGridSettings.Where(x => x.GridSettingsName.ToLower().Trim().Equals(pGridName.ToLower().Trim()) && x.IsActive == true).
                    Select(a => new
                    {
                        srno = a.GridColumnSrNo,
                        name = a.GridColumnName,
                        sortable = a.IsSortable,
                        columnWithCheckbox = a.IsCheckbox,
                        displayName = a.GridColumnDisplayName
                    }).ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(lGridColumns, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }

            return jsonObject;
        }


        [Route("GetGridViewSettings1")]
        [HttpGet]
        public async Task<string> GetGridViewSettings1(string pGridName, string ConnectionString)
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var dtRecords = new DataTable();
                    var pTableEntity = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(pGridName.Trim().ToLower())).FirstOrDefaultAsync();
                    Databas pDatabaseEntity = null;
                    if (pTableEntity is not null)
                    {
                        if (!string.IsNullOrEmpty(pTableEntity.DBName))
                        {
                            pDatabaseEntity = await context.Databases.Where(x => x.DBName.Trim().ToLower().Equals(pTableEntity.DBName.Trim().ToLower())).FirstOrDefaultAsync();
                            ConnectionString = _commonService.GetConnectionString(pDatabaseEntity, false);
                        }
                    }
                    using (var conn = CreateConnection(ConnectionString))
                    {
                        var param = new DynamicParameters();
                        param.Add("@TableName", pGridName);
                        param.Add("@PageNo", 1);
                        param.Add("@PageSize", 0);
                        param.Add("@DataAndColumnInfo", false);
                        param.Add("@ColName", "");
                        param.Add("@Sort", "");

                        var loutput = await conn.ExecuteReaderAsync("SP_RMS_GetTableData", param, commandType: CommandType.StoredProcedure);
                        dtRecords.Load(loutput);

                        var dateColumn = new List<Int32>();
                        Int32 i = 0;
                        var GridColumnEntities = new List<GridColumns>();

                        foreach (DataColumn column in dtRecords.Columns)
                        {
                            var GridColumnEntity = new GridColumns();
                            GridColumnEntity.ColumnSrNo = i + 1;
                            GridColumnEntity.ColumnId = i + 1;
                            GridColumnEntity.ColumnName = column.ColumnName;
                            GridColumnEntity.ColumnDisplayName = column.ColumnName;
                            GridColumnEntity.ColumnDataType = column.DataType.Name;
                            GridColumnEntity.ColumnMaxLength = column.MaxLength.ToString();
                            GridColumnEntity.IsPk = column.Unique;
                            GridColumnEntity.AutoInc = column.AutoIncrement;
                            GridColumnEntity.IsNull = column.AllowDBNull;
                            GridColumnEntity.ReadOnlye = column.ReadOnly;
                            GridColumnEntities.Add(GridColumnEntity);

                            if (column.DataType.Name.Trim().ToString().IndexOf("Date") >= 0)
                            {
                                dateColumn.Add(i);
                            }
                            i = i + 1;
                        }

                        Int32 j = 0;
                        foreach (DataRow rows in dtRecords.Rows)
                        {
                            foreach (Int32 item in dateColumn)
                            {
                                var tempDate = dtRecords.Rows[j].ItemArray[item];
                                if (!(tempDate == DBNull.Value))
                                {
                                    DateTime convertInDate;
                                    if (DateTime.TryParse(tempDate.ToString(), out convertInDate))
                                    {
                                        rows[item] = DateTime.Parse(tempDate.ToString());
                                    }
                                }
                            }
                            j = j + 1;
                        }

                        var lGridColumns = GridColumnEntities.Select(a => new
                        {
                            srno = a.ColumnSrNo,
                            name = a.ColumnName,
                            displayName = a.ColumnDisplayName,
                            dataType = a.ColumnDataType,
                            maxLength = a.ColumnMaxLength,
                            isPk = a.IsPk,
                            isNull = !a.IsNull,
                            autoInc = a.AutoInc,
                            readOnly = a.ReadOnlye
                        });

                        var Setting = new JsonSerializerSettings();
                        Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                        jsonObject = JsonConvert.SerializeObject(lGridColumns, Formatting.Indented, Setting);
                    }
                }
                return jsonObject;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("SetGridOrders")]
        [HttpPost]
        public async Task<bool> SetGridOrders(SetGridOrders_Requesst setGridOrders_Requesst)
        {
            var pGridName = setGridOrders_Requesst.GridName;
            var ConnectionString = setGridOrders_Requesst.ConnectionString;
            var gridSettingsColumns = setGridOrders_Requesst.gridSettingsColumns;

            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    List<GridSettingsColumns> lGridSettingsColumnsEntities = gridSettingsColumns;
                    var pGridSettingsEntity = await context.vwGridSettings.Where(x => x.GridSettingsName.ToLower().Trim().Equals(pGridName.ToLower().Trim())).FirstOrDefaultAsync();

                    if (pGridSettingsEntity == null)
                    {
                        return false;
                    }
                    int pGridSettingId = pGridSettingsEntity.GridSettingsId;
                    var lGridColumnEntities = await context.GridColumns.Where(x => x.GridSettingsId == pGridSettingId && x.IsActive == true).ToListAsync();
                    int iCount = 0;
                    foreach (GridSettingsColumns pGridSettingsColumnsEntity in lGridSettingsColumnsEntities)
                    {
                        if (pGridSettingsColumnsEntity.index is not null)
                        {

                            var pGridColumnEntities = lGridColumnEntities.Where(x => x.GridColumnName.Trim().ToLower().Equals(pGridSettingsColumnsEntity.index.Trim().ToLower())).FirstOrDefault();

                            if (pGridSettingsColumnsEntity.key == true)
                            {
                                pGridColumnEntities.GridColumnSrNo = -1;
                            }
                            else
                            {
                                pGridColumnEntities.GridColumnSrNo = iCount;
                            }

                            //_iGridColumn.Update(pGridColumnEntities);
                            context.Entry(pGridColumnEntities).State = EntityState.Modified;
                            await context.SaveChangesAsync();

                            if (pGridSettingsColumnsEntity.key == false)
                            {
                                iCount += 1;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("GetTableListLabel")]
        [HttpPost]
        public async Task<string> GetTableListLabel(Passport passport)
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var pTableList = from t in await context.Tables.OrderBy(x => x.TableName).ToListAsync() select t;
                    var lAllTables = await context.vwTablesAlls.Select(x => x.TABLE_NAME).ToListAsync();
                    pTableList = pTableList.Where(x => lAllTables.Contains(x.TableName));

                    List<Table> tableList = new List<Table>();

                    foreach (Table tempTable in pTableList)
                    {
                        if ((passport.CheckPermission(tempTable.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Smead.Security.Permissions.Permission.View)))
                            tableList.Add(tempTable);
                    }

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(tableList, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }


        [Route("GetTableList")]
        [HttpGet]
        public async Task<string> GetTableList(string ConnectionString)
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pTableList = from t in await context.Tables.OrderBy(x => x.TableName).ToListAsync() select t;
                    var lAllTables = await context.vwTablesAlls.Select(x => x.TABLE_NAME).ToListAsync();
                    pTableList = pTableList.Where(x => lAllTables.Contains(x.TableName));

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pTableList, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }


        [Route("GetTrackableTableList")]
        [HttpGet]
        public async Task<string> GetTrackableTableList(string ConnectionString)
        {
            var jsonObject = string.Empty;
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pTableList = await context.Tables.Where(x => x.TrackingTable > 0 || x.Trackable == true).ToListAsync();
                    var lAllTables = await context.vwTablesAlls.Select(x => x.TABLE_NAME).ToListAsync();
                    pTableList = pTableList.Where(x => lAllTables.Contains(x.TableName)).ToList();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pTableList, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }


        [Route("GetColumnList")]
        [HttpGet]
        public async Task<string> GetColumnList(string pTableName, int type, string ConnectionString)
        {
            try
            {
                var Setting = new JsonSerializerSettings();
                Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                string sSQL;

                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    if (type == 0)
                    {
                        var pColumnLists = await context.vwColumnsAlls.ToListAsync();
                        var pColumnList = pColumnLists.Where(x => x.TABLE_NAME.Trim().ToLower().Equals(pTableName.Trim().ToLower()));
                        pColumnList = pColumnList.Where(x => !x.COLUMN_NAME.StartsWith("%sl"));

                        var jsonObject = JsonConvert.SerializeObject(pColumnList, Formatting.Indented, Setting);
                        if (jsonObject.Length == 2)
                        {
                            var oTables = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(pTableName.Trim().ToLower())).FirstOrDefaultAsync();
                            Databas pDatabaseEntity = null;
                            if (oTables != null)
                            {
                                if (oTables.DBName != null)
                                {
                                    pDatabaseEntity = await context.Databases.Where(x => x.DBName.Trim().ToLower().Equals(oTables.DBName.Trim().ToLower())).FirstOrDefaultAsync();
                                }
                                if (pDatabaseEntity != null)
                                {
                                    ConnectionString = _commonService.GetConnectionString(pDatabaseEntity, false);
                                }
                            }

                            sSQL = "SELECT ROW_NUMBER() OVER (ORDER BY COLUMN_NAME) AS ID, COLUMN_NAME, TABLE_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + pTableName + "'";
                            var colname = CommonFunctions.GetRecords<dynamic>(ConnectionString, sSQL).ToList();
                            var collist = new List<string>();
                            foreach (var col in colname)
                            {
                                collist.Add(col.COLUMN_NAME);
                            }
                            jsonObject = JsonConvert.SerializeObject(collist, Formatting.Indented, Setting);
                        }
                        return jsonObject;
                    }
                    else
                    {
                        sSQL = pTableName.Split(new string[] { "WHERE" }, StringSplitOptions.None)[0];
                        var tColumnList = new List<string>();
                        using (var conn = CreateConnection(ConnectionString))
                        {
                            var dt = new DataTable();
                            var res = await conn.ExecuteReaderAsync(sSQL.Text(), commandType: CommandType.Text);
                            dt.Load(res);
                            for (int i = 0, loopTo = dt.Columns.Count - 1; i <= loopTo; i++)
                            {
                                if (!dt.Columns[i].ToString().StartsWith("%sl"))
                                {
                                    tColumnList.Add(dt.Columns[i].ToString());
                                }
                            }

                            var jsonObj = JsonConvert.SerializeObject(tColumnList, Formatting.Indented, Setting);
                            return jsonObj;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("TruncateTrackingHistory")]
        [HttpGet]
        public async Task<ReturnErrorTypeErrorMsg> TruncateTrackingHistory(string ConnectionString, string sTableName = "", string sId = "")
        {
            var model = new ReturnErrorTypeErrorMsg();
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    bool catchFlag;
                    string KeysType = "";
                    var innerTruncateTrackingHistory = await _trackingServices.InnerTruncateTrackingHistory(ConnectionString, (await context.Systems.ToListAsync()), sTableName, sId, KeysType);
                    if (innerTruncateTrackingHistory.Success)
                    {
                        if (innerTruncateTrackingHistory.KeysType == "s")
                        {
                            model.ErrorType = "s";
                            model.ErrorMessage = "History has been truncated";
                        }
                        else
                        {
                            model.ErrorType = "w";
                            model.ErrorMessage = "No more history to truncate";
                        }
                    }
                    else
                    {
                        model.ErrorType = "w";
                        model.ErrorMessage = "For another use";
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                model.ErrorType = "e";
                model.ErrorMessage = "Oops an error occurred.  Please contact your administrator.";
            }
            return model;
        }


        [Route("GetRegisteredDatabases")]
        [HttpGet]
        public async Task<string> GetRegisteredDatabases(string ConnectionString)
        {
            var jsonObject = "";
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var pDatabase = await context.Databases.ToListAsync();

                    var Setting = new JsonSerializerSettings();
                    Setting.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                    jsonObject = JsonConvert.SerializeObject(pDatabase, Formatting.Indented, Setting);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return jsonObject;
        }


        [Route("CheckAttachmentMaxSize")]
        [HttpPost]
        public async Task<CheckAttachmentMaxSize_Response> CheckAttachmentMaxSize(CheckAttachmentMaxSize_Request checkAttachmentMaxSize_Request)
        {
            var model = new CheckAttachmentMaxSize_Response();
            var ConnectionString = checkAttachmentMaxSize_Request.Passport.ConnectionString;
            var filesizeMB = checkAttachmentMaxSize_Request.FileSizeMB;
            try
            {
                using (var context = new TABFusionRMSContext(checkAttachmentMaxSize_Request.Passport.ConnectionString))
                {
                    var param = new CheckaddNewAttachmentPermission_Request();
                    param.Passport = checkAttachmentMaxSize_Request.Passport;
                    param.TableName = checkAttachmentMaxSize_Request.TableName;
                    var checkaddNewAttachmentPermission = await CheckaddNewAttachmentPermission(param);
                    if (checkaddNewAttachmentPermission.Success)
                    {
                        var expMaxSize = (await context.Settings.Where(x => x.Section == "DragAndDropAttachment" && x.Item == "MaxSize").FirstOrDefaultAsync())?.ItemValue;
                        if (!(await CheckFilesize(ConnectionString, filesizeMB)) && !string.IsNullOrEmpty(checkAttachmentMaxSize_Request.TableId) || checkAttachmentMaxSize_Request.TableName == "Orphans")
                        {
                            model.CheckConditions = "maxsize";
                            model.WarringMsg = string.Format("The file you are attaching is bigger than the server allows. File attachment size limit is set to {0} MB. Please contact Administrator to change the limit.", expMaxSize);
                        }
                        else
                        {
                            model.CheckConditions = "success";
                        }
                    }
                    else
                    {
                        model.CheckConditions = "permission";
                    }
                    model.ErrorNumber = checkaddNewAttachmentPermission.ErrorNumber;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return model;
        }


        [Route("GetWorkGroupList")]
        [HttpPost]
        public List<WorkGroupItem> GetWorkGroupList(Passport passport)
        {
            List<WorkGroupItem> wlist = new List<WorkGroupItem>();
            try
            {
                wlist = Navigation.GetWorkGroups(passport).OrderBy(x => x.WorkGroupName).ToList();
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return wlist;
        }


        [Route("SaveOrphanAttachment")]
        [HttpPost]
        public void SaveOrphanAttachment(SaveOrphanAttachment_Request saveOrphanAttachment_Request)
        {
            try
            {
                int counter = 0;
                var filePathList = saveOrphanAttachment_Request.FileInfoModels;
                var filesinfo = saveOrphanAttachment_Request.DocumentViewrApiModels;
                var passport = saveOrphanAttachment_Request.Passport;
                var ticket = passport.get_CreateTicket(string.Format(@"{0}\{1}", passport.ServerName, passport.DatabaseName), Imaging.Attachments.OrphanName, "").ToString();
                foreach (var item in filesinfo)
                {
                    var OrgFileName = filePathList[counter].OrgFileName;
                    Imaging.Attachments.AddOrphan(ticket, passport.UserId, passport, "", OrgFileName, item, Path.GetExtension(item.FilePath), true);
                    counter++;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("OrphanPartial")]
        [HttpPost]
        public int OrphanPartial(Passport passport)
        {
            int TotalRecords = 0;
            try
            {
                var dt = Attachments.GetAllOrphansCount(passport, "");
                TotalRecords = dt.Rows[0].ItemArray[0].IntValue();
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return TotalRecords;
        }


        [Route("CheckaddNewAttachmentPermission")]
        [HttpPost]
        public async Task<CheckaddNewAttachmentPermission_Response> CheckaddNewAttachmentPermission(CheckaddNewAttachmentPermission_Request _Request)
        {
            var model = new CheckaddNewAttachmentPermission_Response();
            var tableName = _Request.TableName;
            var passport = _Request.Passport;

            try
            {
                using (var context = new TABFusionRMSContext(passport.ConnectionString))
                {
                    var oSystem = await context.Systems.FirstOrDefaultAsync();
                    var oOutputSetting = await context.OutputSettings.Where(x => x.Id.Trim().ToLower().Equals(oSystem.DefaultOutputSettingsId.Trim().ToLower())).FirstOrDefaultAsync();
                    var oVolume = await context.Volumes.Where(x => x.Id == oOutputSetting.VolumesId).FirstOrDefaultAsync();
                    model.ExpMaxSize = (await context.Settings.Where(x => x.Section == "DragAndDropAttachment" && x.Item == "MaxSize").FirstOrDefaultAsync())?.ItemValue;
                    if (!passport.CheckPermission(oOutputSetting.Id, Smead.Security.SecureObject.SecureObjectType.OutputSettings, Smead.Security.Permissions.Permission.Access))
                    {
                        model.ErrorNumber = 2;
                        model.Success = false;
                        return model;
                    }
                    if (string.IsNullOrWhiteSpace(tableName) || string.Compare(tableName, Imaging.Attachments.OrphanName) == 0)
                    {
                        model.Success = true;
                        return model;
                    }
                    if (!passport.CheckPermission(tableName, Smead.Security.SecureObject.SecureObjectType.Attachments, Smead.Security.Permissions.Permission.Add))
                    {
                        model.ErrorNumber = 0;
                        model.Success = false;
                        return model;
                    }
                    if (!passport.CheckPermission(oVolume.Name, Smead.Security.SecureObject.SecureObjectType.Volumes, Smead.Security.Permissions.Permission.Add))
                    {
                        model.ErrorNumber = 1;
                        model.Success = false;
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            model.Success = true;
            return model;
        }


        [Route("LoadAttachmentData")]
        [HttpPost]
        public async Task<LoadAttachmentData_Response> LoadAttachmentData(LoadAttachmentData_Request _Request)
        {
            var model = new LoadAttachmentData_Response();
            try
            {
                using (var context = new TABFusionRMSContext(_Request.Passport.ConnectionString))
                {
                    var tableId = Navigation.PrepPad(_Request.TableName, _Request.TableId, _Request.Passport);
                    model.TableId = tableId;
                    model.Table = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(_Request.TableName.Trim().ToLower())).FirstOrDefaultAsync();
                    model.Display = Navigation.GetItemName(_Request.TableName, tableId, _Request.Passport);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return model;
        }


        [Route("LoadAttachmentOrphanData")]
        [HttpPost]
        public async Task<List<GetAllOrphansData>> LoadAttachmentOrphanData(LoadAttachmentOrphanData_Request _Requesst)
        {
            var orphanList = new List<GetAllOrphansData>();
            try
            {
                var param = new DynamicParameters();
                param.Add("@OFFSET", _Requesst.PageIndex);
                param.Add("@FILTER", _Requesst.Filter);
                param.Add("@PerPageRecord", _Requesst.PageSize);
                param.Add("@userid", _Requesst.UserId);
                using (var conn = CreateConnection(_Requesst.ConnectionString))
                {
                    var query = Resources.GetAllOrphans;
                    orphanList = (await conn.QueryAsync<GetAllOrphansData>(query, param, commandType: CommandType.Text)).ToList();
                }
                return orphanList;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("GetAllImageFlyOut")]
        [HttpPost]
        public async Task<GetAllImageFlyOut_Response> GetAllImageFlyOut(string ConnectionString, string TableName, string TableId)
        {
            var model = new GetAllImageFlyOut_Response();
            try
            {
                var attachments = new List<GetAttchmentName>();
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    var tableId = TableId;
                    var oTable = await context.Tables.Where(x => x.TableName.Trim().ToLower().Equals(TableName.Trim().ToLower())).FirstOrDefaultAsync();
                    if (oTable == null)
                    {
                        model.IsTableEntityNull = true;
                        return model;
                    }

                    var idFieldName = oTable.IdFieldName;
                    var dbEntity = await context.Databases.Where(x => x.DBName.Trim().ToLower().Equals(oTable.DBName.Trim().ToLower())).FirstOrDefaultAsync();
                    if (dbEntity != null)
                    {
                        ConnectionString = _commonService.GetConnectionString(dbEntity, false);
                    }

                    if (!CommonFunctions.IdFieldIsString(TableName, idFieldName))
                        tableId = tableId.PadLeft(30, '0');


                    using (var conn = CreateConnection(ConnectionString))
                    {
                        var param = new DynamicParameters();
                        param.Add("@tableId", tableId);
                        param.Add("@tableName", TableName);
                        attachments = (await conn.QueryAsync<GetAttchmentName>("SP_RMS_GetAttchmentName", param, commandType: CommandType.StoredProcedure)).ToList();
                    }
                }
                model.IsTableEntityNull = false;
                model.GetAttchmentNames = attachments.OrderBy(a => a.AttachmentNumber).ToList();
                return model;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        [Route("CheckFilesize")]
        [HttpGet]
        public async Task<string> CheckFilesize(string ConnectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(ConnectionString))
                {
                    return (await context.Settings.Where(x => x.Section == "DragAndDropAttachment" && x.Item == "MaxSize").FirstOrDefaultAsync())?.ItemValue;
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        #region PrivateMethods

        private async Task<bool> CheckFilesize(string ConnectionString, string filesSizes)
        {
            using (var context = new TABFusionRMSContext(ConnectionString))
            {
                var expMaxSize = (await context.Settings.Where(x => x.Section == "DragAndDropAttachment" && x.Item == "MaxSize").FirstOrDefaultAsync()).ItemValue;
                var filearr = filesSizes.Split(",");
                for (int i = 0, loopTo = filearr.Count() - 1; i <= loopTo; i++)
                {
                    if (Math.Round(Convert.ToDecimal(expMaxSize)) < Math.Round(Convert.ToDecimal(Convert.ToInt64(filearr[i]) / 1024d / 1024d)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        #endregion
    }
}
