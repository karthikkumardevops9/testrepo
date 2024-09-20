using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Properties;
using Smead.Security;

namespace MSRecordsEngine.RecordsManager
{

    public class Retention
    {
        public enum RetentionHoldTypes
        {
            None,
            RetentionHold,
            LegalHold
        }

        public enum FinalDisposition
        {
            /// <summary>
            /// Indicates what happens to the record when it reaches the end of its retention period.
            ///        None: Retention is not active. Nothing happens.
            ///     Archive: Records (including child records) are marked as archived, but no data or attachments are removed from the
            ///              system. Items are transferred to a ArchiveLocation. Can only be viewed by a user with security permissions
            ///              to review "archived" items.
            /// Destruction: Records (including child records) are marked as destroyed. All attachments are permanently deleted. The data
            ///              is preserved, but can only be viewed by a user with security permissions to review "destroyed" items.
            ///      Purged: Records (including child records) and attachments are permanently deleted. The information is unrecoverable
            ///              once purged.
            /// </summary>
            /// <remarks></remarks>
            None = 0,
            PermanentArchive = 1,
            Destruction = 2,
            Purge = 3
        }

        public static void UpdateInActiveFlag(Entities.Table table, Entities.SLRetentionCode PRetentionCode, Passport passport, bool ForceEndOfYear = false)
        {
            string sSQL = string.Empty;
            string sTableIdField;
            DateTime dDestructionDate;
            string fieldOpenDate = string.Empty;
            string fieldCreateDate = string.Empty;
            string fieldCloseDate = string.Empty;
            string fieldOtherDate = string.Empty;

            sTableIdField = Strings.Trim(table.TableName) + "_" + Strings.Trim(Navigation.MakeSimpleField(table.IdFieldName));
            sSQL = "SELECT [" + table.TableName + "].[" + Navigation.MakeSimpleField(table.IdFieldName) + "] AS " + sTableIdField + ", ";
            sSQL += " [" + table.TableName + "].[" + Navigation.MakeSimpleField(table.RetentionFieldName) + "], ";
            if (table.RetentionDateCreateField.Length > 0)
            {
                fieldCreateDate = Navigation.MakeSimpleField(table.RetentionDateCreateField);
                sSQL += " [" + table.TableName + "].[" + fieldCreateDate + "], ";
            }
            if (table.RetentionDateClosedField.Length > 0)
            {
                fieldCloseDate = Navigation.MakeSimpleField(table.RetentionDateClosedField);
                sSQL += " [" + table.TableName + "].[" + fieldCloseDate + "], ";
            }
            if (table.RetentionDateOpenedField.Length > 0)
            {
                fieldOpenDate = Navigation.MakeSimpleField(table.RetentionDateOpenedField);
                sSQL += " [" + table.TableName + "].[" + fieldOpenDate + "], ";
            }
            if (table.RetentionDateOtherField.Length > 0)
            {
                fieldOtherDate = Navigation.MakeSimpleField(table.RetentionDateOtherField);
                sSQL += " [" + table.TableName + "].[" + fieldOtherDate + "], ";
            }

            bool idFieldIsString = Navigation.FieldIsAString(table.TableName, passport);
            string sFormattedIdFieldName = "([" + table.TableName + "].[" + Navigation.MakeSimpleField(table.IdFieldName) + "] = [SLDestructCertItems].[TableId] AND " + "[SLDestructCertItems].[TableName] = '" + table.TableName + "') ";

            sSQL += " [SLRetentionCodes].* FROM (([" + table.TableName + "] INNER JOIN [SLRetentionCodes] ON [" + table.TableName + "].[" + Navigation.MakeSimpleField(table.RetentionFieldName) + "] = SLRetentionCodes.Id))" + " WHERE ([" + table.TableName + "].[RetentionCodesId] = '" + PRetentionCode.Id + "')";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sSQL, conn))
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
                                dDestructionDate = DateTime.MinValue;
                                try
                                {
                                    string eventType = row["RetentionEventType"].ToString();
                                    if (eventType.Trim().ToUpper() == "N/A")
                                    {
                                        eventType = row["InactivityEventType"].ToString();
                                    }

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
                                }
                                catch (Exception ex)
                                {
                                    Debug.Print(ex.Message);
                                }
                                DateTime inactiveDate;

                                if (ForceEndOfYear)
                                {
                                    inactiveDate = dDestructionDate.AddYears(Convert.ToInt32(PRetentionCode.RetentionPeriodTotal));
                                    inactiveDate = new DateTime(inactiveDate.Year, 12, 31);
                                }
                                else
                                {
                                    inactiveDate = dDestructionDate.AddYears(Convert.ToInt32(PRetentionCode.InactivityPeriod));
                                }
                                string inactivityFlag = inactiveDate.CompareTo(DateTime.Now) < 0 ? "1" : "0";

                                string id = row[sTableIdField].ToString();
                                if (!string.IsNullOrEmpty(id) && inactivityFlag == "0")
                                {
                                    var deleteRecord = new SqlCommand($"DELETE FROM SLDestructCertItems WHERE TableId = '{id}' AND TableName = '{table.TableName}'", passport.Connection());
                                    deleteRecord.ExecuteNonQuery();
                                }

