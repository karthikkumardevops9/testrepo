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
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly CommonControllersService<TransferController> _commonService;
        private readonly ITrackingServices _trackingServices;
        private readonly IBackgroundStatusService _backgroundStatusService;
        private IDbConnection CreateConnection(string connectionString)
        => new SqlConnection(connectionString);
        public TransferController(CommonControllersService<TransferController> commonService, ITrackingServices trackingServices,IBackgroundStatusService backgroundStatusService)
        {
            _commonService = commonService;
            _trackingServices = trackingServices;
            _backgroundStatusService = backgroundStatusService;
        }

        [Route("StartTransfering")]
        [HttpPost]
        public async Task<TransferReturn> StartTransfering(TransferCommonParam param)
        {
            var res = new TransferReturn();
            var model = new Transfers();
            try
            {
                if (param.passport.CheckPermission(param.req.paramss.TableName, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                {
                    var type = await CheckIfBackgroundProcessing(param.req.paramss,param.passport.ConnectionString);
                    switch (type)
                    {
                        case Enums.BackgroundTaskProcess.Normal:
                            {
                                model.lblTitle = "Normal";
                                GeneratepopupView(model, param.req.paramss,param.passport);
                                break;
                            }
                        case Enums.BackgroundTaskProcess.Background:
                            {
                                model.lblTitle = "background";
                                res.IsBackground = true;
                                GeneratepopupView(model, param.req.paramss, param.passport);
                                break;
                            }
                        case Enums.BackgroundTaskProcess.ExceedMaxLimit:
                            {
                                model.lblTitle = "exceededmaxlimit";
                                break;
                            }
                        case Enums.BackgroundTaskProcess.ServiceNotEnabled:
                            {
                                model.lblTitle = "serviceisnotenable";
                                break;
                            }
                    }
                }
                res.Model = model;
                
            }
            catch (Exception ex) 
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return res;
        }

        [Route("GetTransferItems")]
        [HttpPost]
        public Transfers GetTransferItems(TransferCommonParam param)
        {
            var model = new Transfers();
            try
            {
                var pr = new Parameters(Convert.ToInt32(param.req.paramss.Transfer.ContainerViewid), param.passport);
                model.isDueBack = isDueback(pr,param.passport);
                model.trDestinationsItem = GetListofItems(pr, param.req.paramss.Transfer.TextFilter,param.passport);
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return model;
        }

        [Route("BtnTransfer")]
        [HttpPost]
        public async Task<Transfers> BtnTransfer(BtnTransferParam param)
        {
            if (!await checkIfContainerExist(param.req.paramss.Transfer.ContainerTableName, param.req.paramss.Transfer.ContainerItemValue, param.passport))
                return null;

            var model = new Transfers();
            var dateStr = new DateTime();
            model.isBackground = false;
            model.isWarning = false;
            model.userMsg = "Record/s has been Transferred successfully";
            var pr = new Parameters(Convert.ToInt32(param.req.paramss.Transfer.ContainerViewid), param.passport);
            try
            {
                if ( await _trackingServices.IsOutDestination(param.req.paramss.Transfer.ContainerTableName, param.req.paramss.Transfer.ContainerItemValue, param.passport.ConnectionString))
                {
                    if (isDueback(pr,param.passport))
                    {
                        if (string.IsNullOrEmpty(param.req.paramss.Transfer.TxtDueBack))
                        {
                            model.userMsg = "Due Back Date is required";
                            model.isWarning = true;
                            return model;
                        }

                        dateStr = DateTime.Parse(param.req.paramss.Transfer.TxtDueBack, CultureInfo.CurrentCulture);
                        if (DateTime.Parse(DateTime.Now.ToShortDateString()) > DateTime.Parse(dateStr.ToShortDateString()))
                        {
                            model.userMsg = "Object Due Back Date cannot be less than Today";
                            model.isWarning = true;
                            return model;
                        }
                    }
                }

                if (param.req.paramss.Transfer.IsSelectAllData)
                    param.req.paramss.ids = GetAllDataIds(param.req.paramss.ViewId,param.passport,param.HoldTotalRowQuery);
                if (param.IsBackground)
                    {
                        if (await TransferInBackground(param.req.paramss, model, param.passport,param.HoldTotalRowQuery,param.DataProcessingFilesPath,param.IsDataProcessingNetworkPath,param.RootPath))
                        {
                            model.isBackground = true;
                        }
                        else
                        {
                            model.isWarning = true;
                        }
                    }
                    else
                    {
                        await SubmitTransferdata(param.req.paramss.ids, param.req.paramss,param.passport);
                    }
            }
            catch(Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return model;
        }

        [Route("CountAllTransferData")]
        [HttpPost]
        public int CountAllTransferData(CountAllTransferParam param)
        {
            int TotalRows = 0;

            try
            {
                TotalRows = Query.TotalQueryRowCount(param.HoldTotalRowQuery,param.passport.Connection());
            }
            catch (Exception ex) {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
            return TotalRows;
        }

        [Route("BackgroundTransferTask")]
        [HttpPost]
        public void BackgroundTransferTask(BackgroundTransferTask_Request _Request)
        {
            try
            {
                Tracking.Transfer(_Request.TableName, _Request.TableId, _Request.DestinationTableName, _Request.DestinationTableId, _Request.DueBackDate, _Request.UserName, _Request.passport, _Request.passport.Connection(), null, null, true);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        private async Task SubmitTransferdata(List<string> idlst, UserInterfaceJsonModel @params,Passport passport)
        {
            foreach (string tableId in idlst)
            {
                if (string.IsNullOrEmpty(@params.Transfer.TxtDueBack))
                {
                    await  _trackingServices.PrepareDataForTransfer(@params.TableName, tableId, @params.Transfer.ContainerTableName, @params.Transfer.ContainerItemValue, default(DateTime), passport.UserName, passport);
                }
                else
                {
                   await _trackingServices.PrepareDataForTransfer(@params.TableName, tableId,
                        @params.Transfer.ContainerTableName,
                        @params.Transfer.ContainerItemValue, Convert.ToDateTime(@params.Transfer.TxtDueBack), passport.UserName, passport);
                }
            }
        }

        private void GeneratepopupView(Transfers model, UserInterfaceJsonModel @params,Passport passport)
        {
            this.GetContainerTypes(model, @params.ViewId,passport);

            if (@params.Transfer.IsSelectAllData)
            {
                model.lblTransfer = string.Format("Complete the below selections to Transfer All the '{0}' Records", @params.ViewName);
            }
            else
            {
                model.lblTransfer = string.Format("Complete the below selections to Transfer {0} selected {1}", @params.RecordCount, @params.ViewName);
                model.itemsTobeTransfer = LoadItemsTobeTransfer(@params,passport);
            }
        }

        private async Task<bool> TransferInBackground(UserInterfaceJsonModel @params, Transfers model, Passport passport,string HoldTotalRowQuery,string dataProcessingFilesPath, bool isDataProcessingNetworkPath,string RootPath)
        {
            var datapro = new DataProcessingModel();
            datapro.TaskType = (int)Enums.BackgroundTaskInDetail.Transfer;
            datapro.RecordCount = @params.RecordCount;
            datapro.IsSelectAllData = @params.Transfer.IsSelectAllData;
            datapro.ListofselectedIds = @params.ids;
            datapro.viewId = @params.ViewId;
            datapro.DueBackDate = @params.Transfer.TxtDueBack;
            datapro.ErrorMessage = string.Empty;
            datapro.DestinationTableName = @params.Transfer.ContainerTableName;
            datapro.DestinationTableId = @params.Transfer.ContainerItemValue;

            datapro.FileName = string.Format("{0}_Transfer_{1}", @params.TableName, Guid.NewGuid().ToString());

            if (isDataProcessingNetworkPath)
                datapro.Path = string.Format("{0}/{1}.txt", dataProcessingFilesPath, datapro.FileName);
            else
                datapro.Path = string.Format("{0}{1}.txt", (RootPath + "/BackgroundFiles/"), datapro.FileName);

            //datapro.Path = string.Format("{0}{1}.txt", (_webHostEnvironment.WebRootPath + "/BackgroundFiles/"), datapro.FileName);
            bool output = await _backgroundStatusService.InsertData(datapro, HoldTotalRowQuery, passport);
            if (!output)
            {
                model.userMsg = datapro.ErrorMessage;
            }
            else
            {
                model.userMsg = string.Format("{0} % {1} % {2} % {3}", "Your Transfer added successfully in background. To show the report, Click on {0} link", "Background Status", datapro.FileName, datapro.RecordCount);
            }

            return output;
        }

        private async Task<bool> checkIfContainerExist(string containerName, string tableid, Passport passport)
        {
            // check if container id exist if not (possible hacking...user change the id manualy)
            bool result = false;
            try
            {
                string pkey = Navigation.GetPrimaryKeyFieldName(containerName, passport);
                string command = string.Format("select count({0}) from {1} where {0} = '{2}'", pkey, containerName, tableid);
                using (var con = CreateConnection(passport.ConnectionString))
                {
                    result = await con.ExecuteScalarAsync<bool>(command);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            if (result == false)
            {
                Eventlogs.LogInfo("Fusion recognized security issues and didn't allow the user to continue the transaction");
            }
            return result;
        }

        private List<TransferRadioBox> GetListofItems(Parameters pr, string txtFilter,Passport passport)
        {
            var qry = new Query(passport);
            var lst = new List<TransferRadioBox>();
            pr.QueryType = queryTypeEnum.LikeItemName;
            pr.Paged = true;
            pr.IsMVCCall = true;
            if (!string.IsNullOrEmpty(txtFilter))
            {
                pr.Text = txtFilter;
            }
            qry.FillData(pr);

            foreach (DataRow item in pr.Data.Rows)
            {
                string id = Navigation.PrepPad(pr.TableName, Convert.ToString(item["pkey"]), passport);
                lst.Add(new TransferRadioBox() { text = Convert.ToString(item["ItemName"]), ContainerTableName = pr.TableName, value = id });
            }
            return lst;
        }

        private void GetContainerTypes(Transfers model, int viewid,Passport passport)
        {
            var pr = new Parameters(viewid, passport);
            int trackLevel = Convert.ToInt32(pr.TableInfo["TrackingTable"]);
            if (trackLevel == 0)
                trackLevel = int.MaxValue;
            foreach (var item in Tracking.GetTrackableContainerTypes(passport))
            {
                if (passport.CheckPermission(item.Type, Smead.Security.SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                {
                    if (item.Level < trackLevel)
                    {
                        model.lsTaransferType.Add(item.Type.ToString());
                        var lst = new List<TransferRadioBox>();
                        foreach (var v in Navigation.GetViewsByTableName(item.Type, passport))
                            lst.Add(new TransferRadioBox() { text = v.ViewName, ContainerTableName = item.Type, ContainerViewid = v.Id });
                        model.lsViews.Add(lst);
                    }
                }
            }
        }
        
        private async Task<Enums.BackgroundTaskProcess> CheckIfBackgroundProcessing(UserInterfaceJsonModel @params,string connectionString)
        {
            try
            {
                using (var context = new TABFusionRMSContext(connectionString))
                {
                    string expMinVal = await context.Settings.Where(x => x.Section == "BackgroundTransfer" & x.Item == "MinValue").Select(x=>x.ItemValue).FirstOrDefaultAsync();
                    string expMaxVal = await context.Settings.Where(x => x.Section == "BackgroundTransfer" & x.Item == "MaxValue").Select(x => x.ItemValue).FirstOrDefaultAsync();

                    if ((double)@params.RecordCount < Convert.ToDouble(expMinVal))
                    {
                        return Enums.BackgroundTaskProcess.Normal;
                    }
                    else if ((double)@params.RecordCount > Convert.ToDouble(expMinVal))
                    {
                        return Enums.BackgroundTaskProcess.ExceedMaxLimit;
                    }
                    //else if (Convert.ToBoolean(ContextService.GetSessionValueByKey("ServiceManagerEnabled", httpContext)))
                    //{
                    //    return Enums.BackgroundTaskProcess.Background;
                    //}
                    else
                    {
                        return Enums.BackgroundTaskProcess.Background;
                        //return Enums.BackgroundTaskProcess.ServiceNotEnabled;
                    }
                }
            }
            catch(Exception ex)
            {
                return Enums.BackgroundTaskProcess.Normal;
            }
        }

        private bool isDueback(Parameters pr,Passport passport)
        {
            bool _dueDateOn = Navigation.CBoolean(Navigation.GetSystemSetting("DateDueOn", passport));
            bool rtn = true;
            if (!_dueDateOn || pr.TableInfo["TrackingDueBackDaysFieldName"] is DBNull)
                rtn = false;
            return rtn;
        }

        private List<string> LoadItemsTobeTransfer(UserInterfaceJsonModel @params,Passport passport)
        {
            var lst = new List<string>();
            foreach (var id in @params.ids)
                lst.Add(Navigation.GetItemName(@params.TableName, id, passport));
            return lst;
        }

        private List<string> GetAllDataIds(int viewid,Passport passport,string HoldTotalRowQuery)
        {
            var pr = new Parameters(viewid, passport);
            var lst = new List<string>();
            string query = HoldTotalRowQuery;
            int fromIndex = query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            string rightPart = query.Substring(fromIndex);
            string command = string.Format("SELECT [{0}] {1}", pr.KeyField, rightPart);
            var dt = new DataTable();

            if (passport != null)
            {
                using (var cmd = new SqlCommand(command, passport.Connection()))
                {
                    var adp = new SqlDataAdapter();
                    adp.SelectCommand = cmd;
                    adp.Fill(dt);
                }
            }

            foreach (DataRow row in dt.Rows)
                lst.Add(row[pr.KeyField].ToString());

            return lst;
        }

    }
}
