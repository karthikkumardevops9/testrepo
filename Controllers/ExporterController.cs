using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExporterController : ControllerBase
    {
        private IDbConnection CreateConnection(string connectionString)
        => new SqlConnection(connectionString);
        private readonly CommonControllersService<ExporterController> _commonService;
        private readonly IExporterService _exporterService;
        private readonly IBackgroundStatusService _backgroundStatusService;
        public ExporterController(CommonControllersService<ExporterController> commonService, IExporterService exporterService,IBackgroundStatusService backgroundStatusService)
        {
            _commonService = commonService;
            _exporterService = exporterService;
            _backgroundStatusService = backgroundStatusService;
        }

        [Route("DialogConfirmExportCSVorTXT")]
        [HttpPost]
        public async Task<Exporter> DialogConfirmExportCSVorTXT(DialogConfirmExportParam param)
        {
            var model = new Exporter();
            model.isRequireBtn = true;
            if (param.req.paramss.IsSelectAllData)
            {
                param.req.paramss.RecordCount = Query.TotalQueryRowCount(param.sql, param.passport.Connection());
            }
            try
            {
                if(param.passport.CheckPermission(param.req.paramss.viewName, Smead.Security.SecureObject.SecureObjectType.View, Permissions.Permission.Export))
                {
                    var type = await CheckIfBackgroungProcessing(param.req.paramss,param.passport.ConnectionString);
                    switch (type)
                    {
                        case var @case when @case == Enums.BackgroundTaskProcess.Normal:
                        {
                            model.lblTitle = param.req.paramss.IsCSV ? "Export Data into csv file format" : "Export Data into text file format";
                            model.HtmlMessage = string.Format("<p>" + "{1} will Export {0} rows." + "</p><p>{2}</p>", param.req.paramss.RecordCount, "TAB FusionRMS", "Do you wish to continue?");
                            model.ParamVal = param.req.paramss;
                            model.IsBackgroundProcessing = false;
                            break;
                        }
                        case var case1 when case1 == Enums.BackgroundTaskProcess.Background:
                        {
                            model.lblTitle = param.req.paramss.IsCSV ? "Export Data into csv file format" : "Export Data into text file format";
                            model.HtmlMessage = string.Format("<p>" + "{1} will Export {0} rows in background" + "</p><p>{2}</p>", param.req.paramss.RecordCount, "TAB FusionRMS", "Do you wish to continue?");
                            param.req.paramss.IsBackgroundProcessing = true;
                            model.IsBackgroundProcessing = param.req.paramss.IsBackgroundProcessing;
                            param.req.paramss.TaskType = 4;
                                 model.ParamVal = param.req.paramss;
                            break;
                        }
                        case var case2 when case2 == Enums.BackgroundTaskProcess.ExceedMaxLimit:
                        {
                            model.lblTitle = "Warning Message";
                            model.HtmlMessage = string.Format("<p>" + "You have exceeded max limit count. Please kindly contact your Administrator to change the Max Limit" + "</p>");
                            model.isRequireBtn = false;
                            break;
                        }
                        case var case3 when case3 == Enums.BackgroundTaskProcess.ServiceNotEnabled:
                        {
                            model.lblTitle = "Warning Message";
                            model.HtmlMessage = string.Format("<p>" + "Service Control Manager is not Enabled, Please kindly enable it from Slim Manager" + "</p>");
                            model.isRequireBtn = false;
                            break;
                        }
                    }
                }
                else
                {
                    model.lblTitle = "Export Data into csv file format";
                    model.HtmlMessage = "You have not been granted the permission to do export";
                }
            }
            catch(Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        [Route("BuildString")]
        [HttpPost]
        public string BuildString(BuildStringParam param)
        {
            var sb = new StringBuilder();
            try
            {
                sb = _exporterService.BuildString(param.passport, param.ParamViewId, param.Query, param.CurrentLevel, param.CultureShortPattern, param.OffSetVal);
                return sb.ToString();
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return sb.ToString();
        }

        [Route("DialogConfirmExportCSVorTXTReport")]
        [HttpPost]
        public async Task<Exporter> DialogConfirmExportCSVorTXTReport(DialogConfirmExportReportParam param)
        {
            var model = new Exporter();
            model.isRequireBtn = true;
            ExporterReportJsonModel @params = param.req.paramss;
            try
            {
                if (@params.viewId > 0)
                {
                    if (!param.passport.CheckPermission(@params.viewName, Smead.Security.SecureObject.SecureObjectType.Reports, Permissions.Permission.Export))
                    {
                        model.lblTitle = "Export Data into csv file format";
                        model.HtmlMessage = "You have not been granted the permission to do export";
                        model.Permission = false;
                        return model;
                    }
                    else if (@params.IsSelectAllData)
                    {
                        @params.RecordCount = await CountSelectAllData(@params.viewId, param.passport.ConnectionString);
                    }
                }
                else
                {
                    @params.RecordCount = @params.DataRows.Count;
                }

                Enums.BackgroundTaskProcess type;
                if (@params.ReportType == "19")
                {
                    type = await CheckIfBackgroungProcessing(@params, param.passport.ConnectionString);
                }
                else
                {
                    type = Enums.BackgroundTaskProcess.Normal;
                }

                switch (type)
                {
                    case var @case when @case == Enums.BackgroundTaskProcess.Normal:
                        {
                            model.lblTitle = @params.IsCSV ? "Export Data into csv file format" : "Export Data into text file format";
                            model.HtmlMessage = string.Format("<p>" + "{1} will Export {0} rows." + "</p><p>{2}</p>", @params.RecordCount, "TAB FusionRMS", "Do you wish to continue?");
                            model.ParamVal = @params;
                            model.IsBackgroundProcessing = false;
                            break;
                        }
                    case var case1 when case1 == Enums.BackgroundTaskProcess.Background:
                        {
                            model.lblTitle = @params.IsCSV ? "Export Data into csv file format" : "Export Data into text file format";
                            model.HtmlMessage = string.Format("<p>" + "{1} will Export {0} rows in background" + "</p><p>{2}</p>", @params.RecordCount, "TAB FusionRMS", "Do you wish to continue?");
                            @params.IsBackgroundProcessing = true;
                            model.IsBackgroundProcessing = @params.IsBackgroundProcessing;
                            @params.TaskType = 4;
                            model.ParamVal = @params;
                            break;
                        }
                    case var case2 when case2 == Enums.BackgroundTaskProcess.ExceedMaxLimit:
                        {
                            model.lblTitle = "Warning Message";
                            model.HtmlMessage = string.Format("<p>" + "You have exceeded max limit count. Please kindly contact your Administrator to change the Max Limit" + "</p>");
                            model.isRequireBtn = false;
                            break;
                        }
                    case var case3 when case3 == Enums.BackgroundTaskProcess.ServiceNotEnabled:
                        {
                            model.lblTitle = "Warning Message";
                            model.HtmlMessage = string.Format("<p>" + "Service Control Manager is not Enabled, Please kindly enable it from Slim Manager" + "</p>");
                            model.isRequireBtn = false;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }

            return model;
        }

        [Route("BackGroundProcessing")]
        [HttpPost]
        public async Task<Exporter> BackGroundProcessing(BackGroundProcessingParam param)
        {
            var model = new Exporter();
            try
            {
                var datapro = new DataProcessingModel();
                datapro.ErrorMessage = param.exporterData.ErrorMessage;
                datapro.viewId = param.exporterData.viewId;

                datapro.Reconciliation = param.exporterData.Reconciliation;
                datapro.RecordCount = param.exporterData.RecordCount;
                datapro.TaskType = param.exporterData.TaskType;
                datapro.IsSelectAllData = param.exporterData.IsSelectAllData;
                datapro.DueBackDate = null;
                datapro.DestinationTableName = "";
                datapro.DestinationTableId = "";
                datapro.ListofselectedIds = param.exporterData.ListofselectedIds;

                string extension = "txt";
                if (param.exporterData.IsCSV)
                    extension = "csv";

                string errorMessage = string.Empty;

                datapro.FileName = string.Format("{0}_Export_{1}_{2}", param.exporterData.tableName, extension.ToUpper(), Guid.NewGuid().ToString());
                param.exporterData.FileName = datapro.FileName;

                if (param.isDataProcessingNetworkPath)
                {
                    datapro.Path = string.Format("{0}/{1}.{2}", param.dataProcessingFilesPath, datapro.FileName, extension);
                    datapro.ExportReportPath = string.Format("{0}/{1}_Report.txt", param.dataProcessingFilesPath, datapro.FileName);
                }
                else
                {
                    datapro.Path = string.Format("{0}{1}.{2}", param.rootPath + "/BackgroundFiles/", datapro.FileName, extension);
                    datapro.ExportReportPath = string.Format("{0}{1}_Report.txt", param.rootPath + "/BackgroundFiles/", datapro.FileName);
                }

                bool output = await _backgroundStatusService.InsertData(datapro, param.rowQuery ?? "", param.passport);
                model = BackgroungMessage(output, param.exporterData);
            }
            catch (Exception ex) 
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return model;
        }

        [Route("BackgroundExportTask")]
        [HttpPost]
        public void BackgroundExportTask(BackgroundExportTask_Request _Request)
        {
            try
            {
                Export.ExportFiles(_Request.TaskId, _Request.Passport, _Request.IsCsv);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        [Route("SendEmail")]
        [HttpPost]
        public void SendEmail(SendEmail_Request _Request)
        {
            try
            {
                using (var conn = new SqlConnection(_Request.ConnectionString))
                {
                    Navigation.SendEmail(_Request.message, _Request.ToAddressList, _Request.FromAddress, _Request.Subject, _Request.AttachmentList, conn); 
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        #region Private Method
        private async Task<Enums.BackgroundTaskProcess> CheckIfBackgroungProcessing(ExporterJsonModel @params,string connectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    var s_MinVal = await context.Settings.FirstOrDefaultAsync(x => x.Section == "BackgroundExport" && x.Item == "MinValue");
                    var s_MaxVal = await context.Settings.FirstOrDefaultAsync(x => x.Section == "BackgroundExport" && x.Item == "MaxValue");
                    if (s_MinVal != null && @params.RecordCount < Convert.ToInt32(s_MinVal.ItemValue))
                    {
                        return Enums.BackgroundTaskProcess.Normal;
                    }
                    else if (s_MaxVal != null && @params.RecordCount > Convert.ToInt32(s_MaxVal.ItemValue))
                    {
                        return Enums.BackgroundTaskProcess.ExceedMaxLimit;
                    }
                    else
                    {
                        return Enums.BackgroundTaskProcess.Background;
                        //return Enums.BackgroundTaskProcess.ServiceNotEnabled;
                    }
                }
            }
            catch (Exception)
            {
                return Enums.BackgroundTaskProcess.Normal;
            }
        }

        private async Task<int> CountSelectAllData(int viewid,string connectionString)
        {
            int Rowscount = 0;
            using (var con = CreateConnection(connectionString))
            {
                string command = $"select count(*) from view__{viewid}";
                Rowscount = await con.ExecuteScalarAsync<int>(command);
            }
            return Rowscount;
        }

        private Exporter BackgroungMessage(bool output, ExporterJsonModel @params)
        {
            var msg = new StringBuilder();
            var model = new Exporter();
            if (output)
            {
                // If params.FileName.Contains("Transfer") Then
                // msg.Append(String.Format("<p>" + "Your Transfer added successfully in background. To show the report, Click on {0} link" + "</p>", "<b>" + "Background Status" + "</b>"))
                // Else
                // msg.Append(String.Format("<p>" + "Your Export added successfully in background. To show the report, Click on {0} link" + "</p>", "Background Status"))
                // End If
                msg.Append(string.Format("<p>" + "Your Export added successfully in background. To show the report, Click on {0} link" + "</p>", "Background Status"));
                msg.Append(string.Format("<p>" + "File Name: {0}" + "</p>", "<b>" + @params.FileName + "</b>"));
                msg.Append(string.Format("<p>" + "Selected Rows: {0}" + "</p>", "<b>" + @params.RecordCount.ToString() + "</b>"));
                msg.Append(string.Format("<p>" + "Current Status: {0}" + "</p>", "<b>" + Enum.GetName(typeof(Enums.BackgroundTaskStatus), 1) + "</b>"));
                model.HtmlMessage = msg.ToString();
                model.lblTitle = "Success Message";
            }
            else
            {
                model.lblTitle = "Error Message";
                msg.Append(string.Format("<p>{0}</p>", @params.ErrorMessage));
                model.HtmlMessage = msg.ToString();
            }
            return model;
        }

        #endregion
    }
}
