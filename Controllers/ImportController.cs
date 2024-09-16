using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSRecordsEngine.Models;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MSRecordsEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {

        private readonly CommonControllersService<ImportController> _commonService;

        public ImportController(CommonControllersService<ImportController> commonControllersService)
        {
            _commonService = commonControllersService;
        }


        [Route("RunScriptBeforeAdd")]
        [HttpPost]
        public async Task<bool> RunScriptBeforeAdd(RunScriptBeforeAddParam runScriptBeforeAddParam)
        {
            bool success = false;
            try
            {
                success = (ScriptEngine.RunScriptBeforeAdd(runScriptBeforeAddParam.TableName, runScriptBeforeAddParam.Passport)).Successful;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
            }
            return success;
        }


        [Route("RunScriptBeforeEdit")]
        [HttpPost]
        public async Task<bool> RunScriptBeforeEdit(RunScriptBeforeEditParam runScriptBeforeEditParam)
        {
            bool success = false;
            try
            {
                success = (ScriptEngine.RunScriptBeforeEdit(runScriptBeforeEditParam.TableName, runScriptBeforeEditParam.FieldIndex, runScriptBeforeEditParam.Passport)).Successful;
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw;
            }
            return success;
        }


        [Route("RunScriptAfterAdd")]
        [HttpPost]
        public async Task RunScriptAfterAdd(RunScriptAfterAddEditParam runScriptAfterAddEditParam)
        {
            try
            {
                ScriptEngine.RunScriptAfterAdd(runScriptAfterAddEditParam.TableName, runScriptAfterAddEditParam.TableId, runScriptAfterAddEditParam.Passport);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw;
            }
        }


        [Route("RunScriptAfterEdit")]
        [HttpPost]
        public async Task RunScriptAfterEdit(RunScriptAfterAddEditParam runScriptAfterAddEditParam)
        {
            try
            {
                ScriptEngine.RunScriptAfterEdit(runScriptAfterAddEditParam.TableName, runScriptAfterAddEditParam.TableId, runScriptAfterAddEditParam.Passport);
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw;
            }
        }


        [Route("DoTransfer")]
        [HttpPost]
        public async Task DoTransfer(DoTransferParam doTransferParam)
        {
            try
            {
               await Tracking.TransferAsync(
                    doTransferParam.TableName,
                    doTransferParam.TableId,
                    doTransferParam.DestinationTableName,
                    doTransferParam.DestinationTableId,
                    doTransferParam.DueDate,
                    doTransferParam.UserName,
                    doTransferParam.passport,
                    doTransferParam.TrackingAdditionalField1,
                    doTransferParam.TrackingAdditionalField2,
                    doTransferParam.TransactionDateTime);

                using (var conn = new SqlConnection())
                {
                    Navigation.UpdateSingleField("", "", "", "?", conn);
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw;
            }
        }


        [Route("RemoveRecordForInactiveEligibleList")]
        [HttpPost]
        public async Task RemoveRecordForInactiveEligibleList(RemoveRecordForInactiveEligibleListParam removeRecordForInactiveEligibleListParam)
        {
            try
            {
                var SQL = removeRecordForInactiveEligibleListParam.SQL;
                var table = removeRecordForInactiveEligibleListParam.table;
                var ConnectionString = removeRecordForInactiveEligibleListParam.ConnectionString;
                var rowId = removeRecordForInactiveEligibleListParam.rowId;
                var fieldOpenDate = removeRecordForInactiveEligibleListParam.fieldOpenDate;
                var fieldCreateDate = removeRecordForInactiveEligibleListParam.fieldCreateDate;
                var fieldCloseDate = removeRecordForInactiveEligibleListParam.fieldCloseDate;
                var fieldOtherDate = removeRecordForInactiveEligibleListParam.fieldOtherDate;

                using (var conn = new SqlConnection(ConnectionString))
                {
                    using (var cmd = new SqlCommand(SQL, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                string fieldRetentionCode = table.RetentionFieldName;

                                foreach (DataRow row in dt.Rows)
                                {
                                    DateTime dDestructionDate = DateTime.MinValue;

                                    string eventType = row["RetentionEventType"].ToString();

                                    switch (eventType)
                                    {
                                        case "Date Opened":
                                            if (!string.IsNullOrEmpty(table.RetentionDateOpenedField))
                                                dDestructionDate = Convert.ToDateTime(row[fieldOpenDate]);
                                            break;

                                        case "Date Closed":
                                            if (!string.IsNullOrEmpty(table.RetentionDateClosedField))
                                                dDestructionDate = Convert.ToDateTime(row[fieldCloseDate]);
                                            break;

                                        case "Date Created":
                                            if (!string.IsNullOrEmpty(table.RetentionDateCreateField))
                                                dDestructionDate = Convert.ToDateTime(row[fieldCreateDate]);
                                            break;

                                        case "Date Other":
                                            if (!string.IsNullOrEmpty(table.RetentionDateOtherField))
                                                dDestructionDate = Convert.ToDateTime(row[fieldOtherDate]);
                                            break;
                                    }

                                    DateTime inactiveDate = dDestructionDate.AddYears(Convert.ToInt32(row["InactivityPeriod"].ToString()));
                                    if (inactiveDate.CompareTo(DateTime.Now) > 0)
                                    {
                                        string deleteRecord = "DELETE FROM SLDestructCertItems WHERE TableId = '" + rowId + "'";
                                        using (var del = new SqlCommand(deleteRecord, conn))
                                        {
                                            del.ExecuteNonQuery();
                                        }
                                    }

                                    string inactivityFlag = inactiveDate.CompareTo(DateTime.Now) < 0 ? "1" : "0";
                                    string id = rowId.ToString();

                                    if (!string.IsNullOrEmpty(id))
                                        Navigation.UpdateSingleField(table.TableName, id, "%slRetentionInactive", inactivityFlag, conn);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _commonService.Logger.LogError($"Error:{ex.Message}");
                throw;
            }
        }
    }
}
