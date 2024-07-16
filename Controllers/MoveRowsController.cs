using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Models;
using MSRecordsEngine.Models.FusionModels;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services;
using Smead.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoveRowsController : ControllerBase
    {
        private IDbConnection CreateConnection(string connectionString)
        => new SqlConnection(connectionString);
        private readonly CommonControllersService<MoveRowsController> _commonService;

        public MoveRowsController(CommonControllersService<MoveRowsController> commonService)
        {
            _commonService = commonService;
        }

        [Route("GetMovePopup")]
        [HttpPost]
        public Moving GetMovePopup(MoveRowsCommonParam param) 
        {
            var model = new Moving();
            try
            {
                LoadDestinationTables(param.req.paramss, model,param.passport);
                // model.rbDestinationsItem = LoadDestinationItems(params.MoveRecords.MoveViewid)
                model.itemsTobeMove = LoadItemsTobeMove(param.req.paramss,param.passport);
                model.moveView = param.req.paramss.MoveRecords.MoveViewid.ToString();
            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        [Route("LoadDestinationItems")]
        [HttpPost]
        public Moving LoadDestinationItems(MoveRowsCommonParam param)
        {
            var model = new Moving();
            try
            {
                model.rbDestinationsItem = DestinationItemsList(param.req.paramss.MoveRecords.MoveViewid,param.passport);
            }
            catch (Exception ex) 
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        [Route("FilterItemsList")]
        [HttpPost]
        public MoveradioBoxModel FilterItemsList(MoveRowsCommonParam param)
        {
            var model = new MoveradioBoxModel();
            try
            {
                var qry = new Query(param.passport);
                var localparam = new Parameters(param.req.paramss.MoveRecords.MoveViewid, param.passport);
                localparam.QueryType = queryTypeEnum.LikeItemName;
                localparam.Paged = true;
                localparam.IsMVCCall = true;
                localparam.Text = param.req.paramss.MoveRecords.TextFilter;
                qry.FillData(localparam);
                var lst = new List<MoveradioBox>();
                foreach (DataRow row in localparam.Data.Rows)
                    lst.Add(new MoveradioBox() { text = row["ItemName"].ToString(), value = row["pkey"].ToString() });
                model.Data = lst;
            }
            catch (Exception ex)
            {
                model.isError = true;
                model.Msg = "Oops an error occurred.  Please contact your administrator.";
                _commonService.Logger.LogError($"Error:{ex.Message} Database: {param.passport.DatabaseName} CompanyName: {param.passport.License.CompanyName}");
            }
            return model;
        }

        [Route("BtnMoveItems")]
        [HttpPost]
        public async Task<Moving> BtnMoveItems(BtnMoveItemsParam param)
        {
            var model = new Moving();
            var @params = new Parameters(param.req.paramsUI.ViewId, param.passport);
            var parentParams = new Parameters(param.req.paramsUI.MoveRecords.MoveViewid, param.passport);
            try
            {
                using (var conn = param.passport.Connection())
                {
                    string currentValue;
                    foreach (string tableId in param.req.paramsUI.ids)
                    {
                        if (Navigation.IsArchivedOrDestroyed(param.req.paramsUI.TableName, tableId, Navigation.Enums.meFinalDispositionStatusType.fdstArchived | Navigation.Enums.meFinalDispositionStatusType.fdstDestroyed, conn))
                        {
                            throw new Exception("Destroyed records can't be edited or moved.");
                        }
                        string sql = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}] = @tableId", Navigation.MakeSimpleField(param.req.paramsUI.MoveRecords.fieldName), @params.TableName, Navigation.MakeSimpleField(@params.KeyField));

                        using (var cmd = CreateConnection(param.passport.ConnectionString))
                        {
                            try
                            {
                                currentValue = await cmd.ExecuteScalarAsync<string>(sql, new {tableId = tableId});
                            }
                            catch (Exception)
                            {
                                currentValue = string.Empty;
                            }
                        }

                        Navigation.UpdateSingleField(@params.TableName, tableId, param.req.paramsUI.MoveRecords.fieldName, param.req.paramsUI.MoveRecords.fieldItemValue, conn);

                        if (Navigation.CBoolean(@params.TableInfo["AuditUpdate"]))
                        {
                            {
                                var withBlock = AuditType.WebAccess;
                                withBlock.TableName = @params.TableName;
                                withBlock.TableId = tableId;
                                withBlock.ClientIpAddress = param.ClientIpAddress;
                                withBlock.ActionType = AuditType.WebAccessActionType.MoveRecord;
                                withBlock.AfterData = string.Format("{0}: {1}", Navigation.MakeSimpleField(param.req.paramsUI.MoveRecords.fieldName), param.req.paramsUI.MoveRecords.fieldItemValue);
                                if (!string.IsNullOrWhiteSpace(currentValue))
                                    withBlock.BeforeData = string.Format("{0}: {1}", Navigation.MakeSimpleField(param.req.paramsUI.MoveRecords.fieldName), currentValue);
                            }

                            string action = string.Format("Move Record: {0}: {1} from {2}: {3} to {2}: {4}", @params.TableInfo["UserName"].ToString(), tableId, parentParams.TableInfo["UserName"].ToString(), currentValue, param.req.paramsUI.MoveRecords.fieldItemValue);
                            Auditing.AuditUpdates(AuditType.WebAccess, action, param.passport);
                        }
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

        #region Private Methods

        private List<string> LoadItemsTobeMove(UserInterfaceJsonModel @params, Passport passport)
        {
            var lst = new List<string>();
            foreach (var id in @params.ids)
                lst.Add(Navigation.GetItemName(@params.TableName, id, passport));
            return lst;
        }

        private void LoadDestinationTables(UserInterfaceJsonModel @params, Moving model, Passport passport)
        {
            foreach (DataRow item in Navigation.GetUpperRelationships(@params.TableName, passport).Rows)
            {
                model.rbDestinationTable.Add(item[0].ToString());
                var lst = new List<MoveradioBox>();
                foreach (ViewItem View in Navigation.GetViewsByTableName(item[0].ToString(), passport))
                    lst.Add(new MoveradioBox() { text = View.ViewName, viewId = View.Id, value = item[2].ToString(), selected = false });
                model.rbDestinationViews.Add(lst);
            }
        }

        private List<MoveradioBox> DestinationItemsList(int Moveviewid,Passport passport)
        {
            var qry = new Query(passport);
            var @params = new Parameters(Moveviewid, passport);
            @params.QueryType = queryTypeEnum.OpenTable;
            @params.Paged = true;
            @params.IsMVCCall = true;
            qry.FillData(@params);
            var lst = new List<MoveradioBox>();
            foreach (DataRow row in @params.Data.Rows)
                lst.Add(new MoveradioBox() { text = row["ItemName"].ToString(), value = row["pkey"].ToString() });
            return lst;
        }

        private List<MoveradioBox> LoadAttachments(UserInterfaceJsonModel @params,Passport passport) //No References
        {
            var dt = Navigation.GetAttachments(@params.TableName, @params.ids[0].ToString(), passport);
            var lst = new List<MoveradioBox>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                    lst.Add(new MoveradioBox() { text = string.Format("Add Attachment {0}", item["AttachmentNumber"].ToString()), value = item["AttachmentNumber"].ToString(), selected = false });
            }
            return lst;
        }

        #endregion
    }
}
