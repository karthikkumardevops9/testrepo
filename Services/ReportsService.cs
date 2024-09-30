using Microsoft.VisualBasic;
using MSRecordsEngine.Models;
using MSRecordsEngine.RecordsManager;
using MSRecordsEngine.Services.Interface;
using Smead.Security;
using System.Data;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Reflection;
using System.Collections;
using static MSRecordsEngine.Models.Enums;
using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using static MSRecordsEngine.RecordsManager.Retention;
using System.Runtime.InteropServices;
using MSRecordsEngine.Entities;
using SecureObject = Smead.Security.SecureObject;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using static MSRecordsEngine.Models.AuditReportSearch;
using Microsoft.AspNetCore.Routing.Constraints;
using Leadtools.Barcode;

namespace MSRecordsEngine.Services
{
    public class ReportsService : IReportsService
    {
        public async Task<ReportingMenu> ReportingMenu(UserInterfaceProps props)
        {
            var model = new ReportingMenu();
            var sbMenu = new StringBuilder();
            if (CheckPermssionsReportTab(props))
            {
                sbMenu.Append("<ul class='drillDownMenu'><li>");
                sbMenu.Append(string.Format("<a href='#'><i class='font_icon theme_color fa fa-database'></i>{0}</a>", "Reports"));
                sbMenu.Append("<ul>");
                await CreateHtml(sbMenu, props, model);
                sbMenu.Append("</ul>");
                sbMenu.Append("</li></ul>");
            }
            model.dateFormat = props.DateFormat.Trim().ToUpper();
            return model;
        }
        public async Task<AuditReportSearch> RunAuditSearch(UIproperties paramss, bool ispaging)
        {
            var model = new AuditReportSearch();
            DateTime startDate = CommonFunctions.ConvertStringToCulture(paramss.StartDate, paramss.DateFormat);
            DateTime endDate = CommonFunctions.ConvertStringToCulture(paramss.EndDate, paramss.DateFormat);

            paramss.StartDate = CommonFunctions.ConvertStringToSqlCulture(paramss.StartDate, paramss.DateFormat);
            paramss.EndDate = CommonFunctions.ConvertStringToSqlCulture(paramss.EndDate, paramss.DateFormat);

            DateTime currentDate = DateTime.Now;

            if (startDate > currentDate || endDate > currentDate)
            {
                string errorMessage = "Future dates are not allowed.";
                throw new Exception(errorMessage);
            }
            if (endDate < startDate)
            {
                string errorMessage = "From Date must be less than To Date.";
                throw new Exception(errorMessage);
            }

            await AuditQuery(paramss, ispaging, model);
            return model;
        }
        public async Task<ReportsModels> InitiateReports(ReportingJsonModelReq req)
        {
            var model = new ReportsModels();
            string TodayDate = DateTime.Now.Date.ToString("MM/dd/yyyy").Split(' ')[0];
            model.dateFromTxt = DateTime.ParseExact(TodayDate, "MM/dd/yyyy", model.CultureInfo);

            switch ((ReportsType)req.paramss.reportType)
            {
                case ReportsType.PastDueTrackableItemsReport:
                    {
                        await PastDueTrackableItemsReport(model, req);
                        break;
                    }
                case ReportsType.ObjectOut:
                    {
                        await ObjectsOut(model, req);
                        break;
                    }
                case ReportsType.ObjectsInventory:
                    {
                        await ObjectsInventory(model, req);
                        break;
                    }
                case ReportsType.RequestNew:
                    {
                        await Requestnew(model, req);
                        break;
                    }
                case ReportsType.RequestNewBatch:
                    {
                        await RequestNewBatch(model, req);
                        break;
                    }
                case ReportsType.RequestPullList:
                    {
                        await PullList(model, req);
                        break;
                    }
                case ReportsType.RequestException:
                    {
                        await RequestExceptions(model, req);
                        break;
                    }
                case ReportsType.RequestInProcess:
                    {
                        await InProcess(model, req);
                        break;
                    }
                case ReportsType.RequestWaitList:
                    {
                        await WaitList(model, req);
                        break;
                    }
            }
            return model;
        }
        public async Task<ReportsModels> InitiateReportsPagination(ReportingJsonModelReq req)
        {
            var model = new ReportsModels();
            string TodayDate = DateTime.Now.Date.ToString().Split(' ')[0];
            //model.dateFromTxt = DateTime.Parse(TodayDate, model.CultureInfo);
            //model.dateFromTxt = Conversions.ToDate(model.dateFromTxt.ToString().Split(' ')[0]);
            switch ((ReportsType)req.paramss.reportType)
            {
                case ReportsType.PastDueTrackableItemsReport:
                    {
                        await PastDueTrackableItemsReport_QueryCount(model, req);
                        break;
                    }
                case ReportsType.ObjectOut:
                    {
                        ObjectsOut_QueryCount(model, req);
                        break;
                    }
                case ReportsType.ObjectsInventory:
                    {
                        await ObjectsInventory_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RequestNew:
                    {
                        await Requestnew_QueryCount(model, req);
                        break;
                    }
                case ReportsType.RequestNewBatch:
                    {
                        await RequestNewBatch_QueryCount(model, req);
                        break;
                    }
                case ReportsType.RequestPullList:
                    {
                        await PullList_QueryCount(model, req);
                        break;
                    }
                case ReportsType.RequestException:
                    {
                        await RequestExceptions_Count(model, req);
                        break;
                    }
                case ReportsType.RequestInProcess:
                    {
                        await InProcess_QueryCount(model, req);
                        break;
                    }
                case ReportsType.RequestWaitList:
                    {
                        await WaitList_QueryCount(model, req);
                        break;
                    }
            }
            return model;
        }
        public async Task<RetentionReportModel> InitiateRetentionReport(ReportingJsonModelReq req)
        {
            var model = new RetentionReportModel();
            if (!req.passport.CheckPermission(" Retention", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the retention codes report";
                return model;
            }
            switch ((ReportsType)req.paramss.reportType)
            {
                case ReportsType.RetentionFinalDisposition:
                    {
                        await RetentionFinalDisposition(model, req);
                        break;
                    }
                case ReportsType.RetentionCertifieDisposition:
                    {
                        await RetentionCertifieDisposition(model, req);
                        break;
                    }
                case ReportsType.RetentionInactivePullList:
                    {
                        await InactivePullList(model, req);
                        break;
                    }
                case ReportsType.RetentionInactiveRecords:
                    {
                        await RetentionInactiveRecords(model, req);
                        break;
                    }
                case ReportsType.RetentionRecordsOnHold:
                    {
                        await RetentionRecordsOnHold(model, req);
                        break;
                    }
                case ReportsType.RetentionCitations:
                    {
                        await RetentionCitations(model, req);
                        break;
                    }
                case ReportsType.RetentionCitationsWithRetCodes:
                    {
                        await RetentionCitationsWithRetCodes(model, req);
                        break;
                    }
                case ReportsType.RetentionCodes:
                    {
                        await RetentionCodes(model, req);
                        break;
                    }
                case ReportsType.RetentionCodesWithCitations:
                    {
                        await RetentionCodesWithCitations(model, req);
                        break;
                    }
            }
            return model;

        }
        public async Task<RetentionReportModel> InitiateRetentionReportPagination(ReportingJsonModelReq req)
        {
            var model = new RetentionReportModel();
            if (!req.passport.CheckPermission(" Retention", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the retention codes report";
                return model;
            }

            switch ((ReportsType)req.paramss.reportType)
            {
                case ReportsType.RetentionFinalDisposition:
                    {
                        await RetentionFinalDisposition_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RetentionCertifieDisposition:
                    {
                        await RetentionCertifieDisposition_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RetentionInactivePullList:
                    {
                        await InactivePullList_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RetentionInactiveRecords:
                    {
                        await RetentionInactiveRecords_QueryTableCount(model, req);
                        break;
                    }
                // Case ReportsType.RetentionRecordsOnHold
                // RetentionRecordsOnHold()
                case ReportsType.RetentionCitations:
                    {
                        await RetentionCitations_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RetentionCitationsWithRetCodes:
                    {
                        await RetentionCitationsWithRetCodes_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RetentionCodes:
                    {
                        await RetentionCodes_QueryTableCount(model, req);
                        break;
                    }
                case ReportsType.RetentionCodesWithCitations:
                    {
                        await RetentionCodesWithCitations_QueryTableCount(model, req);
                        break;
                    }
            }
            return model;

        }
        public async Task<ReportsModels> BtnSendRequestToThePullList(ReportingJsonModelReq req)
        {
            var model = new ReportsModels();
            if (!req.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the new request report";
                return model;
            }
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("TabFusionRMS.WebCS.Resource", Assembly.GetExecutingAssembly());

            int pullListId;
            string sql = rm.GetString("InsertSLPullList") == null ? "" : rm.GetString("InsertSLPullList");
            string status = "new";
            if (req.paramss.isBatchRequest)
            {
                sql = rm.GetString("UpdateBatchSLPullList") == null ? "" : "";
            }
            using (var conn = new SqlConnection(req.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (req.paramss.isBatchRequest)
                    {
                        status = "New Batch";
                        cmd.Parameters.AddWithValue("@pullListId", req.paramss.id);
                        cmd.ExecuteScalar();
                        pullListId = Convert.ToInt32(req.paramss.id);
                    }
                    else
                    {
                        var user = new User(req.passport, true);
                        cmd.Parameters.AddWithValue("@userName", user.UserName);
                        cmd.Parameters.AddWithValue("@batchRequest", req.paramss.isBatchRequest);
                        var res = cmd.ExecuteScalar();
                        pullListId = Convert.ToInt32(res);
                    }
                }

                if (pullListId == 0)
                {
                    pullListId = Convert.ToInt32(false);
                }

                using (var cmd = new SqlCommand(rm.GetString("UpdateSLRequestor"), conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", string.Empty);
                    cmd.Parameters.AddWithValue("@tableID", string.Empty);
                    cmd.Parameters.AddWithValue("@pullListId", pullListId);
                    cmd.Parameters.AddWithValue("@status", status);

                    foreach (Items row in req.paramss.ListofPullItem)
                    {
                        cmd.Parameters["@tableName"].Value = row.tableName;
                        cmd.Parameters["@tableID"].Value = row.tableid;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return model;
        }

        public async Task<ReportCommonModel> BtnSubmitDisposition(ReportingJsonModelReq req)
        {
            var model = new ReportCommonModel();
            if (req.paramss.submitType != null)
            {
                var location = string.Empty;
                var locationId = string.Empty;

                FinalDisposition dispositionType = FinalDisposition.None;
                switch (req.paramss.submitType)
                {
                    case "destruction":
                        {
                            dispositionType = FinalDisposition.Destruction;
                            break;
                        }

                    case "archive":
                        {
                            location = Tracking.GetLocationsTableName(req.passport);
                            locationId = req.paramss.locationId;
                            dispositionType = FinalDisposition.PermanentArchive;
                            break;
                        }

                    case "purge":
                        {
                            dispositionType = FinalDisposition.Purge;
                            break;
                        }
                }
                if (dispositionType != FinalDisposition.None)
                {
                    DateTime dispositionDate = DateTime.Parse(req.paramss.udate);
                    if (ApplyDispositionToList((List<string>)req.paramss.ids, dispositionType, "", dispositionDate, req.passport, location, locationId))
                        await ApproveDestructionAsync(req.paramss.reportId.ToString(), dispositionDate, req.passport);
                }

            }
            else
                model.Msg = "error violation";

            return model;
        }
        public async Task<ReportCommonModel> BtnSubmitInactive(ReportingJsonModelReq param)
        {
            var model = new ReportCommonModel();
            var location = await Tracking.GetLocationsTableNameAsync(param.passport.ConnectionString);
            Retention.SetListInactive(param.paramss.ids, location, param.paramss.ddlSelected, param.passport);
            return model;
        }
        public async Task<int> CreateEligibleRecordsForReport(UserInterfaceProps props, FinalDisposition iCurrDispositionType)
        {
            bool bFoundOne;
            DateTime dDestructionDate;
            DateTime dScheduledDate;
            string sTableIdField;
            string sSQL;
            string sTrackedTableId;
            int destructionCertId = 0;
            // Hold on to your butts! This is going to be a bumpy ride.
            bFoundOne = false;
            // First delete an orphaned SLDestructCertItems records that have elapsed past their hold date
            sSQL = "DELETE FROM [SLDestructCertItems] WHERE [SnoozeUntil] IS NOT NULL AND [SnoozeUntil] < @snoozeUntil";

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@snoozeUntil", DateTime.Today.AddDays(1d));
                    cmd.ExecuteNonQuery();
                }

                var allTables = Navigation.GetAllTables(props.passport);

                foreach (RecordsManage.TablesRow oTables in allTables)
                {
                    if (Convert.ToBoolean(oTables.RetentionPeriodActive) & oTables.RetentionFinalDisposition == (int)iCurrDispositionType)
                    {
                        string fieldOpenDate = string.Empty;
                        string fieldCreateDate = string.Empty;
                        string fieldCloseDate = string.Empty;
                        string fieldOtherDate = string.Empty;

                        sTableIdField = Strings.Trim(oTables.TableName) + "_" + Strings.Trim(Navigation.MakeSimpleField(oTables.IdFieldName));
                        sSQL = "SELECT [" + oTables.TableName + "].[" + Navigation.MakeSimpleField(oTables.IdFieldName) + "] AS " + sTableIdField + ", ";
                        sSQL += " [" + oTables.TableName + "].[" + Navigation.MakeSimpleField(oTables.RetentionFieldName) + "], ";
                        if (oTables.RetentionDateCreateField.Length > 0)
                        {
                            fieldCreateDate = Navigation.MakeSimpleField(oTables.RetentionDateCreateField);
                            sSQL += " [" + oTables.TableName + "].[" + fieldCreateDate + "], ";
                        }
                        if (oTables.RetentionDateClosedField.Length > 0)
                        {
                            fieldCloseDate = Navigation.MakeSimpleField(oTables.RetentionDateClosedField);
                            sSQL += " [" + oTables.TableName + "].[" + fieldCloseDate + "], ";
                        }
                        if (oTables.RetentionDateOpenedField.Length > 0)
                        {
                            fieldOpenDate = Navigation.MakeSimpleField(oTables.RetentionDateOpenedField);
                            sSQL += " [" + oTables.TableName + "].[" + fieldOpenDate + "], ";
                        }
                        if (oTables.RetentionDateOtherField.Length > 0)
                        {
                            fieldOtherDate = Navigation.MakeSimpleField(oTables.RetentionDateOtherField);
                            sSQL += " [" + oTables.TableName + "].[" + fieldOtherDate + "], ";
                        }

                        bool idFieldIsString = Navigation.FieldIsAString(oTables.TableName, props.passport);
                        string sFormattedIdFieldName = "([" + oTables.TableName + "].[" + Navigation.MakeSimpleField(oTables.IdFieldName) + "] = [SLDestructCertItems].[TableId] AND " + "[SLDestructCertItems].[TableName] = '" + oTables.TableName + "') ";


                        sSQL += " [SLRetentionCodes].* FROM (([" + oTables.TableName + "] INNER JOIN [SLRetentionCodes] ON [" + oTables.TableName + "].[" + Navigation.MakeSimpleField(oTables.RetentionFieldName) + "] = SLRetentionCodes.Id)) LEFT JOIN " + "[SLDestructCertItems] ON " + sFormattedIdFieldName + " WHERE (([SLRetentionCodes].[RetentionLegalHold] = 0) OR ([SLRetentionCodes].[RetentionLegalHold] IS NULL)) " + "   AND (([SLDestructCertItems].[LegalHold] = 0) OR ([SLDestructCertItems].[LegalHold] IS NULL)) " + "   AND (([SLDestructCertItems].[RetentionHold] = 0) OR ([SLDestructCertItems].[RetentionHold] IS NULL)) " + "   AND ([SLDestructCertItems].[SLDestructionCertsId] IS NULL)";

                        using (var cmd = new SqlCommand(sSQL, conn))
                        {
                            using (var da = new SqlDataAdapter(cmd))
                            {
                                var dt = new DataTable();
                                da.Fill(dt);

                                if (dt.Rows.Count > 0)
                                {
                                    // get a Field Object to speed things up
                                    var trackingTables = Navigation.GetTrackingTables(props.passport);
                                    string fieldRetentionCode = oTables.RetentionFieldName;
                                    var tableInfo = Navigation.GetTableInfo(oTables.TableName, props.passport);

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        dDestructionDate = DateTime.MinValue;
                                        try
                                        {
                                            if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Opened", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateOpenedField.Length > 0)
                                                    dDestructionDate = Convert.ToDateTime(row[fieldOpenDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Closed", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateClosedField.Length > 0)
                                                    dDestructionDate = Convert.ToDateTime(row[fieldCloseDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Created", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateCreateField.Length > 0)
                                                    dDestructionDate = Convert.ToDateTime(row[fieldCreateDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Other", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateOtherField.Length > 0)
                                                    dDestructionDate = Convert.ToDateTime(row[fieldOtherDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Last Tracked", Constants.vbTextCompare) == 0)
                                            {
                                                if (idFieldIsString)
                                                {
                                                    sTrackedTableId = row[Navigation.MakeSimpleField(oTables.IdFieldName)].ToString();
                                                }
                                                else
                                                {
                                                    sTrackedTableId = Navigation.PrepPad(row[Navigation.MakeSimpleField(oTables.IdFieldName)].ToString());
                                                }

                                                dDestructionDate = Retention.GetDestructionDate(oTables.TableName, sTrackedTableId, trackingTables, props.passport);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.Print(ex.Message);
                                        }

                                        if (dDestructionDate > DateTime.MinValue)
                                        {
                                            dScheduledDate = Navigation.ApplyYearEndToDate(dDestructionDate, Convert.ToDouble(row["RetentionPeriodTotal"]), Navigation.CBoolean(row["RetentionPeriodForceToEndOfYear"]), props.passport);

                                            if (dScheduledDate <= DateTime.Now)
                                            {
                                                // Record has exceeded it's eligible Date; Add to list
                                                if (!bFoundOne)
                                                {
                                                    // create cert if it does not exist
                                                    destructionCertId = CreateDestructionCert(new User(props.passport, true).UserName, DateTime.Now, "", "", "", "", "", (int)iCurrDispositionType, props.passport);
                                                    bFoundOne = true;
                                                }

                                                string argreturnInactivityEventType = "";
                                                var inactDate = Retention.CalcRetentionInactiveDate(tableInfo, row, row[fieldRetentionCode].ToString(), props.passport, returnInactivityEventType: ref argreturnInactivityEventType);
                                                Retention.SaveDestructionCertItem(oTables.TableName, row[sTableIdField].ToString(), 0, destructionCertId, "", row[oTables.RetentionFieldName].ToString(), false, "", RetentionHoldTypes.None, default(DateTime), dScheduledDate, inactDate, true, conn);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return destructionCertId;
        }
        public async Task<AuditReportSearch> GetauditReportView(UserInterfaceProps props)
        {
            var model = new AuditReportSearch();
            await BindUserDDL(props, model);
            await BindTableDDL(props, model);
            model.dateFormat = props.DateFormat.ToString().Trim().ToUpper();
            return model;
        }
        public async Task<RetentionButtons> GetInactivePopup(UserInterfaceProps props)
        {
            var model = new RetentionButtons();
            var user = new User(props.passport, true);
            model.TodayDate = DateTime.Now.ToString("yyyy-MM-dd");
            model.username = user.UserName;
            var locationList = await Tracking.GetInactiveLocationsAsync(props.passport);
            if (locationList.Count > 0)
            {
                foreach (var location in locationList)
                    model.ddlSelection.Add(new DDLItems() { text = location.Value, value = location.Key });
            }
            return model;
        }

        public async Task<RetentionButtons> GetSubmitForm(UserInterfaceProps props)
        {
            string submitType = props.StringHolder;
            var model = new RetentionButtons();
            var user = new User(props.passport, true);
            model.TodayDate = DateTime.Now.ToString("yyyy-MM-dd");
            model.username = user.UserName;
            if (submitType == "archived")
            {
                var locationList = await Tracking.GetArchiveLocationsAsync(props.passport);
                model.btnSubmitText = "Archive";
                model.btnSetSubmitType = "archive";
                if (locationList.Count > 0)
                {
                    foreach (var item in locationList)
                        model.ddlSelection.Add(new DDLItems() { value = item.Key, text = item.Value });
                }
            }
            if (submitType == "destruction")
            {
                model.btnSubmitText = "Destruction";
                model.btnSetSubmitType = "destruction";
            }
            if (submitType == "purge")
            {
                model.btnSubmitText = "Purge";
                model.btnSetSubmitType = "purge";
            }
            return model;
        }

        private async Task BindTableDDL(UserInterfaceProps props, AuditReportSearch model)
        {
            string sSQL;
            var dsObject = new DataTable();
            //sSQL = "select UserName,TableName +'|'+cast((select COUNT(*) from [dbo].[FNGetChildTables](TableName))AS varchar(20)) as ObjectValue from Tables where TableName = 'true' or AuditUpdate = 'true' or AuditConfidentialData = 'true'";
            sSQL = "select t.UserName, t.TableName + '|' + cast((select COUNT(*) from [dbo].[FNGetChildTables](t.TableName)) AS varchar(20)) as ObjectValue from Tables t inner join (select distinct SO.Name as TableName from SecureUserGroup as SUG inner join SecureObjectPermission as SOP on SUG.GroupID = SOP.GroupID inner join SecureObject as SO on SOP.SecureObjectID = SO.SecureObjectID where SUG.UserID = @UserId and SO.BaseID = 2) as virtualtable on virtualtable.TableName = t.TableName where t.TableName = 'true' or t.AuditUpdate = 'true' or t.AuditConfidentialData = 'true'";
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", props.passport.UserId);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsObject);
                    }
                }
            }
            model.objectDDL.Add(new DDLprops() { text = "Select Table", valuetxt = "-1" });
            model.objectDDL.Add(new DDLprops() { text = "All Tables", valuetxt = "All" });
            foreach (DataRow row in dsObject.Rows)
            {
                bool isIdstring = Navigation.FieldIsAString(row["ObjectValue"].ToString().Split("|")[0], props.passport);
                model.objectDDL.Add(new DDLprops() { text = row["UserName"].ToString(), valuetxt = row["ObjectValue"].ToString(), isIdstring = isIdstring });
            }

        }
        private async Task BindUserDDL(UserInterfaceProps props, AuditReportSearch model)
        {
            var bReturnAllUsers = default(bool);
            string sSQL;
            var dsSettings = new DataSet();
            var dsUser = new DataTable();
            if (props.passport.UsingActiveDirectory)
            {
                sSQL = "SELECT UserName, ISNULL(DisplayName,'<unknown user>') AS DispName FROM SecureUser WHERE AccountType = 'A'";
            }
            else if (props.passport.UsingAzureActiveDirectory)
            {
                sSQL = "SELECT UserName, ISNULL(DisplayName,'<unknown user>') AS DispName FROM SecureUser WHERE AccountType = 'Z'";
            }
            else
            {
                sSQL = "SELECT UserName, ISNULL(DisplayName,'<unknown user>') AS DispName FROM SecureUser WHERE AccountType = 'S'";
            }

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("select * from Settings where Section='AuditReport' and Item='ReturnAll'", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsSettings);
                    }
                }
            }

            if (dsSettings is not null)
            {
                if (dsSettings.Tables.Count > 0 && dsSettings.Tables[0].Rows.Count > 0)
                {

                    if (Convert.ToInt32(dsSettings.Tables[0].Rows[0]["Id"]) != 0 & Convert.ToString(dsSettings.Tables[0].Rows[0]["ItemValue"]).Length > 0)
                    {
                        bReturnAllUsers = Convert.ToBoolean(Convert.ToString(dsSettings.Tables[0].Rows[0]["ItemValue"]));
                    }
                }
                if (!bReturnAllUsers)
                {
                    sSQL = sSQL + " AND UserName IN " + Constants.vbCrLf;
                    sSQL = sSQL + " (SELECT DISTINCT [OperatorsId] FROM [SLAuditUpdates] UNION" + Constants.vbCrLf;
                    sSQL = sSQL + "  SELECT DISTINCT [OperatorsId] FROM [SLAuditConfData] UNION" + Constants.vbCrLf;
                    sSQL = sSQL + "  SELECT DISTINCT [OperatorsId] FROM [SLAuditLogins]" + Constants.vbCrLf;
                    sSQL = sSQL + "  WHERE [OperatorsId] IS NOT NULL AND [OperatorsId] > '')";
                }
            }

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsUser);
                    }
                }
            }
            model.userDDL.Add(new DDLprops() { text = "All Users", value = -1 });
            int ddlOrder = 0;
            foreach (DataRow row in dsUser.Rows)
            {
                model.userDDL.Add(new DDLprops() { text = row["UserName"].ToString(), value = ddlOrder });
                ddlOrder = ddlOrder + 1;
            }
        }
        private async Task RetentionInactiveRecords_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
           
            var dt = await RetentionInactiveRecords_Query(model, props, true);

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task InactivePullList_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
            var dt = await InactivePullList_Query(model, props, true);

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task RetentionCertifieDisposition_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            model.lblTitle = "Certificate of Disposition";
            if (props.paramss.isQueryFromDDL)
            {
                model.ddlid = Convert.ToInt32(props.paramss.id);
            }
            else
            {
                await RetentionCertifieDisposition_DropDown(model, props);
            }

            string sql = "SELECT count(*) ";

            sql += " FROM (([SLDestructCertItems] " + "LEFT JOIN [TrackingStatus] ON ([SLDestructCertItems].[TableName] = [TrackingStatus].[TrackedTable] AND [SLDestructCertItems].[TableId] = [TrackingStatus].[TrackedTableId])) " + "LEFT JOIN [Tables] ON [SLDestructCertItems].[TableName] = [Tables].[TableName]) " + "LEFT JOIN [SLRetentionCodes] ON [SLDestructCertItems].[RetentionCode] = [SLRetentionCodes].[Id] " + "LEFT JOIN SecureUser ON SLDestructCertItems.ApprovedBy = SecureUser.UserName " + "WHERE [SLDestructCertItems].[SLDestructionCertsID] = @param AND [SLDestructCertItems].[DispositionType] <> 0";

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@param", model.ddlid);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task RetentionFinalDisposition_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {

            if (props.passport.CheckPermission("Disposition", SecureObject.SecureObjectType.Retention, Permissions.Permission.Access))
            {
                model.lblTitle = "Eligible for Final Disposition Reports";
                // get trackble containers
                model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
                // check for btn conditions
                await RetentionFinalDisposition_btnConditions(model, props);
                // check if call was from dropdown
                if (!props.paramss.isQueryFromDDL)
                {
                    await RetentionFinalDisposition_Dropdown(model, props);
                }
                else
                {
                    model.ddlid = Convert.ToInt32(props.paramss.id);
                }
            }

            string sql = "SELECT COUNT(*) ";

            sql += " FROM (([SLDestructCertItems] " + " LEFT JOIN [TrackingStatus] ON ([SLDestructCertItems].[TableName] = [TrackingStatus].[TrackedTable] AND [SLDestructCertItems].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLDestructCertItems].[TableName] = [Tables].[TableName]) " + " LEFT JOIN [SLRetentionCodes] ON [SLDestructCertItems].[RetentionCode] = [SLRetentionCodes].[Id] " + " LEFT JOIN SecureUser ON SLDestructCertItems.ApprovedBy = SecureUser.UserName " + " WHERE ([SLDestructCertItems].[DispositionDate] IS NULL AND [SLDestructCertItems].[SLDestructionCertsId] = @SLDestructionCertsId) " + "   AND ([SLDestructCertItems].[LegalHold] IS NULL OR [SLDestructCertItems].[LegalHold] = 0) " + "   AND ([SLDestructCertItems].[RetentionHold] IS NULL OR [SLDestructCertItems].[RetentionHold] = 0) " + "   AND ([SLRetentionCodes].[RetentionLegalHold] IS NULL OR [SLRetentionCodes].[RetentionLegalHold] = 0)";

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SLDestructionCertsId ", model.ddlid);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }

        private async Task RetentionCodesWithCitations(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model.lblTitle = "Retention Codes w/ Citations";

            var dt = new DataTable();
            dt = await RetentionCodesWithCitations_QueryTable(model, props);
            await RetentionCodesWithCitations_QueryTableCount(model, props);
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, false, false, false, model, props);
        }
        private async Task RetentionCodesWithCitations_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT count(*) FROM ([SLRetentionCodes] C " + "LEFT JOIN [SLRetentionCitaCodes] CC ON C.Id = CC.[SLRetentionCodesId]) " + "LEFT JOIN [SLRetentionCitations] Ci ON CC.[SLRetentionCitationsCitation] = Ci.Citation";

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }
        private async Task<DataTable> RetentionCodesWithCitations_QueryTable(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT C.Id AS Code, C.Description, C.DeptOfRecord AS [Department of Record], " + "C.RetentionPeriodLegal AS [Retention Legal Period], C.RetentionPeriodUser AS [Retention User Period], C.RetentionPeriodTotal AS [Retention Total Period], " + "Ci.Citation, Ci.Subject, Ci.LegalPeriod AS [Legal Period] " + "FROM ([SLRetentionCodes] C " + "LEFT JOIN [SLRetentionCitaCodes] CC ON C.Id = CC.[SLRetentionCodesId]) " + "LEFT JOIN [SLRetentionCitations] Ci ON CC.[SLRetentionCitationsCitation] = Ci.Citation ORDER BY C.Id";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        private async Task RetentionCodes(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model.lblTitle = "All Retention Codes";
            var dt = new DataTable();
            dt = await RetentionCodes_QueryTable(model, props);
            await RetentionCodes_QueryTableCount(model, props);
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, false, false, false, model, props);
        }

        private async Task RetentionCodes_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT Count(*) FROM [SLRetentionCodes]";

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task<DataTable> RetentionCodes_QueryTable(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT Id AS [Code], Description, DeptOFRecord AS [Department of Record], InactivityEventType AS [Inactivity Event Type]," + "InactivityPeriod AS [Inactivity Period], InactivityForceToEndOfYear AS [Inactivity Force To Year End], RetentionEventType AS [Retention Event Type], " + "RetentionPeriodLegal AS [Retention Legal Period], RetentionPeriodUser AS [Retention User Period], RetentionPeriodTotal AS [Retention Total Period], " + "RetentionLegalHold AS [Legal Hold], RetentionPeriodForceToEndOfYear AS [Retention Force to Year End]" + " FROM [SLRetentionCodes] ORDER BY Id";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        private async Task RetentionCitationsWithRetCodes(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model.lblTitle = "All Citations w/ Retention Codes";
            var dt = new DataTable();
            dt = await RetentionCitationsWithRetCodes_QueryTable(model, props);
            await RetentionCitationsWithRetCodes_QueryTableCount(model, props);
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, false, false, false, model, props);
        }
        private async Task RetentionCitationsWithRetCodes_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT  count(*) FROM ([SLRetentionCitations] Ci " + "LEFT JOIN [SLRetentionCitaCodes] CC ON Ci.[Citation] = CC.[SLRetentionCitationsCitation]) " + "LEFT JOIN [SLRetentionCodes] C ON CC.[SLRetentionCodesId] = C.[Id]";

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task<DataTable> RetentionCitationsWithRetCodes_QueryTable(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT Ci.Citation, Ci.Subject, Ci.LegalPeriod AS [Citation Legal Period], C.Id AS [Retention Code], C.Description AS [Retention Description],  " + "C.RetentionPeriodLegal AS [Retention Legal Period], C.RetentionPeriodUser AS [Retention User Period], C.RetentionPeriodTotal AS [Retention Total Period] " + "FROM ([SLRetentionCitations] Ci " + "LEFT JOIN [SLRetentionCitaCodes] CC ON Ci.[Citation] = CC.[SLRetentionCitationsCitation]) " + "LEFT JOIN [SLRetentionCodes] C ON CC.[SLRetentionCodesId] = C.[Id] ORDER BY Citation";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        private async Task RetentionCitations(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model.lblTitle = "All Citations";
            var dt = new DataTable();
            dt = await RetentionCitations_QueryTable(model, props);
            await RetentionCitations_QueryTableCount(model, props);
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, false, false, false, model, props);
        }

        private async Task<DataTable> RetentionCitations_QueryTable(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT Citation, Subject, LegalPeriod AS [Legal Period] FROM [SLRetentionCitations] ORDER BY Citation";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
        private async Task RetentionCitations_QueryTableCount(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT count(*) FROM [SLRetentionCitations]";
            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }
        private async Task RetentionRecordsOnHold(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model.lblTitle = "All Records On Hold";
            var result = new List<Table>();
            var objSLRetentionInactive = new SLRetentionInactive();
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                context.SLRetentionInactives.Add(objSLRetentionInactive);
                context.SaveChanges();
                context.SLRetentionInactives.Remove(objSLRetentionInactive);
                context.SaveChanges();
                result = context.Tables.Where(x => x.RetentionPeriodActive == true).ToList();
            }


            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);

            string insertQuery = "INSERT INTO [SLRetentionInactive] ([TableName], [TableId], [Batch], [RetentionCode], [ScheduledInactivity], " + "[EventDate], [DeptOfRecord], [HoldReason], [LegalHold], [RetentionHold], [SLDestructCertItemId]) SELECT [SLDestructCertItems].[TableName], " + "[SLDestructCertItems].[TableId], " + objSLRetentionInactive.Id.ToString() + ", [SLDestructCertItems].[RetentionCode], [SLDestructCertItems].[ScheduledDestruction], " + "[SLDestructCertItems].[SnoozeUntil], [SLRetentionCodes].[DeptOfRecord], [SLDestructCertItems].[HoldReason], [SLDestructCertItems].[LegalHold], " + "[SLDestructCertItems].[RetentionHold], [SLDestructCertItems].[Id] FROM ([SLDestructCertItems] LEFT JOIN [SLRetentionCodes] ON [SLDestructCertItems].[RetentionCode] = [SLRetentionCodes].[Id]) WHERE ([SLDestructCertItems].[LegalHold] <> 0) Or ([SLDestructCertItems].[RetentionHold] <> 0)";

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            foreach (var item in result)
            {
                var data = new DataTable();
                string tableName = item.TableName;
                string idFieldName = item.IdFieldName;

                string sqlQuery = string.Format("SELECT {0}.*, {1} AS [TableId], RetentionEventType, RetentionPeriodTotal, [SLRetentionCodes].[Id] AS [RetentionCode], " + " [SLRetentionCodes].[DeptOfRecord] AS [DeptOfRecord], [SLRetentionCodes].[RetentionLegalHold] AS [RetentionCodeHold], 'Retention Code on Hold' AS [HoldReason] ", tableName, idFieldName) + string.Format("  FROM ([{0}] LEFT JOIN [SLRetentionCodes] ON {0}.{1} = [SLRetentionCodes].[Id]) ", tableName, item.RetentionFieldName) + string.Format("  LEFT JOIN ([SLDestructCertItems] LEFT JOIN [SLDestructionCerts] ON [SLDestructCertItems].[SLDestructionCertsId] = [SLDestructionCerts].[Id]) ON {0} = [SLDestructCertItems].[TableId] ", idFieldName) + string.Format("   AND [SLDestructCertItems].[TableName] = '{0}' ", tableName) + string.Format(" WHERE ([SLRetentionCodes].[RetentionLegalHold] <> 0) AND (([SLDestructCertItems].[TableName] = '{0}') OR ([SLDestructCertItems].[TableName] IS NULL)) AND (([SLDestructionCerts].[DateDestroyed] = 0) OR ([SLDestructionCerts].[DateDestroyed] IS NULL))", tableName);

                using (var conn = new SqlConnection(props.passport.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(sqlQuery, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(data);
                        }
                    }
                }

                if (data.Rows.Count > 0)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        string tableId = row["TableId"].ToString();
                        var DoesExist = new SLRetentionInactive();
                        using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
                        {
                            DoesExist = context.SLRetentionInactives.FirstOrDefault(x => x.TableName == tableName & x.TableId == tableId & x.Batch == objSLRetentionInactive.Id);
                        }

                        if (DoesExist is not null)
                        {
                            DoesExist.RetentionCodeHold = Convert.ToBoolean(1);
                            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
                            {
                                context.SLRetentionInactives.Attach(DoesExist);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            string retentionEventType = Convert.ToString(row["RetentionEventType"]);
                            string dateField = string.Empty;
                            var ScheduledInactivity = new DateTime();

                            if (retentionEventType == "Date Opened")
                            {
                                dateField = item.RetentionDateOpenedField;
                            }
                            else if (retentionEventType == "Date Created")
                            {
                                dateField = item.RetentionDateCreateField;
                            }
                            else if (retentionEventType == "Date Closed")
                            {
                                dateField = item.RetentionDateClosedField;
                            }
                            else if (retentionEventType == "Date Other")
                            {
                                dateField = item.RetentionDateOtherField;
                            }

                            if (!string.IsNullOrWhiteSpace(dateField) && !string.IsNullOrWhiteSpace(row[dateField].ToString()))
                            {
                                ScheduledInactivity = Convert.ToDateTime(row[dateField]).AddYears(Convert.ToInt32(row["RetentionPeriodTotal"]));
                            }

                            string insertRow = "INSERT INTO SLRetentionInactive(TableName, TableId, Batch, RetentionCode, ScheduledInactivity, DeptOfRecord, RetentionCodeHold, SLDestructCertItemId) " + "VALUES(@TableName, @TableId, @Batch, @RetentionCode, @ScheduledInactivity, @DeptOfRecord, @RetentionCodeHold, @SLDestructCertItemId)";

                            using (var conn = new SqlConnection(props.passport.ConnectionString))
                            {
                                await conn.OpenAsync();
                                using (var cmd = new SqlCommand(insertRow, conn))
                                {
                                    cmd.Parameters.AddWithValue("@TableName", tableName);
                                    cmd.Parameters.AddWithValue("@TableId", tableId);
                                    cmd.Parameters.AddWithValue("@Batch", objSLRetentionInactive.Id);
                                    cmd.Parameters.AddWithValue("@RetentionCode", row["RetentionCode"]);

                                    if (ScheduledInactivity == DateTime.MinValue)
                                    {
                                        cmd.Parameters.AddWithValue("@ScheduledInactivity", DBNull.Value);
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@ScheduledInactivity", ScheduledInactivity);
                                    }

                                    cmd.Parameters.AddWithValue("@DeptOfRecord", row["DeptOfRecord"]);
                                    cmd.Parameters.AddWithValue("@RetentionCodeHold", 1);
                                    cmd.Parameters.AddWithValue("@SLDestructCertItemId", 0);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }

            string sql = "Select  ri.TableName AS [Folder Type], ri.TableName, '' AS Item, ri.TableId, ri.DeptofRecord AS [Department of Record], ri.RetentionCode AS [Retention Code]," + "ri.ScheduledInactivity AS [Scheduled Disposition], ri.EventDate AS [Snooze Until], ri.HoldReason AS [Reason], ri.LegalHold AS [Legal Hold], ri.RetentionHold  " + "as [Retention Hold], ri.RetentionCodeHold AS [Retention Code Hold], [TrackingStatus].[LocationsId], [Tables].[UserName]  " + "FROM (([SLRetentionInactive] ri " + "LEFT JOIN [TrackingStatus] ON (ri.[TableName] = [TrackingStatus].[TrackedTable] AND ri.[TableId] = [TrackingStatus].[TrackedTableId]))  " + "LEFT JOIN [Tables] ON ri.[TableName] = [Tables].[TableName]) WHERE ri.Batch = " + objSLRetentionInactive.Id.ToString() + "";

            sql += " ORDER BY ri.TableId ";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            if (props.paramss.isCountRecord)
            {
                string sqlCount = "Select count(*) " + "FROM (([SLRetentionInactive] ri " + "LEFT JOIN [TrackingStatus] ON (ri.[TableName] = [TrackingStatus].[TrackedTable] AND ri.[TableId] = [TrackingStatus].[TrackedTableId]))  " + "LEFT JOIN [Tables] ON ri.[TableName] = [Tables].[TableName]) WHERE ri.Batch = " + objSLRetentionInactive.Id.ToString() + "";

                await RetentionRecordsOnHold_QueryTableCount(sqlCount, model, props);

            }

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        RetentionMassageDataForRequests(dt, model, props);
                        RetentionRemoveUnneededColumns(dt, string.Empty, model);
                        // DataTableReport(dt, False, True, False)
                    }
                }
            }
            using (var context = new TABFusionRMSContext(props.passport.ConnectionString))
            {
                var del = context.SLRetentionInactives.Where(x => x.Batch == objSLRetentionInactive.Id).ToList();
                context.SLRetentionInactives.RemoveRange(del);
            }
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, true, false, false, model, props);
        }

        private async Task RetentionRecordsOnHold_QueryTableCount(object sql, RetentionReportModel model, ReportingJsonModelReq props)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(Convert.ToString(sql), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task RetentionInactiveRecords(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            model.lblTitle = "Inactive Records";
            var dt = await RetentionInactiveRecords_Query(model, props);
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, true, false, false, model, props);
        }

        private async Task<DataTable> RetentionInactiveRecords_Query(RetentionReportModel model, ReportingJsonModelReq props)
        {

            var dt = await RetentionInactiveRecords_Query(model, props, false);
            RetentionMassageDataForRequests(dt, model, props);
            RetentionRemoveUnneededColumns(dt, string.Empty, model);
            return dt;
        }
        private async Task InactivePullList(RetentionReportModel model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Retention", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the inactive pull list";
                return;
            }
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            model.lblTitle = "Inactive Pull Lists";

            // btnCommand.Text = Languages.Translation("btnHTMLReportsSetInactive")

            var dt = await InactivePullList_Query(model, props, false);
            RetentionMassageDataForRequests(dt, model, props);
            RetentionRemoveUnneededColumns(dt, string.Empty, model);
            RetentionHeaders(dt, false, true, model);
            RetentionRows(dt, true, false, true, model, props);
        }

        private async Task<DataTable> InactivePullList_Query(RetentionReportModel model, ReportingJsonModelReq props, bool ispaging)
        {
            var dt = new DataTable();
            string sql = string.Empty;
            if (ispaging)
            {
                sql = "SELECT count(*) FROM (([SLRetentionInactive] " +
                  "LEFT JOIN [TrackingStatus] ON ([SLRetentionInactive].[TableName] = [TrackingStatus].[TrackedTable] " +
                  "AND [SLRetentionInactive].[TableId] = [TrackingStatus].[TrackedTableId])) " +
                  "LEFT JOIN [Tables] ON [SLRetentionInactive].[TableName] = [Tables].[TableName]) " +
                  "LEFT JOIN [SLRetentionCodes] ON [SLRetentionInactive].[RetentionCode] = [SLRetentionCodes].[Id]";
            }
            else
            {
                sql = "SELECT [Tables].[UserName] AS FolderType," +
                 "[SLRetentionInactive].[TableName], " +
                 "CAST([FileRoomOrder] AS VARCHAR) AS [File Room Order]," +
                 "'' AS [Item], '' AS [Currently At]," +
                 " [SLRetentionInactive].[TableId]," +
                 " [SLRetentionInactive].[EventDate] AS [Event Date]," +
                 "[SLRetentionCodes].[InactivityEventType] AS [Event Type]," +
                 "[SLRetentionInactive].[ScheduledInactivity] AS [Scheduled Inactivity]";
                foreach (DataRow row in model._TrackingTables.Rows)
                {
                    sql = sql + ", TrackingStatus." + row["TrackingStatusFieldName"].ToString();
                }

                sql = sql + " FROM (([SLRetentionInactive]" +
                    " LEFT JOIN [TrackingStatus] ON ([SLRetentionInactive].[TableName] = [TrackingStatus].[TrackedTable] " +
                    " AND [SLRetentionInactive].[TableId] = [TrackingStatus].[TrackedTableId])) " +
                    " LEFT JOIN [Tables] ON [SLRetentionInactive].[TableName] = [Tables].[TableName]) " +
                    " LEFT JOIN [SLRetentionCodes] ON [SLRetentionInactive].[RetentionCode] = [SLRetentionCodes].[Id] ORDER BY [SLRetentionInactive].ID";
                sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);
            }



            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            return dt;
        }

        private async Task<DataTable> RetentionInactiveRecords_Query(RetentionReportModel model, ReportingJsonModelReq props, bool ispaging)
        {
            var dt = new DataTable();
            string sql = string.Empty;
            if (ispaging)
            {
                sql = "SELECT count(*) " +
                      " FROM (([SLRetentionInactive] " +
                      "LEFT JOIN [TrackingStatus] ON ([SLRetentionInactive].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRetentionInactive].[TableId] = [TrackingStatus].[TrackedTableId])) " +
                      "LEFT JOIN [Tables] ON [SLRetentionInactive].[TableName] = [Tables].[TableName]) " +
                      "LEFT JOIN [SLRetentionCodes] ON [SLRetentionInactive].[RetentionCode] = [SLRetentionCodes].[Id] ";
            }
            else
            {
                sql = "SELECT [Tables].[UserName] AS [Folder Type], [SLRetentionInactive].[TableName], CAST([FileRoomOrder] AS VARCHAR) AS [File Room Order], " +
                           "'' AS [Item],'' AS [Currently At], [SLRetentionCodes].[InactivityEventType] AS [Event Type], [SLRetentionInactive].[TableId], " +
                           "[SLRetentionInactive].[EventDate] AS [Event Date], [SLRetentionInactive].[ScheduledInactivity] AS [Scheduled Inactivity]";
                foreach (DataRow row in model._TrackingTables.Rows)
                {
                    sql = sql + ", TrackingStatus." + row["TrackingStatusFieldName"].ToString();
                }
                sql = sql + " FROM (([SLRetentionInactive] " +
                    "LEFT JOIN [TrackingStatus] ON ([SLRetentionInactive].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRetentionInactive].[TableId] = [TrackingStatus].[TrackedTableId])) " +
                    "LEFT JOIN [Tables] ON [SLRetentionInactive].[TableName] = [Tables].[TableName]) " +
                    "LEFT JOIN [SLRetentionCodes] ON [SLRetentionInactive].[RetentionCode] = [SLRetentionCodes].[Id] ORDER BY [SLRetentionInactive].ID ";
                sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);
            }

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            return dt;

        }
  
        private async Task RetentionCertifieDisposition(RetentionReportModel model, ReportingJsonModelReq props)
        {

            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            model.lblTitle = "Certificate of Disposition";
            if (props.paramss.isQueryFromDDL)
            {
                model.ddlid = Convert.ToInt32(props.paramss.id);
            }
            else
            {
                await RetentionCertifieDisposition_DropDown(model, props);
            }

            var dt = await RetentionCertifieDisposition_Query(model, props);
            // RetentionCertifieDisposition_QueryTableCount()
            RetentionHeaders(dt, false, false, model);
            RetentionRows(dt, false, false, false, model, props);
        }

        private async Task<DataTable> RetentionCertifieDisposition_Query(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT [SLDestructCertItems].TableName, [Tables].UserName AS [Table Name],[SLDestructCertItems].TableId, '' AS Item,[SLRetentionCodes].[DeptOfRecord] AS [Department of Record], " + "[SLRetentionCodes].[Id] AS [Retention Code], [SLRetentionCodes].[RetentionEventType] AS [Event Type], [SLDestructCertItems].EventDate AS [Event Date],  " + "[SLDestructCertItems].ScheduledDestruction AS [Scheduled Disposition]," + "[Type of Disposition] = CASE " + "    WHEN ([SLDestructCertItems].[DispositionType] = '1') THEN 'Archived'" + "    WHEN ([SLDestructCertItems].[DispositionType] = '2') THEN 'Destroyed'" + "    WHEN ([SLDestructCertItems].[DispositionType] = '3') THEN 'Purged' " + "END";

            foreach (DataRow row in model._TrackingTables.Rows)
                sql += ", TrackingStatus." + row["TrackingStatusFieldName"].ToString();

            sql += ", ISNULL(SecureUser.DisplayName, SLDestructCertItems.ApprovedBy) AS [Authorized by]" + "FROM (([SLDestructCertItems] " + "LEFT JOIN [TrackingStatus] ON ([SLDestructCertItems].[TableName] = [TrackingStatus].[TrackedTable] AND [SLDestructCertItems].[TableId] = [TrackingStatus].[TrackedTableId])) " + "LEFT JOIN [Tables] ON [SLDestructCertItems].[TableName] = [Tables].[TableName]) " + "LEFT JOIN [SLRetentionCodes] ON [SLDestructCertItems].[RetentionCode] = [SLRetentionCodes].[Id] " + "LEFT JOIN SecureUser ON SLDestructCertItems.ApprovedBy = SecureUser.UserName " + "WHERE [SLDestructCertItems].[SLDestructionCertsID] = @param AND [SLDestructCertItems].[DispositionType] <> 0 ORDER BY [Tables].[TableName], [SLDestructCertItems].[EventDate]";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@param", model.ddlid);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        RetentionMassageDataForRequests(dt, model, props);
                        RetentionRemoveUnneededColumns(dt, string.Empty, model);
                    }
                }
            }
            return dt;
        }

        private async Task RetentionCertifieDisposition_DropDown(RetentionReportModel model, ReportingJsonModelReq props)
        {
            model.lblSelectReport = "Certificate of Disposition Report";
            string sql = "SELECT Id, DateDestroyed, DateCreated FROM SlDestructionCerts WHERE NOT DateDestroyed IS NULL order by id desc";
            var dt = new DataTable();

            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            int start = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (start == 0)
                {
                    model.ddlid = Convert.ToInt32(row["Id"]);
                }
                start = start + 1;
                model.ddlSelectReport.Add(new DDLItems() { text = row["DateCreated"].ToString(), value = Convert.ToString(row["Id"]), Id = Convert.ToString(row["Id"]) });
            }
        }
        private async Task RetentionFinalDisposition(RetentionReportModel model, ReportingJsonModelReq props)
        {
            if (props.passport.CheckPermission("Disposition", SecureObject.SecureObjectType.Retention, Permissions.Permission.Access))
            {
                model.lblTitle = "Eligible for Final Disposition Reports";
                // get trackble containers
                model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
                // check for btn conditions
                await RetentionFinalDisposition_btnConditions(model, props);
                // check if call was from dropdown
                if (!props.paramss.isQueryFromDDL)
                {
                    await RetentionFinalDisposition_Dropdown(model, props);
                }
                else
                {
                    model.ddlid = Convert.ToInt32(props.paramss.id);
                }
            }
            // query the table
            var dt = await RetentionFinalDisposition_QueryTable(model, props);
            // create header and rows
            RetentionHeaders(dt, false, true, model);
            RetentionRows(dt, true, false, true, model, props);
        }

        private void RetentionRows(DataTable data, bool replaceCrLf, bool auditReport, bool ishiddenField, RetentionReportModel model, ReportingJsonModelReq props)
        {
            string tableid = string.Empty;
            string tableName = string.Empty;
            foreach (DataRow row in data.Rows)
            {
                var cell = new List<string>();
                foreach (DataColumn col in data.Columns)
                {
                    if (!auditReport && !Navigation.IsSystemColumn(col.ColumnName) || auditReport && (!Navigation.IsSystemColumn(col.ColumnName) || string.Compare(col.ColumnName, "tablename", true) == 0))
                    {
                        if (replaceCrLf && "|data before|databefore|data after|dataafter|".IndexOf(string.Format("|{0}|", col.ColumnName.ToLower())) > -1)
                        {
                            cell.Add(row[col].ToString().Replace(Constants.vbCrLf, "<br>"));
                        }
                        else if (Navigation.IsADateType(col.DataType))
                        {
                            if (!string.IsNullOrEmpty(row[col].ToString()))
                            {
                                cell.Add(DateTime.Parse(row[col].ToString()).ToString(props.DateFormat));
                            }
                            else
                            {
                                cell.Add("");
                            }
                        }
                        else if (!string.IsNullOrEmpty(row[col].ToString()))
                        {
                            cell.Add(row[col].ToString());
                        }
                        else
                        {
                            cell.Add("");

                        }
                    }
                    // written for if table need the hidden field which provide tableid and tableName
                    else if (ishiddenField)
                    {
                        if (col.ColumnName.ToLower() == "tableid")
                        {
                            // cell.Add(row(col).ToString)
                            tableid = row[col].ToString();
                        }
                        if (col.ColumnName.ToLower() == "tablename")
                        {
                            // cell.Add(row(col).ToString)
                            tableName = row[col].ToString();
                        }
                    }
                }
                if (ishiddenField)
                {
                    var cells = new List<string>();
                    cells.Add(tableid + "||" + tableName);
                    // this extra loop written for rearranging the loop
                    foreach (string item in cell)
                        cells.Add(item);
                    model.ListOfRows.Add(cells);
                }
                else
                {
                    model.ListOfRows.Add(cell);
                }
            }
        }

        private void RetentionHeaders(DataTable data, bool auditReport, bool ishiddenField, RetentionReportModel model)
        {
            // written for if table need the hidden field which provide tableid and tableName
            if (ishiddenField)
                model.ListOfHeader.Add("id");
            foreach (DataColumn col in data.Columns)
            {
                if (!auditReport && !Navigation.IsSystemColumn(col.ColumnName) || auditReport && (!Navigation.IsSystemColumn(col.ColumnName) || string.Compare(col.ColumnName, "tablename", true) == 0))
                {
                    if (col.ExtendedProperties["heading"] is null)
                    {
                        if (string.Compare(col.ColumnName, col.Caption) != 0)
                        {
                            model.ListOfHeader.Add(col.Caption);
                        }
                        else
                        {
                            model.ListOfHeader.Add(col.ColumnName);
                        }
                    }
                    else
                    {
                        model.ListOfHeader.Add(Convert.ToString(col.ExtendedProperties["heading"]));
                    }
                }
                else
                {
                    // add logic
                }
            }
        }
        private async Task<DataTable> RetentionFinalDisposition_QueryTable(RetentionReportModel model, ReportingJsonModelReq props)
        {
            string sql = "SELECT [SLDestructCertItems].TableName, [Tables].UserName AS [Table Name], [SLDestructCertItems].TableId, '' AS Item, [SLRetentionCodes].[DeptOfRecord] AS [Department of Record], " + "[SLRetentionCodes].[Id] AS [Retention Code], [SLRetentionCodes].[RetentionEventType] AS [Event Type], [SLDestructCertItems].EventDate AS [Event Date],  " + "[SLDestructCertItems].ScheduledDestruction AS [Scheduled Disposition], '' AS [Approved for Disposition]";

            foreach (DataRow row in model._TrackingTables.Rows)
                sql += ", [TrackingStatus].[" + Navigation.MakeSimpleField(row["TrackingStatusFieldName"].ToString()) + "]";

            sql += " FROM (([SLDestructCertItems] " + " LEFT JOIN [TrackingStatus] ON ([SLDestructCertItems].[TableName] = [TrackingStatus].[TrackedTable] AND [SLDestructCertItems].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLDestructCertItems].[TableName] = [Tables].[TableName]) " + " LEFT JOIN [SLRetentionCodes] ON [SLDestructCertItems].[RetentionCode] = [SLRetentionCodes].[Id] " + " LEFT JOIN SecureUser ON SLDestructCertItems.ApprovedBy = SecureUser.UserName " + " WHERE ([SLDestructCertItems].[DispositionDate] IS NULL AND [SLDestructCertItems].[SLDestructionCertsId] = @SLDestructionCertsId) " + "   AND ([SLDestructCertItems].[LegalHold] IS NULL OR [SLDestructCertItems].[LegalHold] = 0) " + "   AND ([SLDestructCertItems].[RetentionHold] IS NULL OR [SLDestructCertItems].[RetentionHold] = 0) " + "   AND ([SLRetentionCodes].[RetentionLegalHold] IS NULL OR [SLRetentionCodes].[RetentionLegalHold] = 0) ";

            sql += " ORDER BY [SLDestructCertItems].Id";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SLDestructionCertsId ", model.ddlid);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        RetentionMassageDataForRequests(dt, model, props);
                        RetentionRemoveUnneededColumns(dt, string.Empty, model);
                        // DataTableReport(dt, False, True, False)
                    }
                }
            }
            return dt;
        }
        private void RetentionRemoveUnneededColumns(DataTable data, string status, RetentionReportModel model)
        {
            // _TrackingTables = GetTrackingContainerTypes(_passport.Connection())
            switch (status.ToLower() ?? "")
            {
                case "new":
                    {
                        if (data.Columns.Contains("Status"))
                            data.Columns.Remove(data.Columns["Status"]);
                        if (data.Columns.Contains("Date Pulled"))
                            data.Columns.Remove(data.Columns["Date Pulled"]);
                        break;
                    }
                case "newbatchesreport":
                    {
                        if (data.Columns.Contains("Date Pulled"))
                            data.Columns.Remove(data.Columns["Date Pulled"]);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            if (data.Columns.Contains("EmployeeId"))
                data.Columns.Remove(data.Columns["EmployeeId"]);
            if (data.Columns.Contains("UserName"))
                data.Columns.Remove(data.Columns["UserName"]);

            foreach (DataRow row in model._TrackingTables.Rows)
            {
                if (data.Columns.Contains(row["TrackingStatusFieldName"].ToString()))
                    data.Columns.Remove(data.Columns[row["TrackingStatusFieldName"].ToString()]);
            }
        }

        private void RetentionMassageDataForRequests(DataTable data, RetentionReportModel model, ReportingJsonModelReq props)
        {
            var requestorRow = Tracking.GetRequestorTableInfo(props.passport);
            string requestorTableName = requestorRow["TableName"].ToString();
            var idsByTable = new Dictionary<string, string>();
            var listOfTableNames = new List<string>();
            idsByTable.Add(requestorTableName, "");
            listOfTableNames.Add(requestorTableName);

            foreach (DataRow row in data.Rows)
            {
                if (!idsByTable.ContainsKey(row["TableName"].ToString()))
                {
                    idsByTable.Add(row["TableName"].ToString(), "");
                    listOfTableNames.Add(row["TableName"].ToString());
                }

                if (string.IsNullOrEmpty(idsByTable[row["TableName"].ToString()]))
                {
                    idsByTable[row["TableName"].ToString()] += "'" + row["TableId"].ToString() + "'";
                }
                else
                {
                    idsByTable[row["TableName"].ToString()] += ",'" + row["TableId"].ToString().Replace("'", "''") + "'";
                }
            }

            var tablesInfo = Navigation.GetMultipleTableInfo(listOfTableNames, props.passport);
            var descriptions = new Dictionary<string, DataTable>();

            foreach (DataRow tableInfo in tablesInfo.Rows)
            {
                string ids = string.Empty;
                if (idsByTable.TryGetValue(tableInfo["TableName"].ToString(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                {
                    descriptions.Add(tableInfo["TableName"].ToString(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), props.passport, tableInfo, ids));
                }
            }

            foreach (DataRow row in data.Rows)
            {
                row["Item"] = Navigation.ExtractItemName(row["TableName"].ToString(), row["TableId"].ToString(), descriptions, tablesInfo, props.passport);
                if (data.Columns.Contains("Currently At"))
                    row["Currently At"] = GetTrackingItemName(row, props, model);
                if (data.Columns.Contains("Requestor") && !string.IsNullOrEmpty(row[requestorRow["TrackingStatusFieldName"].ToString()].ToString()))
                {
                    row["Requestor"] = Navigation.ExtractItemName(requestorTableName, row[requestorRow["TrackingStatusFieldName"].ToString()].ToString(), descriptions, tablesInfo, props.passport);
                }
            }
        }
        private async Task RetentionFinalDisposition_Dropdown(RetentionReportModel model, ReportingJsonModelReq props)
        {
            var dTomorrow = DateTime.Now.AddDays(1d);
            string Sql = "SELECT DISTINCT [SLDestructionCerts].[Id], [SLDestructionCerts].[DateDestroyed], [SLDestructionCerts].[DateCreated], ISNULL([SecureUser].[DisplayName], [SLDestructionCerts].[CreatedBy]) AS OperatorsId, DispositionTypeDesc = CASE WHEN ([SLDestructionCerts].[RetentionDispositionType] = '1') THEN 'Archived' " + " WHEN ([SLDestructionCerts].[RetentionDispositionType] = '2') THEN 'Destruction' WHEN ([SLDestructionCerts].[RetentionDispositionType] = '3') THEN 'Purged' END " + " FROM [SLDestructionCerts] " + " INNER JOIN [SLDestructCertItems] ON [SLDestructCertItems].[SLDestructionCertsId] = [SLDestructionCerts].[Id] " + " LEFT OUTER JOIN [SLRetentionCodes] ON [SLRetentionCodes].[Id] = [SLDestructCertItems].[RetentionCode] " + " LEFT OUTER JOIN [SecureUser] ON [SecureUser].[UserName] = [SLDestructionCerts].[CreatedBy] " + " WHERE ([SLDestructionCerts].[RetentionDispositionType] IN ({0})) " + "   AND ([SLDestructCertItems].[DispositionDate] IS NULL) " + "   AND ([SLDestructCertItems].[LegalHold] IS NULL OR [SLDestructCertItems].[LegalHold] = 0) " + "   AND ([SLDestructCertItems].[RetentionHold] IS NULL OR [SLDestructCertItems].[RetentionHold] = 0) " + string.Format("   AND ([SLDestructCertItems].[SnoozeUntil] IS NULL OR [SLDestructCertItems].[SnoozeUntil] < '{0}-{1}-{2}') ", dTomorrow.Year, dTomorrow.Month, dTomorrow.Day) + "   AND ([SLRetentionCodes].[RetentionLegalHold] IS NULL OR [SLRetentionCodes].[RetentionLegalHold] = 0) " + " ORDER BY [SLDestructionCerts].[Id] DESC";
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                Sql = string.Format(Sql, await RetentionFinalDispositiontypesInUse(props));
                using (var cmd = new SqlCommand(Sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        int start = 0;
                        string spacer = new string(' ', 1);
                        string extra;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (start == 0)
                            {
                                model.ddlid = Convert.ToInt32(row["id"]);
                                start = start + 1;
                            }
                            int maxDateLength = TrimAMPMIndicators(new DateTime(2000, 12, 31, 12, 59, 59).ToString("g", model.CultureInfo)).Length;
                            int maxIdLength = dt.Rows[0]["Id"].ToString().Length;
                            string item = string.Empty;
                            int maxUserLength = Convert.ToInt32(GetMaxUserLength(dt));
                            extra = row["DateCreated"].ToString();
                            item = string.Format("{0}{1}", spacer, extra.PadRight(maxDateLength, ' '));
                            item += string.Format("{0}{1}{0}{2}", spacer, row["OperatorsId"].ToString().PadRight(maxUserLength, ' '), row["DispositionTypeDesc"].ToString());
                            model.ddlSelectReport.Add(new DDLItems() { value = Convert.ToString(row["Id"]), text = item, Id = Convert.ToString(row["Id"]) });
                        }
                    }
                }
            }
        }
        private async Task<object> RetentionFinalDispositiontypesInUse(ReportingJsonModelReq props)
        {
            string typesInUse = "SELECT TOP 1 (STUFF((SELECT ',' + CAST(i.[RetentionFinalDisposition] AS VARCHAR) " + " FROM [Tables] i WHERE (i.RetentionFinalDisposition <> 0 AND i.RetentionFinalDisposition IS NOT NULL AND i.RetentionPeriodActive <> 0 AND i.RetentionPeriodActive IS NOT NULL) FOR XML PATH('')), 1, 1, '')) " + " FROM [Tables] t WHERE (t.RetentionFinalDisposition <> 0 AND t.RetentionFinalDisposition IS NOT NULL AND t.RetentionPeriodActive <> 0 AND t.RetentionPeriodActive IS NOT NULL)";
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(string.Empty, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandText = typesInUse;
                        typesInUse = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            return typesInUse;
        }
        private async Task RetentionFinalDisposition_btnConditions(RetentionReportModel model, ReportingJsonModelReq props)
        {
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                if (Retention.UsesRetentionType(FinalDisposition.PermanentArchive, conn))
                {
                    model.PermanentArchive.isBtnVisibal = true;
                    model.PermanentArchive.btnText = "Archival Report";
                }
                if (Retention.UsesRetentionType(FinalDisposition.Purge, conn))
                {
                    model.Purge.isBtnVisibal = true;
                    model.Purge.btnText = "Purge Report";
                }
                if (Retention.UsesRetentionType(FinalDisposition.Destruction, conn))
                {
                    model.Destruction.isBtnVisibal = true;
                    model.Destruction.btnText = "Destruction Report";
                }
            }
            model.SubmitDisposition.btnText = "Submit for Disposition";
            model.SubmitDisposition.isBtnVisibal = model.PermanentArchive.isBtnVisibal || model.Purge.isBtnVisibal || model.Destruction.isBtnVisibal;
        }
        private async Task WaitList_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "Wait List Report";
                return;
            }
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            await RequestWaitListQueryCount(model, props);
        }

        private async Task RequestWaitListQueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            string sql = string.Format("SELECT Count(*)" + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable]" + " AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) LEFT JOIN [Tables] ON" + " [SLRequestor].[TableName] = [Tables].[TableName] WHERE [SLRequestor].[Status] = @status", AddTrackingFields(model));
            // Dim orderClause = " ORDER BY SLRequestor.Priority, SLRequestor.DateRequested, Tables.UserName, SLRequestor.TableId"

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", "WaitList");
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }
        private async Task InProcess_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "In Process Report";
                return;
            }
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            await RequestInProcessQueryCount(model, props);
        }

        private async Task RequestInProcessQueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            string sql = string.Format("SELECT count(*)" + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLRequestor].[TableName] = [Tables].[TableName] WHERE [SLRequestor].[Status] = @status" + "");

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", "In Process");
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }

        private async Task RequestExceptions_Count(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "Exceptions Report";
                return;
            }
            // _TrackingTables = GetTrackingContainerTypes(_passport.Connection())
            await RequestExceptionQueryCount(model, props);
        }

        private async Task RequestExceptionQueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            string sql = string.Format("SELECT count(*) " + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLRequestor].[TableName] = [Tables].[TableName] WHERE [SLRequestor].[Status] = @status" + "");
            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", "Exception");
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }

        private async Task PullList_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "Pull List Report";
                return;
            }
            model._TrackingTables = Tracking.GetTrackingContainerTypes(props.passport.Connection());
            if (!props.paramss.isQueryFromDDL)
            {
                await GenerateDropdownPullList(model, props);
            }
            else
            {
                model.ddlid = Convert.ToInt32(props.paramss.id);
            }
            string sql = WriteBaseRequestQuery(true, model);
            if (model.ddlid == 0)
            {
                model.ddlid = -1;
            }
            sql += string.Format(" WHERE [SLRequestor].[SLPullListsId] = {0} ", model.ddlid);
            var dt = new DataTable();
            using (var conn = props.passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }
        private async Task RequestNewBatch_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = string.Format("New Batch #{0} Report", model.ddlid);
                return;
            }
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);

            string sql = WriteBaseRequestQuery(true, model);
            if (model.ddlid == 0)
            {
                model.ddlid = -1;
            }
            sql += string.Format(" WHERE [SLRequestor].[SLPullListsId] = {0} ", model.ddlid);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }

        private async Task Requestnew_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the new request report";
                return;
            }
            model._TrackingTables = Tracking.GetTrackingContainerTypes(props.passport.Connection());
            string sql = WriteBaseRequestQuery(true, model);
            sql += " WHERE [SLRequestor].[Status] = 'New'";

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }
        private async Task ObjectsInventory_QueryTableCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the object inventory report";
                return;
            }

            var lstTable = await QueryTableObjectsInventoryCount(model, props);
        }
        private async Task<DataTable> QueryTableObjectsInventoryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            DataRow locationInfo = null;
            var @params = new Parameters(props.passport);
            var query = new Query(props.passport);

            var dts = new DataTable();
            using (var conn = props.passport.Connection())
            {
                var tableInfo = Navigation.GetTableInfo(props.paramss.tableName, conn);
                model.lblSubtitle = tableInfo["UserName"].ToString();
                using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE TrackingTable = 1", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            locationInfo = dt.AsEnumerable().ElementAtOrDefault(0);
                    }
                }

                dts = await QueryTableObjectsInventoryCountOrData(true, locationInfo, tableInfo, props, model);

                if (dts.Rows.Count > 0)
                {
                    ExecutePagingQueryCount(Convert.ToInt32(dts.Rows[0][0]), model, props.paramss.pageNumber);
                }
                else
                {
                    ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
                }

            }
            return dts;
        }
        private void ObjectsOut_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            var @params = new Parameters(props.passport);
            @params.Paged = true;
            @params.PageIndex = props.paramss.pageNumber;
            if (!props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the items out report";
                return;
            }
            Dictionary<string, string> argidsByTable = null;
            var dt = Tracking.GetCurrentItemsOutReportCount(@params, props.passport, idsByTable: ref argidsByTable);

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }
        }

        private async Task PastDueTrackableItemsReport_QueryCount(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the past due report";
                return;
            }
            model.PageTitle = "Past Due Trackable Items Report";
            model.lblTitle = "Past Due Trackable Items Report";
            model.lblSubtitle = "As of " + DateTime.Parse(model.dateFromTxt.ToShortDateString()).ToString(props.DateFormat);
            model.lblReportDate = string.Format("Print Date : {0}", DateTime.Parse(model.dateFromTxt.ToShortDateString()).ToString(props.DateFormat));


            var _trackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            var _trackedTables = Tracking.GetTrackedTables(props.passport);
            // Dim dateFromTxt As Date = DateTime.Parse(txtDate.Text, CultureInfo)
            var param = new Parameters(props.passport);
            param.Paged = true;
            param.PageIndex = props.paramss.pageNumber;

            string todayDate = DateTime.Now.Date.ToString("d", CultureInfo.InvariantCulture);
            DateTime dateFromTxt = DateTime.Parse(todayDate, CultureInfo.InvariantCulture);



            var dt = Tracking.GetPagedPastDueTrackablesCount(dateFromTxt, props.passport, param, _trackingTables, _trackedTables, ref model.IdsByTable, ref model.Descriptions);

            if (dt.Rows.Count > 0)
            {
                ExecutePagingQueryCount(Convert.ToInt32(dt.Rows[0][0]), model, props.paramss.pageNumber);
            }
            else
            {
                ExecutePagingQueryCount(0, model, props.paramss.pageNumber);
            }

        }
        private async Task WaitList(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "Wait List Report";
                return;
            }


            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            model.lblTitle = "Wait List Report";
            var td = await RequestWaitListQuery(model, props);

            RequestHeaders(td, model);
            RequestRows(td, model, props);
        }
        private async Task<DataTable> RequestWaitListQuery(ReportsModels model, ReportingJsonModelReq props)
        {
            string sql = string.Format("SELECT [SLRequestor].[Priority], CONVERT(VARCHAR, [SLRequestor].[DateRequested], 100) AS 'Request Date'," + " [Tables].[UserName] AS 'Object Type', '' AS 'Object Description', '' AS 'Currently At', '' AS 'Requested By'," + " [SLRequestor].[Id] AS 'Request #', [SLRequestor].[TableName], [SLRequestor].[TableId], [SLRequestor].[EmployeeId]{0}" + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable]" + " AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) LEFT JOIN [Tables] ON" + " [SLRequestor].[TableName] = [Tables].[TableName] WHERE [SLRequestor].[Status] = @status", AddTrackingFields(model));
            string orderClause = " ORDER BY SLRequestor.Priority, SLRequestor.DateRequested, Tables.UserName, SLRequestor.TableId ";
            sql += orderClause;

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", "WaitList");
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        MassageDataForWaitList(dt, model, props);
                    }
                }
            }
            return dt;
        }
        private string AddTrackingFields(ReportsModels model)
        {
            var sb = new StringBuilder();
            foreach (DataRow row in model._TrackingTables.Rows)
                sb.Append(string.Format(", [TrackingStatus].[{0}]", row["TrackingStatusFieldName"].ToString()));
            return sb.ToString();
        }
        private void MassageDataForWaitList(DataTable dt, ReportsModels model, ReportingJsonModelReq props)
        {
            string requestorTableName = Tracking.GetRequestorTableName(props.passport);
            var idsByTable = new Dictionary<string, string>();
            var listOfTableNames = new List<string>();
            idsByTable.Add(requestorTableName, "");
            listOfTableNames.Add(requestorTableName);

            foreach (DataRow row in dt.Rows)
            {
                if (!idsByTable.ContainsKey(row["TableName"].ToString()))
                {
                    idsByTable.Add(row["TableName"].ToString(), "");
                    listOfTableNames.Add(row["TableName"].ToString());
                }

                if (string.IsNullOrEmpty(idsByTable[row["TableName"].ToString()]))
                {
                    idsByTable[row["TableName"].ToString()] += "'" + row["TableId"].ToString() + "'";
                }
                else
                {
                    idsByTable[row["TableName"].ToString()] += ",'" + row["TableId"].ToString().Replace("'", "''") + "'";
                }
            }

            var tablesInfo = Navigation.GetMultipleTableInfo(listOfTableNames, props.passport);
            var descriptions = new Dictionary<string, DataTable>();

            foreach (DataRow tableInfo in tablesInfo.Rows)
            {
                string ids = string.Empty;
                if (idsByTable.TryGetValue(tableInfo["TableName"].ToString(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                {
                    descriptions.Add(tableInfo["TableName"].ToString(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), props.passport, tableInfo, ids));
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                row["Object Description"] = Navigation.ExtractItemName(row["TableName"].ToString(), row["TableId"].ToString(), descriptions, tablesInfo, props.passport);
                row["Currently At"] = GetTrackingItemName(row, props, model);

                if (!string.IsNullOrEmpty(row["EmployeeId"].ToString()))
                {
                    row["Requested By"] = Navigation.ExtractItemName(requestorTableName, row["EmployeeId"].ToString(), descriptions, tablesInfo, props.passport);
                }
            }
            RemoveUnneededColumns(dt, string.Empty, model);
        }
        private async Task InProcess(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "In Process Report";
                return;
            }
            model._TrackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            model.lblTitle = "In Process Report";
            var td = await RequestInProcessQuery(model, props);
            RequestHeaders(td, model);
            RequestRows(td, model, props);
        }
        private async Task<DataTable> RequestInProcessQuery(ReportsModels model, ReportingJsonModelReq props)
        {
            string sql = string.Format("SELECT [SLRequestor].[SLPullListsId] AS 'Pull List #', CONVERT(VARCHAR, [SLRequestor].[DatePulled], 100) AS 'Date Pulled'," + " [Tables].[UserName] AS 'Folder Type', '' AS 'Folder Description'," + " CONVERT(VARCHAR, [SLRequestor].[DateRequested], 100) AS 'Request Date', '' AS 'Requestor'," + " [SLRequestor].[Priority], '' AS 'Currently At', [SLRequestor].[Id] AS 'Request #'," + " [SLRequestor].[TableName], [SLRequestor].[TableId], [SLRequestor].[EmployeeId]{0}" + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLRequestor].[TableName] = [Tables].[TableName] WHERE [SLRequestor].[Status] = @status" + "", AddTrackingFields(model));
            string orderClause = " ORDER BY SLRequestor.SLPullListsId, SLRequestor.DatePulled, Tables.UserName";
            sql += orderClause;

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", "In Process");
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        // Me.MassageDataForRequests(dt)
                        RemoveUnneededColumns(dt, string.Empty, model);
                    }
                }
            }
            return dt;
        }
        private async Task RequestExceptions(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "Exceptions Report";
                return;
            }
            model._TrackingTables = Tracking.GetTrackingContainerTypes(props.passport.Connection());
            model.lblTitle = "Exceptions Report";
            var td = await RequestExceptionQuery(model, props);
            RequestHeaders(td, model);
            RequestRows(td, model, props);
        }
        private void RequestRows(DataTable data, ReportsModels model, ReportingJsonModelReq props)
        {
            foreach (DataRow row in data.Rows)
            {
                var cell = new List<string>();
                foreach (DataColumn col in data.Columns)
                {
                    if (!Navigation.IsSystemColumn(col.ColumnName))
                    {
                        if (Navigation.IsADateType(col.DataType))
                        {
                            cell.Add(DateTime.Parse(row[col].ToString()).ToString(""));
                        }
                        else
                        {
                            cell.Add(row[col].ToString());
                        }
                    }
                }
                model.ListOfRows.Add(cell);
            }
        }
        private void RequestHeaders(DataTable data, ReportsModels model)
        {
            foreach (DataColumn col in data.Columns)
            {
                if (!Navigation.IsSystemColumn(col.ColumnName))
                {
                    if (col.ExtendedProperties["heading"] is null)
                    {
                        if (string.Compare(col.ColumnName, col.Caption) != 0)
                        {
                        }

                        else
                        {
                            model.ListOfHeader.Add(col.ColumnName);
                        }
                    }
                    else
                    {
                        model.ListOfHeader.Add(Convert.ToString(col.ExtendedProperties["heading"]));
                    }
                }
            }
        }
        private async Task<DataTable> RequestExceptionQuery(ReportsModels model, ReportingJsonModelReq props)
        {
            string sql = string.Format("SELECT [Tables].[UserName] AS 'Folder Type', '' AS 'Folder Description'," + " CONVERT(VARCHAR, [SLRequestor].[DateRequested], 100) AS 'Request Date', '' AS 'Requestor', CONVERT(VARCHAR, [SLRequestor].[DateNeeded], 100) AS 'Date Needed'," + " [SLRequestor].[ExceptionComments] AS 'Exception Comments', [SLRequestor].[Priority]," + " CONVERT(VARCHAR, [SLRequestor].[DatePulled], 100) AS 'Date Pulled','' AS 'Currently At', [SLRequestor].[Id] AS 'Request #'," + " [SLRequestor].[TableName], [SLRequestor].[TableId], [SLRequestor].[EmployeeId]{0}" + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLRequestor].[TableName] = [Tables].[TableName] WHERE [SLRequestor].[Status] = @status" + "", AddTrackingFields(model));
            string orderClause = " ORDER BY [Tables].[UserName], SLRequestor.TableId, SLRequestor.DateRequested";
            sql += orderClause;

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", "Exception");
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        MassageDataForRequests(dt, props, model);
                        RemoveUnneededColumns(dt, string.Empty, model);
                    }
                }
            }
            return dt;
        }
        private async Task PullList(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "Pull List Report";
                return;
            }
            model._TrackingTables = Tracking.GetTrackingContainerTypes(props.passport.Connection());
            model.lblTitle = "Pull List Report";
            model.lblSelectReport = "Select Pull List:";
            if (!props.paramss.isQueryFromDDL)
            {
                await GenerateDropdownPullList(model, props);
            }
            else
            {
                model.ddlid = Convert.ToInt32(props.paramss.id);
            }
            string sql = WriteBaseRequestQuery(false, model);
            if (model.ddlid == 0)
            {
                model.ddlid = -1;
            }
            sql += string.Format(" WHERE [SLRequestor].[SLPullListsId] = {0} ORDER BY SLRequestor.Priority, SLRequestor.FileRoomOrder ", model.ddlid);

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = await RequestTable(sql, model.ddlid.ToString(), props, model);
            PullListHeaders(dt, model);
            PullListRows(dt, model, props);

        }
        private void PullListRows(DataTable data, ReportsModels model, ReportingJsonModelReq props)
        {
            foreach (DataRow row in data.Rows)
            {
                var cell = new List<string>();
                foreach (DataColumn col in data.Columns)
                {
                    if (!Navigation.IsSystemColumn(col.ColumnName))
                    {
                        if (Navigation.IsADateType(col.DataType))
                        {
                            cell.Add(DateTime.Parse(row[col].ToString()).ToString(props.DateFormat));
                        }
                        else
                        {
                            cell.Add(row[col].ToString());
                        }
                    }
                }
                model.ListOfRows.Add(cell);
            }
        }
        private void PullListHeaders(DataTable data, ReportsModels model)
        {
            foreach (DataColumn col in data.Columns)
            {
                if (!Navigation.IsSystemColumn(col.ColumnName))
                {
                    if (col.ExtendedProperties["heading"] is null)
                    {
                        if (string.Compare(col.ColumnName, col.Caption) != 0)
                        {
                        }

                        else
                        {
                            model.ListOfHeader.Add(col.ColumnName);
                        }
                    }
                    else
                    {
                        model.ListOfHeader.Add(Convert.ToString(col.ExtendedProperties["heading"]));
                    }
                }
            }
        }
        private async Task GenerateDropdownPullList(ReportsModels model, ReportingJsonModelReq props)
        {
            string Sql = "SELECT Id, [DateCreated], OperatorsId, CASE BatchPullList WHEN 0 THEN 'Regular' ELSE 'Batch' END AS BatchType" + " FROM SLPullLists WHERE ((BatchPullList = 0) OR (BatchPullList <> 0 AND BatchPrinted <> 0))" + " ORDER BY Id Desc";
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(Sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            int maxDateLength = TrimAMPMIndicators(new DateTime(2000, 12, 31, 12, 59, 59).ToString("g", model.CultureInfo)).Length;
                            int maxIdLength = dt.Rows[0]["Id"].ToString().Length;
                            int start = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                if (start == 0)
                                {
                                    model.ddlid = Convert.ToInt32(row["Id"]);
                                }
                                start = start + 1;
                                model.ddlSelectReport.Add(new DDLItems() { value = row["Id"].ToString().PadLeft(maxIdLength, ' '), text = string.Format("{0} {1} {2}", DateTime.Parse(row["DateCreated"].ToString()).ToString(props.DateFormat), row["BatchType"].ToString().PadRight(10, ' '), row["OperatorsId"].ToString()) });
                            }
                        }
                    }
                }
            }
        }
        private async Task RequestNewBatch(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = string.Format("New Batch #{0} Report", model.ddlid);
                return;
            }
            model._TrackingTables = Tracking.GetTrackingContainerTypes(new SqlConnection(props.passport.ConnectionString));
            if (!model.isPullListDDLCall)
            {
                GenerateDropdownRequestBatch(model, props);
            }
            model.lblTitle = string.Format("New Batch #{0} Report", model.ddlid);
            string sql = WriteBaseRequestQuery(false, model);
            if (model.ddlid == 0)
            {
                model.ddlid = -1;
            }
            sql += string.Format(" WHERE [SLRequestor].[SLPullListsId] = {0} ORDER BY SLRequestor.Priority, SLRequestor.FileRoomOrder ", model.ddlid);

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = await RequestTable(sql, model.ddlid.ToString(), props, model);
            buildHeadersWithHiddenFields(dt, model);
            buildRowsWithHiddenFields(dt, model, props);
        }
        private void GenerateDropdownRequestBatch(ReportsModels model, ReportingJsonModelReq props)
        {
            string Sql = "SELECT Id, [DateCreated], OperatorsId, COALESCE('[' + SLBatchRequestComment + ']', '') AS Comment" + " FROM SLPullLists WHERE BatchPullList <> 0 AND BatchPrinted = 0 " + " ORDER BY Id Desc";
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                using (var cmd = new SqlCommand(Sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            int maxDateLength = TrimAMPMIndicators(new DateTime(2000, 12, 31, 12, 59, 59).ToString("g", model.CultureInfo)).Length;
                            int maxIdLength = dt.Rows[0]["Id"].ToString().Length;
                            int maxCommentLength = 20;
                            int start = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                if (start == 0)
                                {
                                    model.ddlid = Convert.ToInt32(row["Id"]);
                                }
                                start = start + 1;
                                string test = string.Empty;
                                test = DateTime.Parse(row["DateCreated"].ToString()).ToString(props.DateFormat);
                                test = row["Comment"].ToString();
                                int maxUserLength = Convert.ToInt32(GetMaxUserLength(dt));
                                if (test.Length > maxCommentLength)
                                    test = string.Format("{0}]...", test.Substring(0, maxCommentLength));
                                string items = string.Format("{0} {1} {2}", DateTime.Parse(row["DateCreated"].ToString()).ToString(props.DateFormat), row["OperatorsId"].ToString().PadLeft(maxUserLength, ' '), test);
                                model.ddlSelectReport.Add(new DDLItems() { text = items, value = row["Id"].ToString() });
                            }
                        }
                    }
                }
            }
        }
        protected object GetMaxUserLength(DataTable dt)
        {
            int rtn = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["OperatorsId"].ToString().Length > rtn)
                    rtn = row["OperatorsId"].ToString().Length;
            }

            return rtn;
        }
        private string TrimAMPMIndicators(string text)
        {
            text = text.ToLower();
            if (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator))
            {
                text = text.Replace(string.Format(" {0}", System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator.ToLower()), System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator.ToLower());
                text = text.Replace(string.Format(" {0}", System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator.Substring(0, 1).ToLower()), System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.PMDesignator.Substring(0, 1).ToLower());
            }
            if (!string.IsNullOrEmpty(System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AMDesignator))
            {
                text = text.Replace(string.Format(" {0}", System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AMDesignator.ToLower()), System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AMDesignator.ToLower());
                text = text.Replace(string.Format(" {0}", System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AMDesignator.Substring(0, 1).ToLower()), System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.AMDesignator.Substring(0, 1).ToLower());
            }
            return text;
        }
        private async Task Requestnew(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the new request report";
                return;
            }
            model._TrackingTables = Tracking.GetTrackingContainerTypes(new SqlConnection(props.passport.ConnectionString));
            model.lblTitle = "New Requests Report";
            model.lblSubtitle = string.Empty;
            // btnCommand.Text = Languages.Translation("lblHTMLReportsSendCheckedItmLst")
            string sql = WriteBaseRequestQuery(false, model);
            sql += " WHERE [SLRequestor].[Status] = 'New' ORDER BY [SLRequestor].[Id] ";

            // NOTE: query will through exception if does not include ORDER BY
            sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);

            var dt = await RequestTable(sql, "new", props, model);
            buildHeadersWithHiddenFields(dt, model);
            buildRowsWithHiddenFields(dt, model, props);
        }
        private void buildRowsWithHiddenFields(DataTable data, ReportsModels model, ReportingJsonModelReq props)
        {
            string tableid = string.Empty;
            string tableName = string.Empty;

            foreach (DataRow row in data.Rows)
            {
                var cell = new List<string>();
                foreach (DataColumn col in data.Columns)
                {
                    if (!Navigation.IsSystemColumn(col.ColumnName))
                    {
                        if (Navigation.IsADateType(col.DataType))
                        {
                            cell.Add(DateTime.Parse(row[col].ToString()).ToString(props.DateFormat));
                        }
                        else
                        {
                            cell.Add(row[col].ToString());
                        }
                    }
                    else
                    {
                        if (col.ColumnName.ToLower() == "tableid")
                        {
                            // cell.Add(row(col).ToString)
                            tableid = row[col].ToString();
                        }
                        if (col.ColumnName.ToLower() == "tablename")
                        {
                            // cell.Add(row(col).ToString)
                            tableName = row[col].ToString();
                        }
                    }
                }
                var cells = new List<string>();
                cells.Add(tableid + "||" + tableName);
                // this extra loop written for rearranging the loop
                foreach (string item in cell)
                    cells.Add(item);
                model.ListOfRows.Add(cells);
            }
        }
        private void buildHeadersWithHiddenFields(DataTable data, ReportsModels model)
        {
            model.ListOfHeader.Add("id");
            foreach (DataColumn col in data.Columns)
            {
                if (!Navigation.IsSystemColumn(col.ColumnName))
                {
                    if (col.ExtendedProperties["heading"] is null)
                    {
                        if (string.Compare(col.ColumnName, col.Caption) != 0)
                        {
                        }

                        else
                        {
                            model.ListOfHeader.Add(col.ColumnName);
                        }
                    }
                    else
                    {
                        model.ListOfHeader.Add(Convert.ToString(col.ExtendedProperties["heading"]));
                    }
                }
            }
        }
        private async Task<DataTable> RequestTable(string sql, string status, ReportingJsonModelReq param, ReportsModels model)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(param.passport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                        MassageDataForRequests(dt, param, model);
                        RemoveUnneededColumns(dt, status, model);
                        // DataTableReport(dt)
                    }
                }
            }
            return dt;
        }
        protected void RemoveUnneededColumns(DataTable data, string status, ReportsModels model)
        {
            switch (status.ToLower() ?? "")
            {
                case "new":
                    {
                        if (data.Columns.Contains("Status"))
                            data.Columns.Remove(data.Columns["Status"]);
                        if (data.Columns.Contains("Date Pulled"))
                            data.Columns.Remove(data.Columns["Date Pulled"]);
                        break;
                    }
                case "newbatchesreport":
                    {
                        if (data.Columns.Contains("Date Pulled"))
                            data.Columns.Remove(data.Columns["Date Pulled"]);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            if (data.Columns.Contains("EmployeeId"))
                data.Columns.Remove(data.Columns["EmployeeId"]);
            if (data.Columns.Contains("UserName"))
                data.Columns.Remove(data.Columns["UserName"]);

            foreach (DataRow row in model._TrackingTables.Rows)
            {
                if (data.Columns.Contains(row["TrackingStatusFieldName"].ToString()))
                    data.Columns.Remove(data.Columns[row["TrackingStatusFieldName"].ToString()]);
            }
        }
        private void MassageDataForRequests(DataTable data, ReportingJsonModelReq props, ReportsModels model)
        {
            string requestorTableName = Tracking.GetRequestorTableName(props.passport);
            var idsByTable = new Dictionary<string, string>();
            var listOfTableNames = new List<string>();
            idsByTable.Add(requestorTableName, "");
            listOfTableNames.Add(requestorTableName);

            foreach (DataRow row in data.Rows)
            {
                if (!idsByTable.ContainsKey(row["TableName"].ToString()))
                {
                    idsByTable.Add(row["TableName"].ToString(), "");
                    listOfTableNames.Add(row["TableName"].ToString());
                }

                if (string.IsNullOrEmpty(idsByTable[row["TableName"].ToString()]))
                {
                    idsByTable[row["TableName"].ToString()] += "'" + row["TableId"].ToString() + "'";
                }
                else
                {
                    idsByTable[row["TableName"].ToString()] += ",'" + row["TableId"].ToString().Replace("'", "''") + "'";
                }
            }

            var tablesInfo = Navigation.GetMultipleTableInfo(listOfTableNames, props.passport);
            var descriptions = new Dictionary<string, DataTable>();

            foreach (DataRow tableInfo in tablesInfo.Rows)
            {
                string ids = string.Empty;
                if (idsByTable.TryGetValue(tableInfo["TableName"].ToString(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                {
                    descriptions.Add(tableInfo["TableName"].ToString(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), props.passport, tableInfo, ids));
                }
            }

            foreach (DataRow row in data.Rows)
            {
                row["Folder Description"] = Navigation.ExtractItemName(row["TableName"].ToString(), row["TableId"].ToString(), descriptions, tablesInfo, props.passport);
                row["Currently At"] = GetTrackingItemName(row, props, model);

                if (!string.IsNullOrEmpty(row["EmployeeId"].ToString()))
                {
                    row["Requestor"] = Navigation.ExtractItemName(requestorTableName, row["EmployeeId"].ToString(), descriptions, tablesInfo, props.passport);
                }

                if (!string.IsNullOrEmpty(row["Date Needed"].ToString()))
                    row["Date Needed"] = DateTime.Parse(row["Date Needed"].ToString()).ToString(props.DateFormat);
                if (!string.IsNullOrEmpty(row["Date Pulled"].ToString()))
                    row["Date Pulled"] = DateTime.Parse(row["Date Pulled"].ToString()).ToString(props.DateFormat);
            }
        }
        private string GetTrackingItemName(DataRow row, ReportingJsonModelReq props, ReportsModels model)
        {
            var sb = new StringBuilder();
            for (int i = 0, loopTo = model._TrackingTables.Rows.Count - 1; i <= loopTo; i++)
            {
                if (!string.IsNullOrEmpty(row[model._TrackingTables.Rows[i]["TrackingStatusFieldName"].ToString()].ToString()))
                {
                    sb.Append(Navigation.GetItemName(model._TrackingTables.Rows[i]["TableName"].ToString(), row[model._TrackingTables.Rows[i]["TrackingStatusFieldName"].ToString()].ToString(), props.passport));
                    if (i < model._TrackingTables.Rows.Count - 1)
                        sb.Append(Constants.vbCrLf);
                }
            }
            return sb.ToString();
        }
        private string WriteBaseRequestQuery(bool count, ReportsModels model)
        {
            string sql;

            if (count)
            {
                sql = string.Format("SELECT count(*) " + "FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLRequestor].[TableName] = [Tables].[TableName]");
            }
            else
            {
                sql = string.Format("SELECT [SLRequestor].[Priority], '' AS 'Currently At', [Tables].[UserName] AS 'Table Name'," + " [SLRequestor].[FileRoomOrder] AS 'File Room Order', '' AS 'Folder Description'," + " [SLRequestor].[DateRequested] AS 'Request Date', '' AS 'Requestor', [SLRequestor].[Instructions] AS 'Request Instructions'," + " CONVERT(VARCHAR, [SLRequestor].[DateNeeded], 100) AS 'Date Needed', [SLRequestor].[Status], [SLRequestor].[Id] AS 'Request #'," + " CONVERT(VARCHAR, [SLRequestor].[DatePulled], 100) AS 'Date Pulled'," + " [SLRequestor].[TableName], [SLRequestor].[TableId], [SLRequestor].[EmployeeId]{0}" + " FROM ([SLRequestor] LEFT JOIN [TrackingStatus] ON ([SLRequestor].[TableName] = [TrackingStatus].[TrackedTable] AND [SLRequestor].[TableId] = [TrackingStatus].[TrackedTableId])) " + " LEFT JOIN [Tables] ON [SLRequestor].[TableName] = [Tables].[TableName]", AddTrackingFields(model));
            }

            return sql;
        }
        private async Task ObjectsInventory(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the object inventory report";
                return;
            }

            model.lblTitle = "Object Inventory Report";
            var lstTable = await QueryTableObjectsInventory(model, props);
            ObjectsInventory_Headers(lstTable, model);
            ObjectsInventory_Rows(lstTable, model);
        }
        private void ObjectsInventory_Rows(DataTable data, ReportsModels model)
        {
            foreach (DataRow row in data.Rows)
            {
                var cell = new List<string>();
                cell.Add(row.ItemArray[0].Text());
                cell.Add(row.ItemArray[1].Text());
                cell.Add(row.ItemArray[2].Text());
                cell.Add(row.ItemArray[3].Text());
                cell.Add(row.ItemArray[4].Text());
                cell.Add(row.ItemArray[5].Text());
                model.ListOfRows.Add(cell);
            }
        }
        private void ObjectsInventory_Headers(DataTable data, ReportsModels model)
        {
            foreach (DataColumn col in data.Columns)
                model.ListOfHeader.Add(col.ColumnName.ToString());
        }
        private async Task<DataTable> QueryTableObjectsInventory(ReportsModels model, ReportingJsonModelReq props)
        {
            DataRow locationInfo = null;
            var @params = new Parameters(props.passport);
            var query = new Query(props.passport);

            var dts = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                var tableInfo = Navigation.GetTableInfo(props.paramss.tableName, conn);
                model.lblSubtitle = tableInfo["UserName"].ToString();
                using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE TrackingTable = 1", conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            locationInfo = dt.AsEnumerable().ElementAtOrDefault(0);
                    }
                }

                dts = await QueryTableObjectsInventoryCountOrData(false, locationInfo, tableInfo, props, model);

            }
            return dts;
        }
        private async Task<DataTable> QueryTableObjectsInventoryCountOrData(bool count, DataRow locationInfo, DataRow tableInfo, ReportingJsonModelReq props, ReportsModels model)
        {

            var dts = new DataTable();
            using (var conn = new SqlConnection(props.passport.ConnectionString))
            {
                await conn.OpenAsync();
                if (locationInfo is not null)
                {
                    string tTable = tableInfo["TableName"].ToString();
                    string lTable = locationInfo["TableName"].ToString();
                    string fileRoomOrder = Navigation.GetFileRoomOrderSQL(tableInfo, conn, tTable);
                    string sql = string.Empty;
                    if (count)
                    {
                        sql += "SELECT COUNT(*) ";
                    }
                    else
                    {
                        sql += "SELECT " + fileRoomOrder + " AS [File Room Order], '" + locationInfo["DescFieldPrefixOne"].ToString() + "' + " + Navigation.GetFieldAsVarCharSQL(locationInfo, locationInfo["DescFieldNameOne"].ToString(), conn, true) + " + '" + locationInfo["DescFieldPrefixTwo"].ToString() + "' + " + Navigation.GetFieldAsVarCharSQL(locationInfo, locationInfo["DescFieldNameTwo"].ToString(), conn, true) + " AS Locations, " + lTable + ".[" + locationInfo["InactiveLocationField"].ToString() + "] AS [Inactive Storage], " + lTable + ".[" + locationInfo["TrackingRequestableFieldName"].ToString() + "] AS Requestable, '" + tableInfo["UserName"].ToString() + "' AS [Object], ";




                        if (!string.IsNullOrEmpty(tableInfo["DescFieldNameOne"].ToString().Trim()))
                        {
                            if (!string.IsNullOrEmpty(tableInfo["DescFieldNameTwo"].ToString().Trim()))
                            {
                                sql += "'" + tableInfo["DescFieldPrefixOne"].ToString() + "' + " + Navigation.GetFieldAsVarCharSQL(tableInfo, tableInfo["DescFieldNameOne"].ToString(), conn, true) + " + '" + tableInfo["DescFieldPrefixTwo"].ToString() + "' + " + Navigation.GetFieldAsVarCharSQL(tableInfo, tableInfo["DescFieldNameTwo"].ToString(), conn, true) + " AS [" + tableInfo["UserName"].ToString() + " Description]";
                            }
                            else
                            {
                                sql += "'" + tableInfo["DescFieldPrefixOne"].ToString() + "' + " + Navigation.GetFieldAsVarCharSQL(tableInfo, tableInfo["DescFieldNameOne"].ToString(), conn, true) + " AS [" + tableInfo["UserName"].ToString() + " Description]";
                            }
                        }
                        else if (!string.IsNullOrEmpty(tableInfo["DescFieldNameTwo"].ToString().Trim()))
                        {
                            sql += "'" + tableInfo["DescFieldPrefixTwo"].ToString() + "' + " + Navigation.GetFieldAsVarCharSQL(tableInfo, tableInfo["DescFieldNameTwo"].ToString(), conn, true) + " AS [" + tableInfo["UserName"].ToString() + " Description]";
                        }
                        else
                        {
                            sql += "'Configure Display Fields' AS [" + tableInfo["UserName"].ToString() + " Description]";
                        }
                    }

                    sql += " FROM " + locationInfo["TableName"].ToString() + ", " + tableInfo["TableName"].ToString() + ", TrackingStatus ts WHERE ts.TrackedTable = '" + tTable + "' AND ";

                    if (Navigation.FieldIsAString(tableInfo, conn))
                    {
                        sql += "REPLICATE('0', 30 - LEN(" + tableInfo["IdFieldName"].ToString() + ")) + " + tableInfo["IdFieldName"].ToString() + " = ts.TrackedTableId AND ";
                    }
                    else
                    {
                        sql += tableInfo["IdFieldName"].ToString() + " = ts.TrackedTableId AND ";
                    }
                    if (Navigation.FieldIsAString(locationInfo, conn))
                    {
                        sql += "REPLICATE('0', 30 - LEN(" + locationInfo["IdFieldName"].ToString() + ")) + " + locationInfo["IdFieldName"].ToString() + " = ts.[" + locationInfo["TrackingStatusFieldName"].ToString() + "]";
                    }
                    else
                    {
                        sql += locationInfo["IdFieldName"].ToString() + " = ts.[" + locationInfo["TrackingStatusFieldName"].ToString() + "]";
                    }

                    if (!count)
                    {
                        string orderClause = " ORDER BY " + fileRoomOrder;
                        sql += orderClause;

                        // NOTE: query will through exception if does not include ORDER BY
                        sql += Query.QueryPaging(props.paramss.pageNumber, model.Paging.PerPageRecord);
                    }

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dts);
                        }
                    }
                }
            }

            return dts;
        }
        private async Task ObjectsOut(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the items out report";
                return;
            }
            model.lblTitle = "Current Items Out Report";
            model.lblSubtitle = "All Items Out";
            ObjectsOut_Header(model);
            await ObjectsOut_Rows(model, props);
        }
        private async Task ObjectsOut_Rows(ReportsModels model, ReportingJsonModelReq props)
        {
            var @params = new Parameters(props.passport);
            @params.Paged = true;
            @params.PageIndex = props.paramss.pageNumber;
            Dictionary<string, string> argidsByTable = null;
            var lst = await Tracking.GetCurrentItemsOutReportListAsync(@params, props.passport, model.Paging.PerPageRecord, idsByTable: argidsByTable);
            if (lst is null)
                return;
            foreach (TrackingTransaction trk in lst)
            {
                var cell = new List<string>();
                string tdName = string.Empty;
                string tdMail = string.Empty;
                string tdPhone = string.Empty;
                foreach (var cont in trk.Containers)
                {
                    // content = String.Format("{0} {1} {2}", cont.Name, con)
                    tdName += cont.Name;
                    tdMail += cont.MailStop;
                    tdPhone += cont.Phone;
                }

                cell.Add(tdName);
                cell.Add(tdPhone);
                cell.Add(tdMail);
                cell.Add(trk.Type);
                cell.Add(Navigation.GetItemName(trk.Type, trk.ID, props.passport));
                cell.Add(CommonFunctions.ToClientDateFormats(trk.TransactionDate));
                model.ListOfRows.Add(cell);
            }
        }
        private void ObjectsOut_Header(ReportsModels model)
        {
            model.ListOfHeader.Add("Employee");
            model.ListOfHeader.Add("Phone");
            model.ListOfHeader.Add("Mail Stop:");
            model.ListOfHeader.Add("Item_Type");
            model.ListOfHeader.Add("Description:");
            model.ListOfHeader.Add("Tran_Date");
        }
        private async Task PastDueTrackableItemsReport(ReportsModels model, ReportingJsonModelReq props)
        {
            if (!props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                model.DisplayNotAuthorized = "the past due report";
                return;
            }
            model.PageTitle = "Past Due Trackable Items Report";
            model.lblTitle = "Past Due Trackable Items Report";
            model.lblSubtitle = "As of" + " " + DateTime.Parse(model.dateFromTxt.ToShortDateString()).ToString(props.DateFormat);
            model.lblReportDate = string.Format("Print Date : {0}", DateTime.Parse(model.dateFromTxt.ToShortDateString()).ToString(props.DateFormat));
            PastDueTrackableItems_Headers(model);
            await PastDueTrackableItems_Rows(model, props);
        }
        private void PastDueTrackableItems_Headers(ReportsModels model)
        {
            model.ListOfHeader.Add("Date Due");
            model.ListOfHeader.Add("Item_Type");
            model.ListOfHeader.Add("Description:");
            model.ListOfHeader.Add("Location");
            model.ListOfHeader.Add("Tran_Date");
            model.ListOfHeader.Add("Scan_Operator");
            model.ListOfHeader.Add("Authorization");
        }
        private async Task PastDueTrackableItems_Rows(ReportsModels model, ReportingJsonModelReq props)
        {
            var _trackingTables = await Tracking.GetTrackingContainerTypesAsync(props.passport.ConnectionString);
            var _trackedTables = Tracking.GetTrackedTables(props.passport);
            // Dim dateFromTxt As Date = DateTime.Parse(txtDate.Text, CultureInfo)
            var param = new Parameters(props.passport);
            param.Paged = true;
            param.PageIndex = props.paramss.pageNumber;
            var lst = Tracking.GetPagedPastDueTrackablesList(model.dateFromTxt, props.passport, param, model.Paging.PerPageRecord, _trackingTables, _trackedTables, ref model.IdsByTable, ref model.Descriptions);

            var tablesInfo = Tracking.GetTrackedTableInfo(props.passport, _trackingTables, _trackedTables);
            if (model.Descriptions is null)
            {
                model.Descriptions = new Dictionary<string, DataTable>();
                foreach (DataRow tableInfo in tablesInfo.Rows)
                {
                    string ids = "";
                    if (model.IdsByTable.TryGetValue(tableInfo["TableName"].ToString().ToLower(), out ids)) // If we have ids, prepopulate; otherwise it'll have to get each one individually
                    {
                        model.Descriptions.Add(tableInfo["TableName"].ToString().ToLower(), Navigation.GetItemNames(tableInfo["TableName"].ToString(), props.passport, tableInfo, ids));
                    }
                }
            }
            model.TotalRowsCount = lst.Count.ToString();

            foreach (TrackingTransaction trk in lst)
            {
                var cell = new List<string>();
                cell.Add(DateTime.Parse(trk.DateDue.ToString()).ToString(props.DateFormat));
                cell.Add(trk.Type);
                var itemNameInfo = tablesInfo.Select("TableName='" + trk.Type + "'");

                if (itemNameInfo.Count() > 0)
                {
                    var itemNames = new DataTable();
                    if (model.Descriptions.TryGetValue(trk.Type.ToLower(), out itemNames))
                    {
                        var itemName = itemNames.Select("id='" + trk.ID.Replace("'", "''") + "'");
                        if (itemName.Count() > 0)
                        {
                            cell.Add(Navigation.ItemNamesRowToItemName(itemName[0], itemNameInfo[0], props.passport, trk.ID));
                        }
                        else
                        {
                            cell.Add(Navigation.GetItemName(trk.Type, trk.ID, props.passport, itemNameInfo[0]));
                        }
                    }
                    else
                    {
                        cell.Add(Navigation.GetItemName(trk.Type, trk.ID, props.passport, itemNameInfo[0]));
                    }
                }
                else
                {
                    cell.Add(Navigation.GetItemName(trk.Type, trk.ID, props.passport));
                }
                string location = "";
                foreach (var cont in trk.Containers)
                    location += string.Format("{0}: ", cont.Type);
                foreach (var cont in trk.Containers)
                    location += string.Format(" {0}", cont.Name);
                cell.Add(location);
                cell.Add(CommonFunctions.ToClientDateFormats(trk.TransactionDate));
                cell.Add(trk.UserName);
                cell.Add("");
                model.ListOfRows.Add(cell);
            }
        }
        private async Task AuditQuery(UIproperties UIparam, bool ispaging, AuditReportSearch model)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(UIparam.passport.ConnectionString))
            {
                await conn.OpenAsync();
                string query = BuildSqlQuery(UIparam, ispaging, conn, model).ToString();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (ispaging)
            {
                model.Paging.PageNumber = UIparam.PageNumber;
                if (dt.Rows.Count > 0)
                {
                    ExecutePaging(Convert.ToInt32(dt.Rows[0][0]), model);
                }
                else
                {
                    ExecutePaging(0, model);
                }
            }
            else
            {
                GenerateData(dt, model);
                GenerateHeader(model);
            }

        }
        private void GenerateHeader(AuditReportSearch model)
        {
            model.ListOfHeader.Add("Table");
            model.ListOfHeader.Add("Object Description");
            model.ListOfHeader.Add("Date/Time");
            model.ListOfHeader.Add("User");
            model.ListOfHeader.Add("Function");
            model.ListOfHeader.Add("Action");
            model.ListOfHeader.Add("Data Before");
            model.ListOfHeader.Add("Data After");
            model.ListOfHeader.Add("Domain");
            model.ListOfHeader.Add("Network User Name");
            model.ListOfHeader.Add("Computer Name");
            model.ListOfHeader.Add("IP");
            model.ListOfHeader.Add("MAC");
        }
        private void GenerateData(DataTable dt, AuditReportSearch model)
        {
            foreach (DataRow row in dt.Rows)
            {
                var cell = new ArrayList();
                cell.Add(row["TableName"].ToString());
                cell.Add(row["Object Description"].ToString());
                cell.Add(row["AccessDateTime"].ToString());
                cell.Add(row["AuditOperatorsId"].ToString());
                cell.Add(row["Module"].ToString());
                cell.Add(row["Action"].ToString());
                cell.Add(row["DataBefore"].ToString());
                cell.Add(row["DataAfter"].ToString());
                cell.Add(row["Domain"].ToString());
                cell.Add(row["NetworkLoginName"].ToString());
                cell.Add(row["ComputerName"].ToString());
                cell.Add(row["IP"].ToString());
                cell.Add(row["MacAddress"].ToString());
                model.ListOfRows.Add(cell);
            }
        }
        private void ExecutePaging(int totalRecord, AuditReportSearch model)
        {
            model.Paging.TotalRecord = totalRecord;
            if (model.Paging.TotalRecord > 0)
            {
                if (model.Paging.TotalRecord / (double)model.Paging.PerPageRecord > 0d & model.Paging.TotalRecord / (double)model.Paging.PerPageRecord < 1d)
                {
                    model.Paging.TotalPage = 1;
                }
                else if (model.Paging.TotalRecord % model.Paging.PerPageRecord == 0)
                {
                    model.Paging.TotalPage = (int)Math.Round(model.Paging.TotalRecord / (double)model.Paging.PerPageRecord);
                }
                else
                {
                    int tp = (int)Math.Round(Conversion.Int(model.Paging.TotalRecord / (double)model.Paging.PerPageRecord));
                    model.Paging.TotalPage = tp + 1;
                }
            }
        }

        private void ExecutePagingQueryCount(int totalRecord, ReportsModels model, int pagenumber)
        {
            model.Paging.TotalRecord = totalRecord;
            model.Paging.PageNumber = pagenumber;
            if (model.Paging.TotalRecord > 0)
            {
                if (model.Paging.TotalRecord / (double)model.Paging.PerPageRecord > 0d & model.Paging.TotalRecord / (double)model.Paging.PerPageRecord < 1d)
                {
                    model.Paging.TotalPage = 1;
                }
                else if (model.Paging.TotalRecord % model.Paging.PerPageRecord == 0)
                {
                    model.Paging.TotalPage = (int)Math.Round(model.Paging.TotalRecord / (double)model.Paging.PerPageRecord);
                }
                else
                {
                    int tp = (int)Math.Round(Conversion.Int(model.Paging.TotalRecord / (double)model.Paging.PerPageRecord));
                    model.Paging.TotalPage = tp + 1;
                }
            }
        }
        private StringBuilder BuildSqlQuery(UIproperties UIparam, bool ispaging, SqlConnection conn, AuditReportSearch model)
        {
            var param = new Parameters(UIparam.passport);

            try
            {
                param.PageIndex = Convert.ToInt32(UIparam.PageNumber);
            }
            catch (Exception)
            {
                param.PageIndex = 1;
            }

            // get subtitle
            GetSubTitle(UIparam, conn, model);
            // format the date before sending to query

            UIparam.StartDate = UIparam.StartDate + " 00:00:00";
            UIparam.EndDate = UIparam.EndDate + " 23:59:59";
            // build string objective
            var sqlQuery = new StringBuilder();
            var sqlWrapper = new StringBuilder();
            string sqlId = string.Empty;
            sqlQuery.Append("Select * START_FROM_TOKEN (");
            sqlId = Getid(UIparam, conn);
            if (UIparam.AddEditDelete)
            {
                if (!UIparam.ChildTable)
                {
                    ChkChildTableFalse(UIparam, sqlQuery, sqlId, ispaging);
                }
                else
                {
                    ChkChildTableTrue(UIparam, sqlQuery, sqlId, ispaging);
                }

                if (UIparam.UserDDLId != "-1")
                    sqlQuery.Append(Environment.NewLine + string.Format(" AND (SLAuditUpdates.[OperatorsId] = '{0}') ", UIparam.UserName));
                sqlQuery.Append(Environment.NewLine);
            }
            if (UIparam.SuccessLogin)
            {
                ChkSuccessLoginTrue(UIparam, sqlQuery, sqlId, ispaging);
            }
            if (UIparam.ConfDataAccess)
            {
                ChkConfDataAccessTrue(UIparam, sqlQuery, sqlId, ispaging);
            }
            if (UIparam.FailedLogin)
            {
                ChkloginFailedTrue(UIparam, sqlQuery, sqlId, ispaging);
            }
            EndSqlWrapper(UIparam, sqlQuery, sqlWrapper, param, ispaging, model);
            return sqlWrapper;
        }
        private void EndSqlWrapper(UIproperties UIparam, StringBuilder sqlQuery, StringBuilder sqlWrapper, Parameters param, bool count, AuditReportSearch model)
        {

            if (count)
            {
                sqlQuery.Append(") AS tmp");
                sqlWrapper.Clear().Append(sqlQuery.Replace("* START_FROM_TOKEN", "sum(Total) FROM"));
            }
            else
            {
                sqlQuery.Append(") AS tmp");
                sqlWrapper.Append("DECLARE @cnt INT, @Total INT, @tQuery NVARCHAR(MAX), @tOD VARCHAR(MAX)");
                sqlWrapper.Append(Environment.NewLine);
                string orderClause = " ORDER BY [TableName], TableId, AccessDateTime";

                if (!UIparam.AddEditDelete & !UIparam.SuccessLogin & !UIparam.ConfDataAccess & !UIparam.ChildTable)
                    orderClause = " ORDER BY TableId, AccessDateTime";
                if (UIparam.AddEditDelete | UIparam.SuccessLogin | UIparam.ConfDataAccess | UIparam.FailedLogin | UIparam.ChildTable)
                {
                    if (param is null || !param.Paged)
                    {
                        sqlWrapper.Append(string.Format("SELECT (ROW_NUMBER() OVER ({1})) AS RowNum, overall_count = COUNT(*) OVER(), * INTO #testTemp FROM ({0}) Union_Table ", sqlQuery.Replace(" START_FROM_TOKEN ", " FROM "), orderClause));
                    }
                    else
                    {
                        sqlWrapper.Append(Pagify(sqlQuery.ToString(), orderClause, UIparam).Replace(" AS RowNum", " AS RowNum, overall_count = COUNT(*) OVER()").Replace("* FROM ", "* INTO #testTemp FROM ").Replace(" START_FROM_TOKEN ", " FROM "));
                    }
                }

                if (param is null || !param.Paged)
                {
                    sqlWrapper.Append(Environment.NewLine + "SELECT @cnt = 1, @Total = overall_count FROM #testTemp" + Environment.NewLine);
                }
                else
                {
                    sqlWrapper.Append(Environment.NewLine + "SELECT @cnt = " + ((param.PageIndex - 1) * param.RequestedRows).ToString() + ", @Total = " + (param.PageIndex * param.RequestedRows).ToString() + " FROM #testTemp" + Environment.NewLine);
                }

                sqlWrapper.Append(Environment.NewLine);
                sqlWrapper.Append(" WHILE (@cnt <= @Total) BEGIN " + Environment.NewLine + "     SELECT @tQuery = [Object Description], @tOD='' FROM #testTemp WHERE RowNum = @cnt" + Environment.NewLine + "     IF (@tQuery <> '') BEGIN " + Environment.NewLine + "         EXEC sp_executesql @tQuery, N'@rVal NVARCHAR(MAX) OUT', @tOD OUT " + Environment.NewLine + "         UPDATE #testTemp SET [Object Description] = (CASE WHEN @tOD = '' OR @tOD IS NULL THEN CAST(CONVERT(INT, TableId) AS NVARCHAR(MAX)) ELSE @tOD END) WHERE RowNum = @cnt " + Environment.NewLine + "     END " + Environment.NewLine + "     SELECT @cnt = @cnt + 1" + Environment.NewLine + " END " + Environment.NewLine + " SELECT * FROM #testTemp " + orderClause + Query.QueryPaging(param.PageIndex, model.Paging.PerPageRecord) + Environment.NewLine + " DROP TABLE #testTemp");


            }
        }
        private string Pagify(string sql, string orderClause, UIproperties prop)
        {
            var @params = new Parameters(prop.passport);
            string pageClause = "SELECT TOP ";
            if (@params.RequestedRows <= 0)
                @params.RequestedRows = 250;
            pageClause += @params.RequestedRows.ToString() + " * FROM (SELECT ROW_NUMBER() OVER (" + orderClause.Trim() + ") AS RowNum,";
            pageClause = Strings.Replace(sql, "SELECT", pageClause, 1, 1, CompareMethod.Text);
            pageClause += ") AS PagedResult WHERE RowNum > " + ((@params.PageIndex - 1) * @params.RequestedRows).ToString() + " AND RowNum <= " + (@params.PageIndex * @params.RequestedRows).ToString();
            pageClause += " ORDER BY PagedResult.RowNum";
            return pageClause;
        }
        private void ChkloginFailedTrue(UIproperties UIparam, StringBuilder sqlQuery, string sqlId, bool ispaging)
        {
            if (UIparam.AddEditDelete | UIparam.SuccessLogin | UIparam.ConfDataAccess)
                sqlQuery.Append(Environment.NewLine + " UNION ALL " + Environment.NewLine);

            if (ispaging)
            {
                sqlQuery.Append(string.Format("SELECT COUNT(*) As Total FROM SLAuditFailedLogins LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditFailedLogins.OperatorsId " + Environment.NewLine + "WHERE (([LoginDateTime] >= '{0}') AND ([LoginDateTime] <= '{1}'))", UIparam.StartDate, UIparam.EndDate));
            }
            else
            {
                sqlQuery.Append(string.Format("SELECT [Id], '' AS 'TableName', '' AS 'Object Description', " + Environment.NewLine + "'' AS [TableId], ISNULL(SecureUser.DisplayName, OperatorsId) AS AuditOperatorsId, CASE WHEN NetworkLoginName= '' THEN '[unknown user]' ELSE NetworkLoginName END AS [NetworkLoginName], " + Environment.NewLine + "[Domain], [ComputerName], [MacAddress], [IP], [LoginDateTime] AS [AccessDateTime], " + Environment.NewLine + "'' AS [Module], '" + "Failed Login" + ": ' + [ReasonForFailure] AS [Action], CASE WHEN SecureUser.DisplayName IS NULL THEN [TextEntered] ELSE '' END AS [DataBefore],'' AS [DataAfter] " + Environment.NewLine + "FROM SLAuditFailedLogins LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditFailedLogins.OperatorsId " + Environment.NewLine + "WHERE (([LoginDateTime] >= '{0}') AND ([LoginDateTime] <= '{1}'))", UIparam.StartDate, UIparam.EndDate));
            }

            if (UIparam.UserDDLId != "-1")
                sqlQuery.Append(Environment.NewLine + string.Format(" AND (SLAuditFailedLogins.[OperatorsId] = '{0}') ", UIparam.UserName));
            sqlQuery.Append(Environment.NewLine);
        }
        private void ChkConfDataAccessTrue(UIproperties UIparam, StringBuilder sqlQuery, string sqlId, bool ispaging)
        {
            if (UIparam.AddEditDelete | UIparam.SuccessLogin)
                sqlQuery.Append("UNION ALL " + Environment.NewLine);

            if (ispaging)
            {
                sqlQuery.Append(string.Format("SELECT COUNT(*) As Total FROM SLAuditConfData LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditConfData.OperatorsId " + Environment.NewLine + "WHERE {2} " + Environment.NewLine + "(([AccessDateTime] >= '{0}') AND ([AccessDateTime] <= '{1}'))", UIparam.StartDate, UIparam.EndDate, string.Format(sqlId, "SLAuditConfData")));
            }
            else
            {
                sqlQuery.Append(string.Format("SELECT [Id], [TableName], (dbo.fnGetObjectDescription([TableName],[TableId])) AS 'Object Description', " + Environment.NewLine + "[TableId], ISNULL(SecureUser.DisplayName, OperatorsId) AS AuditOperatorsId, CASE WHEN NetworkLoginName='' THEN '[unknown user]' ELSE NetworkLoginName END AS [NetworkLoginName], " + Environment.NewLine + "[Domain], [ComputerName], [MacAddress], [IP], [AccessDateTime], " + Environment.NewLine + "[Module], 'Confidential Data Access' AS [Action], '' AS [DataBefore], '' AS [DataAfter]  " + Environment.NewLine + "FROM SLAuditConfData LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditConfData.OperatorsId " + Environment.NewLine + "WHERE {2} " + Environment.NewLine + "(([AccessDateTime] >= '{0}') AND ([AccessDateTime] <= '{1}'))", UIparam.StartDate, UIparam.EndDate, string.Format(sqlId, "SLAuditConfData")));
            }


            if (!(UIparam.ObjectId == "All"))
            {
                if (ObjectIdIsSingleTable(UIparam.ObjectId))
                    sqlQuery.Append(Environment.NewLine + string.Format(" AND ([TableName] = '{0}') ", GetTableNameFromObjectId(UIparam.ObjectId)));
            }
            if (UIparam.UserDDLId != "-1")
                sqlQuery.Append(Environment.NewLine + string.Format(" AND (SLAuditConfData.[OperatorsId] = '{0}') ", UIparam.UserName));
            sqlQuery.Append(Environment.NewLine);
        }
        private void ChkSuccessLoginTrue(UIproperties UIparam, StringBuilder sqlQuery, string sqlId, bool ispaging)
        {
            if (UIparam.AddEditDelete)
                sqlQuery.Append("UNION ALL " + Environment.NewLine);
            if (ispaging)
            {
                sqlQuery.Append(string.Format("SELECT COUNT(*) As Total FROM SLAuditLogins LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditLogins.OperatorsId  " + Environment.NewLine + "WHERE (([LoginDateTime] >= '{0}') AND ([LoginDateTime] <= '{1}'))", UIparam.StartDate, UIparam.EndDate));
            }
            else
            {
                sqlQuery.Append(string.Format("SELECT [Id], '' AS [TableName], '' AS 'Object Description', '' AS [TableId], ISNULL(SecureUser.DisplayName, OperatorsId) AS AuditOperatorsId, " + Environment.NewLine + "CASE WHEN NetworkLoginName = '' THEN '[unknown user]' ELSE NetworkLoginName END AS [NetworkLoginName], [Domain], [ComputerName], [MacAddress], [IP], " + Environment.NewLine + "[LoginDateTime] AS [AccessDateTime], '' AS [Module], 'Successful Login' AS [Action], '' AS [DataBefore], '' AS [DataAfter]  " + Environment.NewLine + "FROM SLAuditLogins LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditLogins.OperatorsId  " + Environment.NewLine + "WHERE (([LoginDateTime] >= '{0}') AND ([LoginDateTime] <= '{1}'))", UIparam.StartDate, UIparam.EndDate));
            }

            if (UIparam.UserDDLId != "-1")
                sqlQuery.Append(Environment.NewLine + string.Format(" AND (SLAuditLogins.[OperatorsId] = '{0}') ", UIparam.UserName));
            sqlQuery.Append(Environment.NewLine);

        }
        private void ChkChildTableFalse(UIproperties UIparam, StringBuilder sqlQuery, string sqlId, bool ispaging)
        {
            if (ispaging)
            {
                sqlQuery.Append(string.Format("SELECT COUNT(*) As Total FROM SLAuditUpdates LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditUpdates.OperatorsId " + Environment.NewLine + "WHERE {2}" + Environment.NewLine + "(([UpdateDateTime] >= '{0}') AND ([UpdateDateTime] <= '{1}')) ", UIparam.StartDate, UIparam.EndDate, string.Format(sqlId, "SLAuditUpdates")));
            }
            else
            {
                sqlQuery.Append(string.Format("SELECT [Id], [TableName], (dbo.fnGetObjectDescription([TableName],[TableId])) AS 'Object Description', " + Environment.NewLine + "[TableId], ISNULL(SecureUser.DisplayName, OperatorsId) AS AuditOperatorsId, CASE WHEN NetworkLoginName = '' THEN '[unknown user]' ELSE NetworkLoginName END AS [NetworkLoginName], " + Environment.NewLine + "[Domain], [ComputerName], [MacAddress], [IP], [UpdateDateTime] AS [AccessDateTime], " + Environment.NewLine + "[Module], [Action], CAST([DataBefore] AS VARCHAR(8000)) AS DataBefore, CAST([DataAfter] AS VARCHAR(8000)) AS DataAfter " + Environment.NewLine + "FROM SLAuditUpdates LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditUpdates.OperatorsId " + Environment.NewLine + "WHERE {2}" + Environment.NewLine + "(([UpdateDateTime] >= '{0}') AND ([UpdateDateTime] <= '{1}')) ", UIparam.StartDate, UIparam.EndDate, string.Format(sqlId, "SLAuditUpdates")));
            }

            if (ObjectIdIsSingleTable(UIparam.ObjectId))
            {
                if (!(UIparam.ObjectId == "All"))
                {
                    sqlQuery.Append(Environment.NewLine + string.Format(" AND (SLAuditUpdates.TableName = '{0}') ", GetTableNameFromObjectId(UIparam.ObjectId)));
                }
            }
        }
        private void ChkChildTableTrue(UIproperties UIparam, StringBuilder sqlQuery, string sqlId, bool ispaging)
        {
            if (ispaging)
            {
                sqlQuery.Append(string.Format("SELECT COUNT(*) As Total FROM SLAuditUpdChildren INNER JOIN SLAuditUpdates ON SLAuditUpdChildren.SLAuditUpdatesId = SLAuditUpdates.Id  " + Environment.NewLine + "LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditUpdates.OperatorsId  " + Environment.NewLine + "WHERE {2}" + Environment.NewLine + "(([UpdateDateTime] >= '{0}') AND ([UpdateDateTime] <= '{1}')) ", UIparam.StartDate, UIparam.EndDate, string.Format(sqlId, "SLAuditUpdChildren")));
            }
            else
            {
                sqlQuery.Append(string.Format("SELECT SLAuditUpdates.[Id], SLAuditUpdates.[TableName], (dbo.fnGetObjectDescription(SLAuditUpdates.[TableName], SLAuditUpdates.[TableId])) AS 'Object Description', " + Environment.NewLine + "SLAuditUpdates.[TableId], ISNULL(SecureUser.DisplayName, OperatorsId) AS AuditOperatorsId, CASE WHEN NetworkLoginName='' THEN '[unknown user]' ELSE NetworkLoginName END AS [NetworkLoginName], " + Environment.NewLine + "[Domain], [ComputerName], [MacAddress], [IP], [UpdateDateTime] AS [AccessDateTime], " + Environment.NewLine + "[Module], [Action], CAST([DataBefore] AS VARCHAR(8000)) AS DataBefore, CAST([DataAfter] AS VARCHAR(8000)) AS DataAfter " + Environment.NewLine + "FROM SLAuditUpdChildren INNER JOIN SLAuditUpdates ON SLAuditUpdChildren.SLAuditUpdatesId = SLAuditUpdates.Id  " + Environment.NewLine + "LEFT JOIN SecureUser ON SecureUser.UserName = SLAuditUpdates.OperatorsId  " + Environment.NewLine + "WHERE {2}" + Environment.NewLine + "(([UpdateDateTime] >= '{0}') AND ([UpdateDateTime] <= '{1}')) ", UIparam.StartDate, UIparam.EndDate, string.Format(sqlId, "SLAuditUpdChildren")));

            }

            if (ObjectIdIsSingleTable(UIparam.ObjectId))
            {
                if (!(UIparam.ObjectId == "All"))
                {
                    sqlQuery.Append(Environment.NewLine + string.Format(" AND (SLAuditUpdChildren.TableName = '{0}') ", GetTableNameFromObjectId(UIparam.ObjectId)));
                }
            }
        }
        private void GetSubTitle(UIproperties UIparam, SqlConnection conn, AuditReportSearch model)
        {
            if (string.IsNullOrEmpty(UIparam.Id))
                UIparam.Id = "";
            string SubTitle = string.Empty;
            SubTitle = "Audit Report for " + UIparam.UserName;
            if (UIparam.ObjectName.Trim() != "Select Object")
            {
                // Get table primary key and DescFieldNameTwo field
                var dsTableName = new DataSet();
                string strDescFieldNameTwo = string.Empty;
                string strPKField = string.Empty;


                using (var cmd = new SqlCommand(string.Format("SELECT (CASE WHEN t.IdFieldName IS NULL THEN '' ELSE (SUBSTRING(t.IdFieldName, CHARINDEX('.', t.IdFieldName) + 1, LEN(t.IdFieldName))) END) AS PKField " + ", t.DescFieldNameTwo FROM Tables t WHERE t.TableName = '{0}'", UIparam.ObjectName.Trim()), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dsTableName);
                    }
                }

                if (dsTableName.Tables.Count > 0)
                {
                    if (dsTableName.Tables[0].Rows.Count > 0)
                    {
                        if (!(dsTableName.Tables[0].Rows[0]["DescFieldNameTwo"] is DBNull))
                            strDescFieldNameTwo = Convert.ToString(dsTableName.Tables[0].Rows[0]["DescFieldNameTwo"]);
                        if (!(dsTableName.Tables[0].Rows[0]["PKField"] is DBNull))
                            strPKField = Convert.ToString(dsTableName.Tables[0].Rows[0]["PKField"]);
                    }
                }
                // Get table DescFieldNameTwo field value
                var dsFieldTableName = new DataSet();
                string strDescFieldNameTwoVal = string.Empty;

                if (!string.IsNullOrWhiteSpace(strDescFieldNameTwo))
                {

                    using (var cmd = new SqlCommand(string.Format("SELECT [{0}] AS DescFieldNameTwo FROM [{1}] WHERE [{2}] = '{3}'", strDescFieldNameTwo, UIparam.ObjectName.Trim(), strPKField, UIparam.Id), conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dsFieldTableName);
                        }
                    }

                    if (dsFieldTableName.Tables.Count > 0)
                    {
                        if (dsFieldTableName.Tables[0].Rows.Count > 0 && !(dsFieldTableName.Tables[0].Rows[0]["DescFieldNameTwo"] is DBNull))
                        {
                            strDescFieldNameTwoVal = Convert.ToString(dsFieldTableName.Tables[0].Rows[0]["DescFieldNameTwo"]);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(strDescFieldNameTwoVal))
                    strDescFieldNameTwoVal = " - " + strDescFieldNameTwoVal;
                SubTitle += " and " + UIparam.ObjectName + ":" + UIparam.Id + strDescFieldNameTwoVal;
            }

            SubTitle += " from " + UIparam.StartDate + " thru " + UIparam.EndDate;
            model.SubTitle = SubTitle;
        }
        private string Getid(UIproperties UIparam, SqlConnection conn)
        {
            string sqlId = string.Empty;
            if (!string.IsNullOrEmpty(UIparam.Id))
            {
                if (UIparam.Id.Trim().Length > 0 && ObjectIdIsSingleTable(UIparam.ObjectId))
                {
                    if (Navigation.FieldIsAString(GetTableNameFromObjectId(UIparam.ObjectId), conn))
                    {
                        sqlId = string.Format(" ({0}.TableId = '{1}') AND ", "{0}", UIparam.Id.Trim());
                    }
                    else
                    {
                        sqlId = string.Format(" ({0}.TableId = RIGHT('000000000000000000000000000000' + CAST({1} AS VARCHAR(30)), 30)) AND ", "{0}", UIparam.Id.Trim());
                    }
                }
            }
            return sqlId;
        }
        private string GetTableNameFromObjectId(string ObjectId)
        {
            try
            {
                return ObjectId.Split('|')[0].ToString();
            }
            catch (Exception)
            {
                return ObjectId;
            }
        }
        private bool ObjectIdIsSingleTable(string ObjectId)
        {
            try
            {
                return string.Compare(ObjectId.Split('|')[0].ToString(), "Select Table", true) != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool CheckPermssionsReportTab(UserInterfaceProps props)
        {
            if (props.passport.CheckPermission(" Auditing", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                return true;
            if (props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                return true;
            if (props.passport.CheckPermission(" Retention", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                return true;
            if (props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                return true;
            return false;
        }
        private async Task CreateHtml(StringBuilder sbMenu, UserInterfaceProps props, ReportingMenu model)
        {
            sbMenu.Append(string.Format("<li><a href='#'>{0}</a>", "Custom Reports"));
            var customReports = Navigation.GetViewReports(props.passport);
            sbMenu.Append("<ul>");

            foreach (RecordsManage.ViewsRow report in customReports)
            {
                if (props.passport.CheckPermission(report.ViewName, SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                {
                    sbMenu.Append(string.Format("<li><a onClick=\"reports.LoadCustomReport(this,'{1}')\">{0}</a></li>", report.ViewName, report.Id));
                }
            }

            sbMenu.Append("</ul></li>");
            if (props.passport.CheckPermission(" Auditing", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                sbMenu.Append(string.Format("<li><a onClick=\"reports.LoadAuditReport(this)\">{0}</a>", "Audit Reports"));
            }
            if (props.passport.CheckPermission(" Tracking", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                sbMenu.Append(string.Format("<li><a href='#'>{0}</a>", "Tracking Reports"));
                sbMenu.Append("<ul>");
                sbMenu.Append(string.Format("<li><a id='FA{0}' onClick=\"reports.TrackableReport(this, 0, 1)\">{0}</a></li>", "Past Due Tracked Objects"));
                sbMenu.Append(string.Format("<li><a id='FA{0}' onClick=\"reports.TrackableReport(this, 1, 1)\">{0}</a></li>", "Objects Out"));
                sbMenu.Append(string.Format("<li><a href='#'\">{0}</a>", "Objects Inventory"));
                sbMenu.Append("<ul>");

                var TtableList = await Tracking.GetTrackableTablesAsync(props.passport);
                foreach (DataRow row in TtableList.Rows)
                    sbMenu.Append(string.Format("<li><a id='FA{0}' onClick=\"reports.TrackableReport(this,'{1}',1)\">{0}</a></li>", row["UserName"].ToString(), row["TableName"].ToString()));

                sbMenu.Append("</ul></li>");
                sbMenu.Append("</ul></li>");
            }

            if (props.passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {

                sbMenu.Append(string.Format("<li><a href='#'>{0}</a>", "Requestor Reports"));
                sbMenu.Append("<ul>");

                sbMenu.Append(string.Format("<li><a id=\"newrequest\" onClick=\"reports.RequestorReport(this, 'new', 1)\">{0}</a></li>", "New Requests"));

                // sbMenu.Append(String.Format("<li><a  onClick=""reports.RequestorReport(this, 'newbatch', 1)"">{0}</a></li>", Languages.Translation("mnuWorkGroupMenuControlNewBatch")))

                sbMenu.Append(string.Format("<li><a  onClick=\"reports.RequestorReport(this, 'pulllist', 1)\">{0}</a></li>", "Pull Lists"));

                sbMenu.Append(string.Format("<li><a id=\"exceptions\" onClick=\"reports.RequestorReport(this, 'exception', 1)\">{0}</a></li>", "Exceptions"));

                sbMenu.Append(string.Format("<li><a  onClick=\"reports.RequestorReport(this, 'inprocess', 1)\">{0}</a></li>", "In Process"));

                sbMenu.Append(string.Format("<li><a  onClick=\"reports.RequestorReport(this, 'waitlist', 1)\">{0}</a></li>", "Wait List"));

                sbMenu.Append("</ul></li>");
            }

            if (props.passport.CheckPermission(" Retention", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
            {
                bool turnOffCitation = Convert.ToBoolean(Navigation.GetSystemSetting("RetentionTurnOffCitations", props.passport));

                sbMenu.Append(string.Format("<li><a href='#'>{0}</a>", "Retention Reports"));
                sbMenu.Append("<ul>");

                // 'Adding permission
                if (props.passport.CheckPermission("Disposition", SecureObject.SecureObjectType.Retention, Permissions.Permission.Access))
                {
                    sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'finaldisposition', 1)\">{0}</a></li>", "Eligible for Final Disposition"));
                }
                // coninue from here. 

                sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'certifiedisposition', 1)\">{0}</a></li>", "Certificates of Disposition"));

                sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'inactivepulllist', 1)\">{0}</a></li>", "Inactive Pull Lists"));

                sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'inactivereport', 1)\">{0}</a></li>", "Inactive Records"));

                sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'recordonhold', 1)\">{0}</a></li>", "Records On Hold"));

                if (!turnOffCitation)
                {

                    sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'citations', 1)\">{0}</a></li>", "Citations"));

                    sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'citationwithretcode', 1)\">{0}</a></li>", "Citations with Retention Codes"));
                }


                sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'codes', 1)\">{0}</a></li>", "Retention Codes"));

                if (!turnOffCitation)
                {

                    sbMenu.Append(string.Format("<li><a onClick=\"retentionreport.RetentionReport(this, 'codewithcitations', 1)\">{0}</a></li>", "Retention Codes with Citations"));
                }

                sbMenu.Append("</ul></li>");
            }


            model.AccessMenu = sbMenu.ToString();
        }
    }
}