                                if (!string.IsNullOrEmpty(id))
                                {
                                    Navigation.UpdateSingleField(table.TableName, id, "%slRetentionInactive", inactivityFlag, conn);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ProcessRetentionData(string tableName, string tableID, int destructionCertItemID, int destructionCertID, string newRetentionCode, string oldRetentionCode, bool onHold, string holdReason, RetentionHoldTypes holdType, DateTime snoozeUntil, DateTime scheduledDestructionDate, DateTime scheduledInactivityDate, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                onHold = onHold || snoozeUntil <= DateTime.Today && snoozeUntil > DateTime.MinValue;
                if (onHold & holdType == RetentionHoldTypes.None)
                {
                    holdType = RetentionHoldTypes.RetentionHold;
                }
                if (string.Compare(newRetentionCode, oldRetentionCode, true) != 0 && !onHold || !onHold && destructionCertID == 0)
                {
                    // If the code has been changed and the Destruction Cert Item is not on hold, then delete the record.
                    // If the Destruction Cert Item is not on hold and it does not have a parent id, then delete the record.
                    DeleteRetentionRecord(destructionCertItemID, destructionCertID, conn);
                }
                else if (snoozeUntil <= DateTime.Today & snoozeUntil > DateTime.MinValue)
                {
                    DeleteRetentionRecord(destructionCertItemID, destructionCertID, conn);
                }
                else
                {
                    SaveRetentionData(tableName, tableID, destructionCertItemID, destructionCertID, oldRetentionCode, newRetentionCode, onHold, holdReason, holdType, snoozeUntil, scheduledDestructionDate, scheduledInactivityDate, conn);
                }
                if (string.Compare(newRetentionCode, oldRetentionCode, true) != 0)
                    UpdateRetentionCodeInTableRecord(tableName, tableID, newRetentionCode, conn);
            }
        }

        public static void UpdateRetentionData(string tableName, string newRetentionCode, string oldRetentionCode, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                UpdateRetentionData(tableName, newRetentionCode, oldRetentionCode, conn);
            }
        }

        public static void UpdateRetentionData(string tableName, string newRetentionCode, string oldRetentionCode, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(string.Empty, conn))
            {
                // Undisposed items on hold need updated first to remove SLDestructionCertsId and update retention code
                cmd.CommandText = "UPDATE [SLDestructCertItems] SET [SLDestructionCertsId] = 0, [RetentionCode] = @newRetentionCode " + " WHERE [TableName] = @tableName AND [RetentionCode] = @oldRetentionCode " + " AND ([SLDestructionCertsId] IS NOT NULL AND [SLDestructionCertsId] <> 0) AND [DispositionDate] IS NULL " + " AND (([LegalHold] IS NOT NULL AND [LegalHold] <> 0) OR ([RetentionHold] IS NOT NULL AND [RetentionHold] <> 0))";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@newRetentionCode", newRetentionCode);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@oldRetentionCode", oldRetentionCode);
                cmd.ExecuteNonQuery();
                // Undisposed items that are NOT on hold but have a SLDestructionCertsId can just be deleted
                cmd.CommandText = "DELETE FROM [SLDestructCertItems] WHERE [TableName] = @tableName AND [RetentionCode] = @oldRetentionCode " + " AND ([SLDestructionCertsId] IS NOT NULL AND [SLDestructionCertsId] <> 0) AND [DispositionDate] IS NULL " + " AND (([LegalHold] IS NULL OR [LegalHold] = 0) AND ([RetentionHold] IS NULL OR [RetentionHold] = 0))";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@oldRetentionCode", oldRetentionCode);
                cmd.ExecuteNonQuery();
                // finally, delete any SLDestructionCerts that have no longer have any disposed or undisposed children
                cmd.CommandText = "DELETE FROM [SLDestructionCerts] WHERE NOT [Id] IN (SELECT c.Id FROM SLDestructionCerts c " + "                                                    INNER JOIN SLDestructCertItems i ON i.SLDestructionCertsId = c.Id " + "                                                    GROUP BY c.Id HAVING COUNT(*) > 0)";
                cmd.Parameters.Clear();
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateRetentionData(Parameters @params, List<FieldValue> data, Passport passport)
        {
            Retention.UpdateRetentionData(@params.TableInfo, @params.TableName, @params.KeyValue, data, passport);
        }

        public static void UpdateRetentionData(DataRow tableInfo, string tableName, string keyValue, List<FieldValue> data, Passport passport)
        {
            using (var conn = passport.StaticConnection())
            {
                UpdateRetentionData(tableInfo, tableName, keyValue, data, conn);
            }
        }

        public static void UpdateRetentionData(Parameters @params, List<FieldValue> data, SqlConnection conn)
        {
            Retention.UpdateRetentionData(@params.TableInfo, @params.TableName, @params.KeyValue, data, conn);
        }

        public static void UpdateRetentionData(DataRow tableRow, string tableName, string keyValue, List<FieldValue> data, SqlConnection conn)
        {
            if (!Navigation.CBoolean(tableRow, "RetentionPeriodActive") && !Navigation.CBoolean(tableRow, "RetentionInactivityActive"))
                return;

            var retentionDateChanged = default(bool);
            string newRetentionCode = string.Empty;

            foreach (FieldValue fld in data)
            {
                if (!string.IsNullOrWhiteSpace(fld.value.ToString()))
                {
                    if (string.Compare(fld.Field, tableRow["RetentionFieldName"].ToString()) == 0)
                    {
                        //here if we are not updating the retention filed in grid, so we wont be getting the value of the retention here
                        newRetentionCode = fld.value.ToString();
                        // vbNullChar below means that we are clearing the code (i.e. MAN1000 to blank).
                        // This is used below to indicate that we can delete the retention row
                        if (string.IsNullOrWhiteSpace(newRetentionCode))
                            newRetentionCode = Constants.vbNullChar;
                    }
                    else if (string.Compare(fld.Field, tableRow["RetentionDateOpenedField"].ToString()) == 0 || string.Compare(fld.Field, tableRow["RetentionDateCreateField"].ToString()) == 0 || string.Compare(fld.Field, tableRow["RetentionDateClosedField"].ToString()) == 0 || string.Compare(fld.Field, tableRow["RetentionDateOtherField"].ToString()) == 0)
                    {
                        if (Information.IsDate(fld.value))
                            retentionDateChanged = true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(newRetentionCode) && retentionDateChanged)
                    break;
            }

            // assign the retention code of the updated code to the newRetentionCode varaiable.
            string retentionFieldName = tableRow["RetentionFieldName"].ToString();
            DataRow dr = Navigation.GetSingleFieldValue(tableName, keyValue, retentionFieldName, conn);
            if(dr != null && dr[retentionFieldName] != null && dr[retentionFieldName].ToString().Length != 0)
                newRetentionCode = dr[retentionFieldName].ToString();

            if (string.IsNullOrWhiteSpace(newRetentionCode) && !retentionDateChanged)
                return;

            using (var cmd = new SqlCommand("SELECT * FROM [SLDestructCertItems] WHERE [DispositionDate] IS NULL AND [TableName] = @tableName AND [TableId] = @tableId", conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", keyValue);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var certs = new DataTable();
                    da.Fill(certs);

                    foreach (DataRow row in certs.Rows)
                    {
                        int certid = 0;

                        if (!(row["SLDestructionCertsId"] is DBNull))
                        {
                            try
                            {
                                certid = Conversions.ToInteger(row["SLDestructionCertsId"]);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                certid = 0;
                            }
                        }

                        if (!Navigation.CBoolean(row, "LegalHold") && !Navigation.CBoolean(row, "RetentionHold"))
                        {
                            DeleteRetentionRecord(Conversions.ToInteger(row["Id"]), certid, conn);
                        }
                        else if (string.Compare(newRetentionCode, Constants.vbNullChar) == 0)
                        {
                            // see comment above
                            DeleteRetentionRecord(Conversions.ToInteger(row["Id"]), certid, conn);
                        }
                        else if (!string.IsNullOrWhiteSpace(newRetentionCode))
                        {
                            cmd.CommandText = "UPDATE [SLDestructCertItems] SET [RetentionCode] = @retentionCode, [SLDestructionCertsId] = 0 WHERE [Id] = @Id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@retentionCode", newRetentionCode);
                            cmd.Parameters.AddWithValue("@Id", Conversions.ToInteger(row["Id"]));
                            cmd.ExecuteNonQuery();

                            if (GetDestructionCertChildrenCount(certid, conn) == 0)
                                DeleteDestructionCertRecord(certid, conn);
                        }
                        // HeyReggie - do we need to handle inactivity flag here like we do in desktop by setting [%slRetentionInactive] field?  10/23/2020
                    }
                }
            }
        }

        public static bool UpdateRetentionData(string tableId, string userName, FinalDisposition dispositionType, DateTime dispositionDate, string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return UpdateRetentionData(tableId, userName, dispositionType, dispositionDate, tableName, conn);
            }
        }

        public static bool UpdateRetentionData(string tableId, string userName, FinalDisposition dispositionType, DateTime dispositionDate, string tableName, SqlConnection conn)
        {
            string sql = "UPDATE [SLDestructCertItems] SET [ApprovedBy] = @userName, [DispositionType] = @dispositionType, [DispositionDate] = @dispositionDate WHERE [TableName] = @tableName AND [TableId] = @tableId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@dispositionType", dispositionType);
                cmd.Parameters.AddWithValue("@dispositionDate", dispositionDate);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", tableId);

                try
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public static DateTime CalcRetentionArchiveDate(string tableName, DataRow retentionItem, string retentionCode, Passport passport)
        {
            var tableInfo = Navigation.GetTableInfo(tableName, passport);
            var codeRow = GetRetentionCode(retentionCode, passport);
            string dateField = string.Empty;

            switch (codeRow["RetentionEventType"].ToString() ?? "")
            {
                case "Date Opened":
                    {
                        dateField = tableInfo["RetentionDateOpenedField"].ToString();
                        break;
                    }
                case "Date Created":
                    {
                        dateField = tableInfo["RetentionDateCreateField"].ToString();
                        break;
                    }
                case "Date Closed":
                    {
                        dateField = tableInfo["RetentionDateClosedField"].ToString();
                        break;
                    }
                case "Date Other":
                    {
                        dateField = tableInfo["RetentionDateOtherField"].ToString();
                        break;
                    }
            }

            try
            {
                return Conversions.ToDate(retentionItem[dateField]).AddYears(Conversions.ToInteger(codeRow["RetentionPeriodTotal"]));
            }
            catch
            {
                return default;
            }
        }

        public static DateTime CalcRetentionInactiveDate(DataRow tableInfo, DataRow retentionItem, string retentionCode, [Optional, DefaultParameterValue("")] ref string returnInactivityEventType, DataRow codeRow = null)
        {
            // Dim tableInfo As DataRow = GetTableInfo(tableName, passport)
            if (codeRow is null)
                codeRow = retentionItem;
            string dateField = string.Empty;
            try
            {
                returnInactivityEventType = codeRow["InactivityEventType"].ToString();

                switch (codeRow["InactivityEventType"].ToString() ?? "")
                {
                    case "Date Opened":
                        {
                            dateField = tableInfo["RetentionDateOpenedField"].ToString();
                            break;
                        }
                    case "Date Created":
                        {
                            dateField = tableInfo["RetentionDateCreateField"].ToString();
                            break;
                        }
                    case "Date Closed":
                        {
                            dateField = tableInfo["RetentionDateClosedField"].ToString();
                            break;
                        }
                    case "Date Other":
                        {
                            dateField = tableInfo["RetentionDateOtherField"].ToString();
                            break;
                        }
                }

                // Dim dDispositionDate As Date = ApplyYearEndToDate(CDate(retentionItem(dateField)), CDbl(codeRow("InactivityPeriod")), CBoolean(codeRow("InactivityForceToEndOfYear")), _passport)
                return Conversions.ToDate(retentionItem[dateField]).AddMonths((int)Math.Round(Conversions.ToDouble(codeRow["InactivityPeriod"]) * 12d));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return default;
            }
        }

        public static DateTime CalcRetentionInactiveDate(DataRow tableInfo, DataRow retentionItem, string retentionCode, Passport passport, [Optional, DefaultParameterValue("")] ref string returnInactivityEventType, DataRow codeRow = null)
        {
            return CalcRetentionInactiveDate(tableInfo, retentionItem, retentionCode, ref returnInactivityEventType, codeRow);
        }

        public static DateTime CalcRetentionInactiveDate(string tableName, DataRow retentionItem, string retentionCode, Passport passport, [Optional, DefaultParameterValue("")] ref string returnInactivityEventType)
        {
            var tableInfo = Navigation.GetTableInfo(tableName, passport);
            return Retention.CalcRetentionInactiveDate(tableInfo, retentionItem, retentionCode, passport, ref returnInactivityEventType);
        }

        public static void DeleteDestructionCertRecord(int destructionCertID, SqlConnection conn)
        {
            if (destructionCertID == 0)
                return;

            using (var cmd = new SqlCommand(Resources.DeleteDestructionCertificate, conn))
            {
                cmd.Parameters.AddWithValue("@destructionCertID", destructionCertID);
                cmd.ExecuteNonQuery();
            }
        }

        private static void DeleteRetentionRecord(int destructionCertItemID, int destructionCertId, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.DeleteDestructionCertificateItem, conn))
            {
                cmd.Parameters.AddWithValue("@destructionCertID", destructionCertItemID);
                cmd.ExecuteNonQuery();
            }

            if (destructionCertId == 0)
                return;

            int count = GetDestructionCertChildrenCount(destructionCertId, conn);
            if (count == 0)
                DeleteDestructionCertRecord(destructionCertId, conn);
        }

        public void ExecuteRetentionByRecord(string TableName, string TableID)
        {
            // Hey(Scott)
            // get table info,
            // calc retention 
            // remember end of year
            // set flags
        }

        private static void SaveRetentionData(string tableName, string tableID, int destructionCertItemID, int destructionCertId, string oldRetentionCode, string newRetentionCode, bool onHold, string holdReason, RetentionHoldTypes holdType, DateTime snoozeUntil, DateTime scheduledDestructionDate, DateTime scheduledInactivityDate, SqlConnection conn)
        {
            bool dispositionFlag = true;
            // Dim destructionCertID As Integer = GetDestructionCertID(destructionCertItemID, conn)

            if (snoozeUntil > DateTime.MinValue & snoozeUntil <= DateTime.Today)
                dispositionFlag = false;

            if (onHold | string.Compare(oldRetentionCode, newRetentionCode, true) != 0)
            {
                // Check to see if Parent has any other children and if not, delete it.
                // If destructionCertId = 1 Then DeleteDestructionCertRecord(destructionCertId, conn)
                int count = GetDestructionCertChildrenCount(destructionCertId, conn);
                if (count == 1)
                {
                    DeleteDestructionCertRecord(destructionCertId, conn);
                }
                destructionCertId = 0;
                dispositionFlag = false;
            }

            SaveDestructionCertItem(tableName, tableID, destructionCertItemID, destructionCertId, oldRetentionCode, newRetentionCode, onHold, holdReason, holdType, snoozeUntil, scheduledDestructionDate, scheduledInactivityDate, dispositionFlag, conn);
        }

        public static int GetDestructionCertChildrenCount(int destructionCertID, SqlConnection conn)
        {
            if (destructionCertID == 0)
                return 0;

            using (var cmd = new SqlCommand(Resources.GetDestructionCertChildrenCount, conn))
            {
                cmd.Parameters.AddWithValue("@DestructionCertID", destructionCertID);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return 0;
                    return Conversions.ToInteger(dt.Rows[0]["ItemCount"]);
                }
            }
        }

        private int GetDestructionCertID(int destructionCertItemID, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetDestructionCertID, conn))
            {
                cmd.Parameters.AddWithValue("@DestructionCertItemID", destructionCertItemID);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return 0;
                    return Conversions.ToInteger(dt.Rows[0]["SLDestructionCertsId"]);
                }
            }
        }

        public static void SaveDestructionCertItem(string tableName, string tableID, int destructionCertItemID, int destructionCertID, string oldRetentionCode, string newRetentionCode, bool onHold, string holdReason, RetentionHoldTypes holdType, DateTime snoozeUntil, DateTime scheduledDestructionDate, DateTime scheduledInactivityDate, bool dispositionFlag, SqlConnection conn)
        {
            string sql = Resources.UpdateDestructionCertificateItem;
            if (destructionCertItemID == 0)
                sql = Resources.CreateDestructionCertificateItem;
            var minSQLDate = DateTime.Parse("1/1/1753 12:00:00 AM");
            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (destructionCertItemID == 0)
                    {
                        cmd.Parameters.AddWithValue("@tableId", tableID);
                        cmd.Parameters.AddWithValue("@tableName", tableName);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@destructionCertItemID", destructionCertItemID);
                    }

                    cmd.Parameters.AddWithValue("@destructionCertsID", destructionCertID);
                    cmd.Parameters.AddWithValue("@holdReason", holdReason);
                    cmd.Parameters.AddWithValue("@retentionHold", onHold & holdType == RetentionHoldTypes.RetentionHold);
                    cmd.Parameters.AddWithValue("@legalHold", holdType == RetentionHoldTypes.LegalHold);
                    cmd.Parameters.AddWithValue("@retentionCode", newRetentionCode);

                    if (snoozeUntil == DateTime.MinValue | snoozeUntil < minSQLDate)
                    {
                        cmd.Parameters.AddWithValue("@snoozeUntil", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@snoozeUntil", snoozeUntil);
                    }

                    // Changed on 02/06/2016.
                    if (scheduledDestructionDate == DateTime.MinValue | scheduledDestructionDate < minSQLDate)
                    {
                        cmd.Parameters.AddWithValue("@scheduledDestruction", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@scheduledDestruction", scheduledDestructionDate);
                    }

                    if (scheduledInactivityDate == DateTime.MinValue | scheduledInactivityDate < minSQLDate)
                    {
                        cmd.Parameters.AddWithValue("@scheduledInactivity", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@scheduledInactivity", scheduledInactivityDate);
                    }

                    cmd.Parameters.AddWithValue("@dispositionFlag", dispositionFlag);
                    // cmd.Parameters.AddWithValue("@scheduledDestruction", scheduledDestructionDate)
                    // cmd.Parameters.AddWithValue("@scheduledInactivity", scheduledInactivityDate)
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public static int CreateDestructionCert(string createdBy, DateTime dateCreated, string loginName, string domain, string computerName, string macAddress, string ip, int retentionDispositionType, Passport passport)
        {
            string sql = "INSERT INTO SLDestructionCerts (CreatedBy, DateCreated, NetworkLoginName, Domain, ComputerName, MacAddress, IP, RetentionDispositionType) " + "VALUES (@CreatedBy, @DateCreated, @NetworkLoginName, @Domain, @ComputerName, @MacAddress, @IP, @RetentionDispositionType)  SELECT scope_identity() ";
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                    cmd.Parameters.AddWithValue("@DateCreated", dateCreated);
                    cmd.Parameters.AddWithValue("@NetworkLoginName", loginName);
                    cmd.Parameters.AddWithValue("@Domain", domain);
                    cmd.Parameters.AddWithValue("@ComputerName", computerName);
                    cmd.Parameters.AddWithValue("@MacAddress", macAddress);
                    cmd.Parameters.AddWithValue("@IP", ip);
                    cmd.Parameters.AddWithValue("@RetentionDispositionType", retentionDispositionType);
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
        }

        private static void UpdateRetentionCodeInTableRecord(string tablename, string tableID, string retentionCode, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(string.Format(Resources.UpdateRetentionCodeInTable, tablename, Navigation.GetRetentionFieldName(tablename, conn), Navigation.GetPrimaryKeyFieldName(tablename, conn)), conn))
            {
                cmd.Parameters.AddWithValue("@retentionCode", retentionCode);
                cmd.Parameters.AddWithValue("@tableID", tableID);
                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable GetRetentionCodes(Passport passport)
        {
            string sql = "SELECT * FROM SLRetentionCodes ORDER BY Id";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static DataRow GetRetentionCode(string retentionCode, SqlConnection conn)
        {
            string sql = "SELECT * FROM SLRetentionCodes WHERE [Id] = @retentionCode";
            DataRow dtRow;
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@retentionCode", retentionCode);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        dtRow = dt.Rows[0];
                    }
                    else
                    {
                        dtRow = dt.NewRow();
                    }
                }
            }
            return dtRow;
        }

        public static DataRow GetRetentionCode(string retentionCode, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetRetentionCode(retentionCode, conn);
            }
        }

        public static DataRow GetDescCertRow(string tableName, string tableId, Passport passport)
        {
            string sql = "SELECT * FROM slDestructCertItems WHERE tableName=@tableName AND tableId=@tableId";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                            return null;
                        return dt.Rows[0];
                    }
                }
            }
        }

        public static DataRow GetDescCert(int CertId, Passport passport)
        {
            return GetDescCert(CertId, passport.StaticConnection());
        }

        public static DataRow GetDescCert(int CertId, SqlConnection conn)
        {
            string sql = "SELECT * FROM SLDestructionCerts WHERE Id=@certId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@certId", CertId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return null;
                    return dt.Rows[0];
                }
            }
        }
        public static int CreateEligibleRecordsForReport(FinalDisposition iCurrDispositionType, Passport passport)
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

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sSQL, conn))
                {
                    cmd.Parameters.AddWithValue("@snoozeUntil", DateTime.Today.AddDays(1d));
                    cmd.ExecuteNonQuery();
                }

                var allTables = Navigation.GetAllTables(passport);

                foreach (RecordsManage.TablesRow oTables in allTables)
                {
                    if (Conversions.ToBoolean(oTables.RetentionPeriodActive) & oTables.RetentionFinalDisposition == (int)iCurrDispositionType)
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

                        bool idFieldIsString = Navigation.FieldIsAString(oTables.TableName, passport);
                        string sFormattedIdFieldName = "([" + oTables.TableName + "].[" + Navigation.MakeSimpleField(oTables.IdFieldName) + "] = [SLDestructCertItems].[TableId] AND " + "[SLDestructCertItems].[TableName] = '" + oTables.TableName + "') ";

                        //bool isInactive = oTables.RetentionInactivityActive;
                        //if (isInactive)
                        //{
                        //    sSQL += " [SLRetentionCodes].* FROM (([" + oTables.TableName + "] INNER JOIN [SLRetentionCodes] ON [" + oTables.TableName + "].[" + Navigation.MakeSimpleField(oTables.RetentionFieldName) + "] = SLRetentionCodes.Id)) LEFT JOIN " + "[SLDestructCertItems] ON " + sFormattedIdFieldName + " WHERE ([" + oTables.TableName + "].[ % slRetentionInactive] <> 0)"+" AND (NOT [" + oTables.TableName + "].[ % slRetentionInactive] IS NULL) "+" AND (( [" + oTables.TableName + "].[ % slRetentionInactivefinal] = 0 ) OR ([" + oTables.TableName + "].[ % slRetentionInactivefinal] IS NULL)"+" AND (([SLDestructCertItems].[LegalHold] = 0) OR ([SLDestructCertItems].[LegalHold] IS NULL)) " + " AND (([SLDestructCertItems].[RetentionHold] = 0) OR ([SLDestructCertItems].[RetentionHold] IS NULL)) "+ " AND (([SLDestructCertItems].[SnoozeUntil] <= '8/23/2023') OR ([SLDestructCertItems].[SnoozeUntil] IS NULL)))";
                        //}

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
                                    var trackingTables = Navigation.GetTrackingTables(passport);
                                    string fieldRetentionCode = oTables.RetentionFieldName;
                                    var tableInfo = Navigation.GetTableInfo(oTables.TableName, passport);

                                    foreach (DataRow row in dt.Rows)
                                    {
                                        dDestructionDate = DateTime.MinValue;
                                        try
                                        {
                                            if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Opened", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateOpenedField.Length > 0)
                                                    dDestructionDate = Conversions.ToDate(row[fieldOpenDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Closed", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateClosedField.Length > 0)
                                                    dDestructionDate = Conversions.ToDate(row[fieldCloseDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Created", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateCreateField.Length > 0)
                                                    dDestructionDate = Conversions.ToDate(row[fieldCreateDate]);
                                            }
                                            else if (Strings.StrComp(row["RetentionEventType"].ToString(), "Date Other", Constants.vbTextCompare) == 0)
                                            {
                                                if (oTables.RetentionDateOtherField.Length > 0)
                                                    dDestructionDate = Conversions.ToDate(row[fieldOtherDate]);
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

                                                dDestructionDate = Retention.GetDestructionDate(oTables.TableName, sTrackedTableId, trackingTables, passport);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.Print(ex.Message);
                                        }

                                        if (dDestructionDate > DateTime.MinValue)
                                        {
                                            dScheduledDate = Navigation.ApplyYearEndToDate(dDestructionDate, Conversions.ToDouble(row["RetentionPeriodTotal"]), Navigation.CBoolean(row["RetentionPeriodForceToEndOfYear"]), passport);

                                            if (dScheduledDate <= DateTime.Now)
                                            {
                                                // Record has exceeded it's eligible Date; Add to list
                                                if (!bFoundOne)
                                                {
                                                    // create cert if it does not exist
                                                    destructionCertId = CreateDestructionCert(new User(passport, true).UserName, DateTime.Now, "", "", "", "", "", (int)iCurrDispositionType, passport);
                                                    bFoundOne = true;
                                                }

                                                string argreturnInactivityEventType = "";
                                                var inactDate = Retention.CalcRetentionInactiveDate(tableInfo, row, row[fieldRetentionCode].ToString(), passport, returnInactivityEventType: ref argreturnInactivityEventType);
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

        public static DateTime GetDestructionDate(string sTableName, string sTableID, RecordsManage.TablesDataTable trackingTables, Passport passport)
        {
            var dDestructionDate = DateTime.MinValue;
            RecordsManage.TablesDataTable oTable;
            DataTable oTrackingStatus;
            string sSQL;

            oTable = Navigation.GetTypedTableInfo(sTableName, passport);

            if (oTable is not null)
            {
                // Dim trackingTables = GetTrackingTables(Passport)
                if (trackingTables.Count > 0)
                {
                    var oLocation = trackingTables[0];
                    string locationFieldName = oLocation.TableName + Navigation.MakeSimpleField(oLocation.IdFieldName);
                    bool locationIdIsString = Navigation.FieldIsAString(oLocation.TableName, passport);
                    oTrackingStatus = Tracking.GetTrackingStatus(oTable.TableName, locationFieldName, sTableID, passport);

                    if (oTrackingStatus.Rows.Count > 0)
                    {
                        dDestructionDate = Conversions.ToDate(oTrackingStatus.Rows[0]["TransactionDateTime"]);
                        string sLocationID;


                        foreach (DataRow row in oTrackingStatus.Rows)
                        {
                            sLocationID = row[locationFieldName].ToString();

                            if (Operators.CompareString(sLocationID, string.Empty, false) > 0)
                            {
                                sSQL = "SELECT * FROM [" + oLocation.TableName + "] WHERE [" + Navigation.MakeSimpleField(oLocation.IdFieldName) + "] = "; // & oLocation.IDQueryValue(sLocationID)
                                if (locationIdIsString)
                                {
                                    sSQL += "'" + sLocationID + "'";
                                }
                                else
                                {
                                    sSQL += sLocationID;
                                }

                                using (var conn = passport.Connection())
                                {
                                    using (var cmd = new SqlCommand(sSQL, conn))
                                    {
                                        using (var da = new SqlDataAdapter(cmd))
                                        {
                                            var dt = new DataTable();
                                            da.Fill(dt);
                                            // rsEligibleRecords = moApp.Data.GetADORecordset(sSQL, oTables.TableName)

                                            if (dt.Rows.Count > 0)
                                            {

                                                if (Conversions.ToBoolean(dt.Rows[0][Navigation.MakeSimpleField(oLocation.InactiveLocationField)]))
                                                {
                                                    dDestructionDate = DateTime.MinValue;
                                                }
                                                else
                                                {
                                                    dDestructionDate = Conversions.ToDate(row["TransactionDateTime"]);
                                                    break;
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
            return dDestructionDate;
        }

        public static bool UsesRetentionType(FinalDisposition iCurrDispositionType, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return UsesRetentionType(iCurrDispositionType, conn);
            }
        }

        public static bool UsesRetentionType(FinalDisposition iCurrDispositionType, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) AS TotalCount FROM Tables WHERE ([RetentionFinalDisposition] = @finalDisposition) AND (RetentionPeriodActive IS NOT NULL AND RetentionPeriodActive <> 0)", conn))
            {
                cmd.Parameters.AddWithValue("@finalDisposition", (int)iCurrDispositionType);
                return Conversions.ToInteger(cmd.ExecuteScalar()) > 0;
            }
        }

        public static bool ApplyDispositionToList(List<string> tableNamesIds, FinalDisposition dispositionType, string remoteHost, DateTime dispositionDate, Passport passport, string archiveLocation = "", string archiveId = "")
        {
            var success = default(bool);
            // check up front when archiving if user has correct permissions and throw an exception if not
            if (dispositionType == FinalDisposition.PermanentArchive)
            {
                foreach (string tableNameId in tableNamesIds)
                {
                    string tableName = tableNameId.Split('|')[0];

                    if (!passport.CheckPermission(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                        throw new Exception("Not authorized to transfer");
                    if (!passport.CheckSetting(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                        throw new Exception(string.Format("{0} are not transferable", tableName));
                    if (!passport.CheckPermission(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Transfer))
                        throw new Exception("Not authorized to transfer");
                }
            }

            using (var conn = passport.Connection())
            {
                foreach (string tableNameId in tableNamesIds)
                {
                    string tableName = tableNameId.Split('|')[0];
                    string tableId = tableNameId.Split('|')[1];
                    string idField = Navigation.GetPrimaryKeyFieldName(tableName, conn);

                    switch (dispositionType)
                    {
                        case FinalDisposition.Destruction:
                        case FinalDisposition.PermanentArchive:
                            {
                                if (dispositionType == FinalDisposition.PermanentArchive && (string.IsNullOrWhiteSpace(archiveLocation) || string.IsNullOrWhiteSpace(archiveId)))
                                    return false;

                                string sql = string.Format("UPDATE [{0}] SET [%slRetentionDispositionStatus] = @dispositionType WHERE [{1}] = @tableId", tableName, Navigation.MakeSimpleField(idField));

                                using (var cmd = new SqlCommand(sql, conn))
                                {
                                    cmd.Parameters.AddWithValue("@dispositionType", dispositionType);
                                    cmd.Parameters.AddWithValue("@tableId", tableId);

                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                        success = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.Message);
                                        success = false;
                                    }

                                    if (success)
                                    {
                                        var user = new User(passport, true);
                                        if (!UpdateRetentionData(tableId, user.UserName, dispositionType, dispositionDate, tableName, conn))
                                            success = false;

                                        if (success)
                                        {
                                            if (dispositionType == FinalDisposition.PermanentArchive)
                                            {
                                                Tracking.Transfer(tableName, tableId, archiveLocation, archiveId, default(DateTime), user.UserName, passport, conn);
                                            }
                                            else
                                            {
                                                Query.DeleteTableItem(tableName, tableId, remoteHost, false, false, false, false, conn, passport);
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        case FinalDisposition.Purge:
                            {
                                success = true;
                                if (!UpdateRetentionData(tableId, new User(passport, true).UserName, dispositionType, dispositionDate, tableName, conn))
                                    return false;

                                if (success)
                                {
                                    if (!RecursivePurge(tableName, tableId, false, false, remoteHost, passport))
                                        success = false;
                                }

                                break;
                            }
                    }

                    if (!success)
                        break;
                }
            }

            return success;
        }

        public static bool SetListInactive(List<string> tableIdList, string inactiveLocation, string inactiveId, Passport passport)
        {
            var user = new User(passport, true);
            bool success = true;

            try
            {
                using (var conn = passport.Connection())
                {
                    foreach (var tableId in tableIdList)
                    {
                        string tableName = Strings.Split(tableId, "|")[0];
                        string id = Strings.Split(tableId, "|")[1];

                        var tableInfo = Navigation.GetTableInfo(tableName, conn);
                        string idfield = tableInfo["idFieldName"].ToString();
                        if (!Navigation.FieldIsAString(tableInfo, conn))
                            id = Navigation.StripLeadingZeros(id, false);

                        string sql = string.Format("UPDATE [{0}] SET [%slRetentionInactiveFinal] = 1 WHERE [{1}] = @Id", tableName, Navigation.MakeSimpleField(idfield));

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            success = cmd.ExecuteNonQuery() > 0;
                        }

                        if (success && Navigation.CBoolean(tableInfo["Trackable"]))
                            Tracking.Transfer(tableName, id, inactiveLocation, inactiveId, default(DateTime), user.UserName, passport, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return success;
        }

        public static bool RecursivePurge(string tableName, string id, bool doVerify, bool isChildRow, string remoteHost, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return RecursivePurge(tableName, id, doVerify, isChildRow, remoteHost, conn, passport);
            }
        }

        public static bool RecursivePurge(string tableName, string id, bool doVerify, bool isChildRow, string remoteHost, SqlConnection conn, Passport passport)
        {
            if (doVerify && !passport.CheckPermission(tableName, SecureObject.SecureObjectType.Table, Permissions.Permission.Delete))
                return false;

            Query.DeleteTableItem(tableName, id, remoteHost, doVerify, false, true, isChildRow, conn, passport);
            var lowerTableInfo = Navigation.GetLowerTableInfos(tableName, Navigation.GetPrimaryKeyFieldName(tableName, conn), conn);

            foreach (DataRow lowerTable in lowerTableInfo.Rows)
            {
                if (passport.CheckPermission(lowerTable["TableName"].ToString(), SecureObject.SecureObjectType.Table, Permissions.Permission.Delete))
                {
                    string sql = string.Format("SELECT [{0}] FROM [{1}] WHERE [{2}] = @UpperId", Navigation.MakeSimpleField(lowerTable["IdFieldName"].ToString()), lowerTable["TableName"].ToString(), Navigation.MakeSimpleField(lowerTable["LowerTableFieldName"].ToString()));

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UpperId", id);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            var dt = new DataTable();
                            da.Fill(dt);

                            foreach (DataRow lowerRow in dt.Rows)
                                RecursivePurge(lowerTable["TableName"].ToString(), lowerRow[Navigation.MakeSimpleField(lowerTable["IdFieldName"].ToString())].ToString(), doVerify, true, remoteHost, conn, passport);
                        }
                    }
                }
            }

            return true;
        }

        public static void ApproveDestruction(string id, DateTime dispositionDate, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                string sql = "UPDATE [SLDestructionCerts] SET [ApprovedBy] = @userName, [DateDestroyed] = @dispositionDate WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userName", new User(passport, true).UserName);
                    cmd.Parameters.AddWithValue("@dispositionDate ", dispositionDate);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT COUNT(*) FROM [SLDestructCertItems] WHERE [SLDestructionCertsId] = @Id AND [DispositionDate] IS NULL";
                }
            }
        }

        public static async Task ApproveDestructionAsync(string id, DateTime dispositionDate, Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                string sql = "UPDATE [SLDestructionCerts] SET [ApprovedBy] = @userName, [DateDestroyed] = @dispositionDate WHERE Id = @Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@userName", new User(passport, true).UserName);
                    cmd.Parameters.AddWithValue("@dispositionDate ", dispositionDate);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT COUNT(*) FROM [SLDestructCertItems] WHERE [SLDestructionCertsId] = @Id AND [DispositionDate] IS NULL";
                }
            }
        }

    }
}