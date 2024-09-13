using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Entities;
using MSRecordsEngine.Properties;
using MSRecordsEngine.Repository;
using Smead.Security;
using static MSRecordsEngine.RecordsManager.RecordsManage;
using SecureObject = Smead.Security.SecureObject;


namespace MSRecordsEngine.RecordsManager
{

    public class WorkGroupItem
    {
        public long ID;
        public string WorkGroupName;
    }

    public class TableItem
    {
        public string ID;
        public string TableName;
        public string UserName;
        public string KeyFieldName;
        public string BarcodePrefix = string.Empty;
        public int TrackingTable = -1;


        public TableItem()
        {

        }
    }

    public class ViewItem
    {
        public int Id;
        public string ViewName;

        public ViewItem() { }
        public ViewItem(int Id, string viewName)
        {
            this.Id = Id;
            ViewName = viewName;
        }
    }

    public class Navigation
    {
        private const string EncryptionKey = "MAKV2SPBNI99212";
        public const string DelimiterText = "*delim*";
        private static object cipherText;

        public class Enums
        {
            public enum meFinalDispositionStatusType
            {
                /// <summary>
                /// Indicates the current disposition status.
                ///    Active: Has not reached the retention point.
                /// Archived: Item has been archived or transferred to an "Archive" location.
                /// Destroyed: Item has been destoryed.
                /// Purged is not needed because the record is deleted from the system and never flagged as purged.
                /// </summary>
                fdstActive = 0,
                fdstArchived = 1,
                fdstDestroyed = 2
            }
            public enum eLabelExists
            {
                None = 0,
                BlackAndWhite,
                Color,
                BWAndColor
            }
        }

        public static DataTable GetAttachments(string tableName, string tableId, Passport passport)
        {
            string sql = "SELECT * FROM USERLINKS WHERE IndexTableID=@IndexTableID and IndexTable=@IndexTable ORDER BY AttachmentNumber";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    var dt = new DataTable();
                    cmd.Parameters.AddWithValue("@indexTable", tableName);
                    cmd.Parameters.AddWithValue("@indexTableId", PrepPad(tableName, tableId, conn));

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    return dt;
                }
            }
        }
        public static DataTable GetAttachableTables(Passport passport)
        {
            string sql = "SELECT [TableName] FROM [Tables] WHERE Attachments = 1 ORDER BY [TableName]";

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

        public static void MoveAttachment(string sourceTableName, string sourceTableId, string sourceAttachmentNumber, string destinationTableName, string destinationTableID, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                MoveAttachment(sourceTableName, sourceTableId, sourceAttachmentNumber, destinationTableName, destinationTableID, conn);
            }
        }

        public static void MoveAttachment(string sourceTableName, string sourceTableId, string sourceAttachmentNumber, string destinationTableName, string destinationTableID, SqlConnection conn)
        {
            string sql = "UPDATE [UserLinks] SET [IndexTable] = @destIndexTable, [IndexTableId] = @destIndexTableId, " + "[AttachmentNumber] = (SELECT TOP 1 [AttachmentNumber] FROM [UserLinks] WHERE [IndexTable] = @destIndexTable AND " + "[IndexTableId] = @destIndexTableId ORDER BY [AttachmentNumber] DESC) + 1 WHERE [IndexTableID] = @sourceIndexTableID AND " + "[IndexTable] = @sourceIndexTable AND [AttachmentNumber] = @sourceAttachmentNumber";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@destIndexTable", destinationTableName);
                cmd.Parameters.AddWithValue("@destIndexTableId", PrepPad(destinationTableName, destinationTableID, conn));
                cmd.Parameters.AddWithValue("@sourceIndexTable", sourceTableName);
                cmd.Parameters.AddWithValue("@sourceIndexTableId", PrepPad(sourceTableName, sourceTableId, conn));
                cmd.Parameters.AddWithValue("@sourceAttachmentNumber", sourceAttachmentNumber);
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetSystemSetting(string fieldName, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 [" + fieldName + "] FROM System", conn))
            {
                return cmd.ExecuteScalar().ToString();
            }
        }
        // added by moti mashiah
        public static DataTable GetPkeyProperty(string keyfield, string tableName, SqlConnection conn)
        {
            var dt = new DataTable();
            using (var cmd = new SqlCommand("SELECT [" + keyfield + "] FROM [" + tableName + "]", conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
        // added moti mashiah
        public static bool IsSystemColumn(string columnName)
        {
            if (columnName.ToLower() == "formattedid")
                return true;
            // If columnName.ToLower = "id" Then Return True
            if (columnName.ToLower() == "attachments")
                return true;
            if (columnName.ToLower() == "slrequestable")
                return true;
            if (columnName.ToLower() == "tablename")
                return true;
            if (columnName.ToLower() == "tableid")
                return true;
            if (columnName.ToLower() == "itemname")
                return true;
            if (columnName.ToLower() == "pkey")
                return true;
            if (columnName.ToLower() == "dispositionstatus")
                return true;
            if (columnName.ToLower() == "processeddescfieldnameone")
                return true;
            if (columnName.ToLower() == "processeddescfieldnametwo")
                return true;
            if (columnName.ToLower() == "rownum")
                return true;
            return false;
        }
        public static string GetSystemSetting(string fieldName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetSystemSetting(fieldName, conn);
            }
        }

        public static bool DeleteSetting(string section, string item, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("DELETE FROM SETTINGS WHERE [Section] = @section AND [Item] = @item", conn))
            {
                cmd.Parameters.AddWithValue("@section", section);
                cmd.Parameters.AddWithValue("@item", item);

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

        public static bool DeleteSetting(string section, string item, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return DeleteSetting(section, item, conn);
            }
        }

        public static string GetSetting(string section, string item, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT [ItemValue] FROM [Settings] WHERE [Section] = @section AND [Item] = @item", conn))
            {
                cmd.Parameters.AddWithValue("@section", section);
                cmd.Parameters.AddWithValue("@item", item);

                try
                {
                    return cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
        }

        public static async Task<string> GetSettingAsync(string section, string item, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT [ItemValue] FROM [Settings] WHERE [Section] = @section AND [Item] = @item", conn))
            {
                cmd.Parameters.AddWithValue("@section", section);
                cmd.Parameters.AddWithValue("@item", item);
                var getsetting = await cmd.ExecuteScalarAsync();
                return getsetting.ToString();
            }

        }

        [SecuritySafeCritical]
        public static void GetOutputSettingValues(Entities.SystemAddress oSystemAddress, Entities.Volume oVolume, Entities.OutputSetting oOutputSetting, Passport passport, ref bool bIsValidOutputSettings, ref bool bIsOutputSettingsActive)
        {
            if (oSystemAddress is not null && CBoolean(oVolume.Active))
            {
                string checkPath = oSystemAddress.PhysicalDriveLetter;
                if (checkPath.StartsWith(@"\\"))
                    checkPath += string.Format("{0}{1}", oVolume.PathName.StartsWith(@"\") ? string.Empty : @"\", oVolume.PathName);

                bIsValidOutputSettings = System.IO.Directory.Exists(checkPath) && passport.CheckPermission(oOutputSetting.Id, Smead.Security.SecureObject.SecureObjectType.OutputSettings, Permissions.Permission.Access);
                bIsOutputSettingsActive = CBoolean(oOutputSetting.InActive) == false;
            }
        }

        public static string GetSetting(string section, string item, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetSetting(section, item, conn);
            }
        }

        public static void SendEmail(string message, string toAddressList, string fromAddress, string subject, string attachmentList, SqlConnection conn)
        {
            if (string.IsNullOrWhiteSpace(toAddressList) || string.IsNullOrWhiteSpace(fromAddress))
                return;

            var mail = new MailMessage();
            mail.Body = message;
            mail.From = new MailAddress(fromAddress.ToLower().Trim());
            mail.Subject = subject;
            mail.IsBodyHtml = false;

            foreach (string address in toAddressList.Split(';'))
                mail.To.Add(new MailAddress(address.ToLower().Trim()));

            foreach (string attachment in attachmentList.Split(';'))
            {
                if (!string.IsNullOrWhiteSpace(attachment))
                    mail.Attachments.Add(new Attachment(attachment));
            }

            SmtpClient smtp;
            string smtpServer = GetSystemSetting("SMTPServer", conn);
            string smtpPort = GetSystemSetting("SMTPPort", conn);

            if (!string.IsNullOrWhiteSpace(smtpServer))
            {
                if (!string.IsNullOrWhiteSpace(smtpPort))
                {
                    smtp = new SmtpClient(smtpServer, Conversions.ToInteger(smtpPort));
                }
                else
                {
                    smtp = new SmtpClient(smtpServer);
                }
            }
            else
            {
                smtp = new SmtpClient();
            }

            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error - \"{0}\" in Navigation.SendEmail", ex.Message));
            }
        }
        public static bool IsAuditingEnabled(string tableName, string connectionStr)
        {
            using (var context = new TABFusionRMSContext(connectionStr))
            {
                var tb = context.Tables.Where(x => x.TableName == tableName).FirstOrDefault();
                if (tb.AuditUpdate == true || tb.AuditConfidentialData == true || tb.AuditAttachments == true)
                {
                    return true;
                }
            }
            return false;
        }
        public static void SetSetting(string section, string item, string value, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM [Settings] WHERE [Section] = @section AND [Item] = @item", conn))
            {
                cmd.Parameters.AddWithValue("@section", section);
                cmd.Parameters.AddWithValue("@item", item);
                cmd.Parameters.AddWithValue("@value", value);

                if (Conversions.ToInteger(cmd.ExecuteScalar()) == 0)
                {
                    cmd.CommandText = "INSERT INTO [Settings] ([Section], [Item], [ItemValue]) VALUES (@section, @item, @value)";
                }
                else
                {
                    cmd.CommandText = "UPDATE [Settings] SET [ItemValue] = @value WHERE [Section] = @section AND [Item] = @item";
                }

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public static async Task SetSettingAsync(string section, string item, string value, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM [Settings] WHERE [Section] = @section AND [Item] = @item", conn))
            {
                cmd.Parameters.AddWithValue("@section", section);
                cmd.Parameters.AddWithValue("@item", item);
                cmd.Parameters.AddWithValue("@value", value);

                if (Conversions.ToInteger(await cmd.ExecuteScalarAsync()) == 0)
                {
                    cmd.CommandText = "INSERT INTO [Settings] ([Section], [Item], [ItemValue]) VALUES (@section, @item, @value)";
                }
                else
                {
                    cmd.CommandText = "UPDATE [Settings] SET [ItemValue] = @value WHERE [Section] = @section AND [Item] = @item";
                }

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public static void SetSetting(string section, string item, string value, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                SetSetting(section, item, value, conn);
            }
        }
        public static async Task SetSettingAsync(string section, string item, string value, Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                await SetSettingAsync(section, item, value, conn);
            }
        }

        public static RecordsManage.ViewsDataTable GetViewReports(Passport passport)
        {
            using (var reportsAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    reportsAdapter.Connection = conn;
                    return reportsAdapter.GetViewReports();
                }
            }
        }

        public static bool HasSearchableAttachments(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return HasSearchableAttachments(tableName, conn);
            }
        }

        public static bool HasSearchableAttachments(string tableName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName))
                return false;
            string sql = "SELECT COUNT(*) FROM [SLTextSearchItems] WHERE [IndexTableName] = @tableName AND [IndexType] <> 8";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                return CBoolean(cmd.ExecuteScalar());
            }
        }

        public static bool IsArchivedOrDestroyed(string TableName, string TableId, Enums.meFinalDispositionStatusType eCompareType, SqlConnection conn)
        {
            bool rtn = false;

            try
            {
                var info = GetTableInfo(TableName, conn);
                if (!string.IsNullOrEmpty(info["RetentionFieldName"].ToString()) && CBoolean(info["RetentionPeriodActive"]))
                {
                    bool IdIsString = FieldIsAString(TableName, conn);
                    Enums.meFinalDispositionStatusType eStatusType;
                    var dt = new DataTable();
                    string sSQL = string.Format("SELECT [%slRetentionDispositionStatus] FROM [{0}] WHERE [{1}] = @TableId", TableName, MakeSimpleField(info["IdFieldName"].ToString()));

                    using (var cmd = new SqlCommand(sSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableId", TableId);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    if (dt is not null && dt.Rows.Count > 0)
                    {
                        try
                        {
                            if (!(dt.Rows[0][0] is DBNull))
                            {
                                eStatusType = (Enums.meFinalDispositionStatusType)dt.Rows[0][0];
                                switch (eStatusType)
                                {
                                    case Enums.meFinalDispositionStatusType.fdstArchived:
                                    case Enums.meFinalDispositionStatusType.fdstDestroyed:
                                        {
                                            rtn = (eStatusType & eCompareType) == eStatusType;
                                            break;
                                        }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            rtn = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return rtn;
        }

        public static bool IsSearchableTable(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return IsSearchableTable(tableName, conn);
            }
        }

        public static bool IsSearchableTable(string tableName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName))
                return false;
            string sql = "SELECT COUNT(*) FROM [SLTextSearchItems] WHERE [IndexTableName] = @tableName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                return CBoolean(cmd.ExecuteScalar());
            }
        }

        public static bool IsSearchableView(int viewId, Passport passport)
        {
            string sql = "SELECT COUNT(*) FROM [Views] WHERE [Id] = @viewid and SearchableView = 1";
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@viewid", viewId);
                    return CBoolean(cmd.ExecuteScalar());
                }
            }
        }

        public static bool IsSearchableField(string tableName, string fieldName, Passport passport)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(fieldName))
                return false;
            string sql = "SELECT COUNT(*) FROM [SLTextSearchItems] WHERE [IndexTableName] = @tableName AND [IndexFieldName] = @fieldName AND [IndexType] = 8";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@fieldName", fieldName);
                    return CBoolean(cmd.ExecuteScalar());
                }
            }
        }

        public static string PrepPad(string tableID)
        {
            foreach (char c in tableID)
            {
                if (Strings.Asc(c) < 48 | Strings.Asc(c) > 57)
                {
                    return tableID;
                }
            }
            return tableID.PadLeft(30, '0');
        }

        public static string PrepPad(string tableName, string tableID, SqlConnection conn)
        {
            if (FieldIsAString(tableName, conn))
                return tableID;
            return PrepPad(tableID);
        }

        public static string PrepPad(string tableName, string tableID, Passport passport)
        {
            if (FieldIsAString(tableName, passport))
                return tableID;
            return PrepPad(tableID);
        }

        public static string PrepPad(DataRow tableInfo, string tableID, Passport passport)
        {
            if (FieldIsAString(tableInfo, passport))
                return tableID;
            return PrepPad(tableID);
        }

        public static DataColumn FieldWithOrWithoutTable(string fieldName, DataColumnCollection columns)
        {
            if (string.IsNullOrEmpty(fieldName))
                return new DataColumn();

            try
            {
                return columns[MakeSimpleField(fieldName)];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataColumn();
            }
        }

        public static string GetFieldAsVarCharSQL(DataRow tableInfo, string fieldName, SqlConnection conn, bool IncludeTable = false)
        {
            if (IncludeTable)
            {
                fieldName = "[" + tableInfo["TableName"].ToString() + "].[" + fieldName + "]";
            }
            else
            {
                fieldName = "[" + fieldName + "]";
            }

            if (FieldIsAString(tableInfo, fieldName, conn))
                return fieldName;
            return "CONVERT(VARCHAR, " + fieldName + ")";
        }

        public static string GetFileRoomOrderSQL(DataRow tableInfo, SqlConnection conn, string tablePrefix = "")
        {
            if (!string.IsNullOrEmpty(tablePrefix))
                tablePrefix += ".";
            var dt = new DataTable();
            string fileRoomSQL = "";
            using (var cmd = new SqlCommand("SELECT * FROM SLTableFileRoomOrder WHERE TableName=@tablename", conn))
            {
                cmd.Parameters.AddWithValue("@tablename", tableInfo["TableName"]);
                var da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    return tableInfo["IdFieldName"].ToString();
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (CBoolean(row["StartFromFront"]))
                        {
                            fileRoomSQL += string.Format("SUBSTRING(" + tablePrefix + "[{0}],{1},{2} ) +", row["FieldName"].ToString(), row["StartingPosition"].ToString(), row["NumberofCharacters"].ToString());
                        }
                        else
                        {
                            fileRoomSQL += string.Format("SUBSTRING(RIGHT(REPLICATE('0', 7) + CAST(" + tablePrefix + "[{0}] AS VARCHAR(50)), {1}), 1, {2}) +", row["FieldName"].ToString(), row["StartingPosition"].ToString(), row["NumberofCharacters"].ToString());
                        }
                    }
                    fileRoomSQL = fileRoomSQL.Substring(0, fileRoomSQL.Length - 1);
                }
            }
            return fileRoomSQL;
        }

        public static bool CheckIfDuplicatePrimaryKey(Passport passsport, string tablename, string pkeyname, string pkeyValue)
        {
            bool isDuplicated;
            using (var conn = passsport.Connection())
            {
                using (var cmd = new SqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = @pkeyvalue", tablename, pkeyname), conn))
                {
                    cmd.Parameters.AddWithValue("@pkeyvalue", pkeyValue);
                    try
                    {
                        isDuplicated = Conversions.ToBoolean(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        isDuplicated = false;
                    }
                }
            }
            return isDuplicated;
        }

        public static async Task<bool> CheckIfDuplicatePrimaryKeyAsync(Passport passsport, string tablename, string pkeyname, string pkeyValue)
        {
            bool isDuplicated;
            using (var conn = new SqlConnection(passsport.ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(string.Format("SELECT COUNT(*) FROM {0} WHERE {1} = @pkeyvalue", tablename, pkeyname), conn))
                {
                    cmd.Parameters.AddWithValue("@pkeyvalue", pkeyValue);
                    try
                    {
                        isDuplicated = Conversions.ToBoolean(cmd.ExecuteScalar());
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        isDuplicated = false;
                    }
                }
            }
            return isDuplicated;
        }
        public static DataRow GetPullList(int id, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM SLPullLists WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var da = new SqlDataAdapter();
                    var dt = new DataTable();
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    // Modified by hemin
                    // Throw New NullReferenceException("The pull list could not be found or is not available.")
                    throw new NullReferenceException("The pull list could not be found or is not available.");
                }

            }
        }

        public static List<int> GetTaskLightValues(Passport passport)
        {
            //if (!passport.CheckLicense(SecureObject.SecureObjectType.Application))
            //    return null;
            if (!passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                return null;

            var list = new List<int>();
            string sql = "SELECT COUNT(*) AS TotalCount FROM [SLRequestor] WHERE [SLRequestor].[Status] = 'New'";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    list.Add(Conversions.ToInteger(cmd.ExecuteScalar()));
                    cmd.CommandText = "SELECT COUNT(*) AS TotalCount FROM [SLRequestor] LEFT JOIN SLPullLists ON SLRequestor.SLPullListsId = SLPullLists.Id" + " WHERE ((SLPullLists.BatchPullList <> 0) AND (SLPullLists.BatchPullList IS NOT NULL)" + " AND ((SLPullLists.BatchPrinted = 0) OR (SLPullLists.BatchPrinted IS NULL)))";
                    list.Add(Conversions.ToInteger(cmd.ExecuteScalar()));
                    cmd.CommandText = "SELECT COUNT(*) AS TotalCount FROM [SLRequestor] WHERE [SLRequestor].[Status] = 'Exception'";
                    list.Add(Conversions.ToInteger(cmd.ExecuteScalar()));
                }
            }

            return list;
        }
        public static async Task<List<int>> GetTaskLightValuesAsync(Passport passport)
        {
            //if (!passport.CheckLicense(SecureObject.SecureObjectType.Application))
            //    return null;
            if (!passport.CheckPermission(" Requestor", SecureObject.SecureObjectType.Reports, Permissions.Permission.View))
                return null;

            var list = new List<int>();
            string sql = "SELECT COUNT(*) AS TotalCount FROM [SLRequestor] WHERE [SLRequestor].[Status] = 'New'";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    list.Add(Conversions.ToInteger(cmd.ExecuteScalar()));
                    cmd.CommandText = "SELECT COUNT(*) AS TotalCount FROM [SLRequestor] LEFT JOIN SLPullLists ON SLRequestor.SLPullListsId = SLPullLists.Id" + " WHERE ((SLPullLists.BatchPullList <> 0) AND (SLPullLists.BatchPullList IS NOT NULL)" + " AND ((SLPullLists.BatchPrinted = 0) OR (SLPullLists.BatchPrinted IS NULL)))";
                    list.Add(Conversions.ToInteger(cmd.ExecuteScalar()));
                    cmd.CommandText = "SELECT COUNT(*) AS TotalCount FROM [SLRequestor] WHERE [SLRequestor].[Status] = 'Exception'";
                    list.Add(Conversions.ToInteger(await cmd.ExecuteScalarAsync()));
                }
            }

            return list;
        }

        public static DataTable GetViewSortFields(int viewID, Passport passport)
        {
            string sql = "SELECT RIGHT(FieldName, LEN(FieldName) - CHARINDEX('.', FieldName)) AS FieldName, Heading " + "FROM ViewColumns WHERE ViewsID = @viewId AND SortableField = 1 ORDER BY ColumnOrder";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@viewId", viewID);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static DataTable GetViewWorkFlows(int viewID, Passport passport)
        {
            string sql = "SELECT [WorkFlow1], [WorkFlow1Pic], [WorkFlowDesc1], [WorkFlowToolTip1], [WorkFlowHotKey1], " + "[WorkFlow2], [WorkFlow2Pic], [WorkFlowDesc2], [WorkFlowToolTip2], [WorkFlowHotKey2], " + "[WorkFlow3], [WorkFlow3Pic], [WorkFlowDesc3], [WorkFlowToolTip3], [WorkFlowHotKey3], " + "[WorkFlow4], [WorkFlow4Pic], [WorkFlowDesc4], [WorkFlowToolTip4], [WorkFlowHotKey4], " + "[WorkFlow5], [WorkFlow5Pic], [WorkFlowDesc5], [WorkFlowToolTip5],[WorkFlowHotKey5] FROM Views WHERE ID = @viewId";

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@viewId", viewID);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static List<WorkGroupItem> GetWorkGroups(Passport passport)
        {
            using (var tabSetAdapter = new RecordsManageTableAdapters.TabsetsTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    tabSetAdapter.Connection = conn;
                    var list = new List<WorkGroupItem>();

                    foreach (RecordsManage.TabsetsRow row in tabSetAdapter.GetWorkGroups().Rows)
                    {
                        if (passport.CheckPermission(row.UserName, SecureObject.SecureObjectType.WorkGroup, Permissions.Permission.Access))
                        {
                            foreach (TableItem table in Navigation.GetWorkGroupMenu(row.Id, passport, conn))
                            {
                                if (passport.CheckPermission(table.TableName, SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                                {
                                    if (GetTableFirstEligibleViewId(table.TableName, passport, conn) > 0)
                                    {
                                        var wg = new WorkGroupItem();
                                        wg.ID = (long)row.Id;
                                        wg.WorkGroupName = row.UserName;
                                        list.Add(wg);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    return list;
                }
            }
        }

        public static List<TableItem> GetWorkGroupMenu(short workGroupID, Passport passport, SqlConnection conn)
        {
            using (var tableTabs = new RecordsManageTableAdapters.TabletabsTableAdapter())
            {
                tableTabs.Connection = conn;

                var list = new List<TableItem>();

                foreach (DataRow row in tableTabs.GetTablesByWorkgroup(workGroupID))
                {
                    if (passport.CheckPermission(row["TableName"].ToString(), SecureObject.SecureObjectType.Table, Permissions.Permission.View))
                    {
                        var tbl = new TableItem();
                        tbl.ID = row["Id"].ToString();
                        tbl.TableName = row["TableName"].ToString();
                        tbl.UserName = row["username"].ToString();
                        list.Add(tbl);
                    }
                }

                return list;
            }
        }

        public static List<TableItem> GetWorkGroupMenu(short workGroupID, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetWorkGroupMenu(workGroupID, passport, conn);
            }
        }

        public static List<ViewItem> GetViewsByTableName(string tableName, Passport passport)
        {
            using (var views = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                using (var conn = new SqlConnection(passport.ConnectionString))
                {
                    views.Connection = conn;
                    var list = new List<ViewItem>();
                    foreach (RecordsManage.ViewsRow row in views.GetViewsByTableName(tableName))
                    {
                        if (passport.CheckPermission(row.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.View))
                        {
                            list.Add(new ViewItem(row.Id, row.ViewName));
                        }
                    }

                    return list;
                }
            }
        }
        public static async Task<List<ViewItem>> GetViewsByTableNameAsync(string tableName, Passport passport)
        {
            var sql = "SELECT Id, SQLStatement, TableName, TaskListDisplayString, ViewName, MaxRecsPerFetch FROM Views WHERE \r\n(TableName = @tableName) AND (Printable = 0) ORDER BY ViewOrder";
            var list = new List<ViewItem>();
            using (var conn = new SqlConnection(passport.ConnectionString))
            {

                var getviews = await conn.QueryAsync<ViewItem>(sql, new { @tableName = tableName });
                foreach (ViewItem row in getviews)
                {
                    if (passport.CheckPermission(row.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.View))
                    {
                        list.Add(new ViewItem(row.Id, row.ViewName));
                    }
                }


                return list;
            }

        }

        public static string GetViewItemName(int ViewId, string KeyValue, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                var tblInfo = GetTableInfo(GetViewTableName(ViewId, passport).ToString(), conn);
                string sql = "SELECT {0} FROM {1} WHERE {2}=@KeyValue";
                sql = string.Format(sql, GetViewColumnNameByOrdinal(ViewId, 0, passport), tblInfo["TableName"].ToString(), tblInfo["IdFieldName"].ToString());

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ViewID", ViewId);
                    cmd.Parameters.AddWithValue("@KeyValue", KeyValue);

                    try
                    {
                        return cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return string.Empty;
                    }
                }
            }
        }
        // written by moti mashiah - return all user views access
        public static List<ListOfviews> GetAllUserViews(Passport passport)
        {
            var lst = new List<ListOfviews>();

            var subMenu = new List<TableItem>();
            foreach (WorkGroupItem workGroupItem in GetWorkGroups(passport))
            {
                subMenu = GetWorkGroupMenu((short)workGroupItem.ID, passport);
                foreach (var table in subMenu)
                {
                    var lstViewItems = GetViewsByTableName(table.TableName, passport);

                    foreach (var V in lstViewItems)
                    {
                        if (lst.Where(a => a.viewId == V.Id).Count() == 0)
                        {
                            lst.Add(new ListOfviews() { viewName = V.ViewName, viewId = V.Id, userName = table.UserName, tableName = table.TableName });
                        }

                    }
                }
            }
            return lst;
        }
        public class ListOfviews
        {
            public string viewName { get; set; }
            public int viewId { get; set; }
            public string userName { get; set; }
            public string tableName { get; set; }
        }

        public static RecordsManage.ViewColumnsDataTable GetViewColumns(int viewId, Passport passport)
        {
            using (var viewColAdapter = new RecordsManageTableAdapters.ViewColumnsTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    viewColAdapter.Connection = conn;
                    return viewColAdapter.GetViewColumnInfo(viewId);
                }
            }
        }

        public static List<RecordsManage.ViewColumnsRow> GetsortableFields(int viewid, Passport passport)
        {
            RecordsManage.ViewColumnsDataTable columnSort = GetViewColumns(viewid, passport);
            return columnSort.Where(x => x.SortOrder > 0).OrderBy(x => x.SortOrder).ToList();
        }

        public static int GetAltViewID(int viewId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT ISNULL([AltViewId], 0) FROM [Views] WHERE [ID] = @viewId", conn))
                {
                    cmd.Parameters.AddWithValue("@viewId", viewId);
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
        }

        public static string GetViewColumnNameByOrdinal(int viewId, int column, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT [FieldName] FROM [ViewColumns] WHERE [ViewsID] = @ViewID AND [ColumnNum] = @ColumnNum", conn))
            {
                cmd.Parameters.AddWithValue("@ViewID", viewId);
                cmd.Parameters.AddWithValue("@ColumnNum", column);
                return cmd.ExecuteScalar().ToString();
            }
        }

        public static string GetViewColumnNameByOrdinal(int viewId, int column, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetViewColumnNameByOrdinal(viewId, column, conn);
            }
        }

        public static string GetGrandParentLookupField(int viewId, string fieldName, int columnNumber, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT [FieldName] FROM [ViewColumns] WHERE [ViewsID] = @viewId AND [ColumnNum] = @columnNum", conn))
            {
                cmd.Parameters.AddWithValue("@viewId", viewId);
                cmd.Parameters.AddWithValue("@columnNum", columnNumber);

                try
                {
                    return cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
        }

        public static string GetGrandParentLookupField(int viewId, string fieldName, int columnNumber, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetGrandParentLookupField(viewId, fieldName, columnNumber, conn);
            }
        }

        public static DataRow GetTableInfo(string tableName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName))
                return null;

            using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE TableName = @tableName", conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    // Modified By Hemin   
                    throw new NullReferenceException(string.Format("The Table \"{0}\" could not be found or is not available.", tableName)); // Modified By Nikunj - resource file key "msgNavigationTblCouldNotBeFoundWithTblName" value not found from Window service
                                                                                                                                             // Throw New NullReferenceException(String.Format(Languages.Translation("msgNavigationTblCouldNotBeFoundWithTblName"), tableName))
                }
            }
        }

        public static DataRow GetTableInfo(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTableInfo(tableName, conn);
            }
        }

        public static DataTable GetAllTableInfo(SqlConnection conn)
        {
            var dt = new DataTable();
            using (var cmd = new SqlCommand("SELECT * FROM Tables", conn))
            {
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetMultipleTableInfo(List<string> tableNames, SqlConnection conn)
        {
            var dt = new DataTable();
            string tables = string.Empty;

            foreach (var tableName in tableNames)
            {
                if (!string.IsNullOrEmpty(tableName))
                {
                    if (!string.IsNullOrEmpty(tables))
                        tables += ", ";
                    tables += "'" + tableName + "'";
                }
            }

            if (!string.IsNullOrEmpty(tables))
            {
                using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE TableName IN (" + tables + ")", conn))
                {
                    var da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
            // Modified By Hemin
            // Throw New NullReferenceException(String.Format("A Table could not be found or is not available."))
            throw new NullReferenceException(string.Format("A Table could not be found or is not available"));
        }

        public static DataTable GetMultipleTableInfo(List<string> tableNames, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetMultipleTableInfo(tableNames, conn);
            }
        }

        public static DataRow GetTableInfoByUserName(string tableUserName, Passport passport)
        {
            if (string.IsNullOrEmpty(tableUserName))
                return null;

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE UserName = @userName", conn))
                {
                    cmd.Parameters.AddWithValue("@userName", tableUserName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        throw new NullReferenceException(string.Format("A table with User Name \"{0}\" could not be found or is not available.", tableUserName));
                    }
                }
            }
        }

        public static DataRow GetViewInfo(int viewId, Passport passport)
        {
            if (viewId == 0)
                return null;

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand("select * from views where Id=@ViewID", conn))
                {
                    cmd.Parameters.AddWithValue("@ViewID", viewId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        // Modified by hemin
                        // Throw New NullReferenceException(String.Format("The Table ""{0}"" could not be found or is not available.", viewId))
                        throw new NullReferenceException(string.Format("The Table {0} could not be found or is not available", viewId));
                    }
                }
            }
        }

        public static RecordsManage.TablesDataTable GetAllTables(Passport passport)
        {
            using (var da = new RecordsManageTableAdapters.TablesTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    da.Connection = conn;
                    return da.GetData();
                }
            }
        }

        public static RecordsManage.TablesDataTable GetTypedTableInfo(string tableName, Passport passport)
        {
            using (var da = new RecordsManageTableAdapters.TablesTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    da.Connection = conn;
                    return da.GetTableInfoObsolute(tableName);
                }
            }
        }

        public static RecordsManage.TablesDataTable GetTrackingTables(Passport passport)
        {
            using (var da = new RecordsManageTableAdapters.TablesTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    da.Connection = conn;
                    return da.GetTrackingTables();
                }
            }
        }

        public static DataTable GetTrackingTablesAsDataTable(SqlConnection conn)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Tables WHERE TrackingTable > 0 ORDER BY TrackingTable", conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetTrackingTablesAsDataTable(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTrackingTablesAsDataTable(conn);
            }
        }

        public static int GetTrackingTableCount(Passport passport)
        {
            try
            {
                return GetTrackingTables(passport).Rows.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public static DataTable GetRequestableTables(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                string sql = "SELECT * FROM [Tables] where TableName IN (SELECT Name from SecureObject where BaseID=@TablesId and SecureObjectId in (select SecureObjectID from SecureObjectPermission where PermissionID=@PermissionID))";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TablesID", SecureObject.SecureObjectType.Table);
                    cmd.Parameters.AddWithValue("@PermissionID", Permissions.Permission.Request);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static bool FieldIsAMemo(string tableName, string fieldName, Passport passport)
        {
            var length = default(int);
            if (!IsAStringType(GetFieldType(tableName, fieldName, ref length, passport)))
                return false;
            return length <= 0 | length > 8000;
        }

        public static bool FieldIsAString(string tableName, Passport passport)
        {
            string idFieldName;

            try
            {
                idFieldName = GetPrimaryKeyFieldName(tableName, passport);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                idFieldName = "Id";
            }

            return IsAStringType(GetFieldType(tableName, idFieldName, passport));
        }

        public static bool FieldIsAString(string tableName, SqlConnection conn)
        {
            string idFieldName;

            try
            {
                idFieldName = GetPrimaryKeyFieldName(tableName, conn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                idFieldName = "Id";
            }

            var fieldLength = default(int);
            return IsAStringType(GetFieldType(tableName, idFieldName, ref fieldLength, conn));
        }

        public static bool FieldIsAString(DataRow tableInfo, SqlConnection conn)
        {
            string idFieldName;

            try
            {
                idFieldName = GetPrimaryKeyFieldName(tableInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                idFieldName = "Id";
            }

            var fieldLength = default(int);
            return IsAStringType(GetFieldType(tableInfo["TableName"].ToString(), idFieldName, ref fieldLength, conn));
        }

        public static bool FieldIsAString(DataRow tableInfo, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return FieldIsAString(tableInfo, conn);
            }
        }

        public static bool FieldIsAString(DataRow tableInfo, string fieldName, Passport Passport)
        {
            return IsAStringType(GetFieldType(tableInfo["TableName"].ToString(), fieldName, Passport));
        }

        public static bool FieldIsAString(DataRow tableInfo, string fieldName, SqlConnection conn)
        {
            var fieldLength = default(int);
            return IsAStringType(GetFieldType(tableInfo["TableName"].ToString(), fieldName, ref fieldLength, conn));
        }

        public static bool FieldIsAString(string tableName, string fieldName, Passport passport)
        {
            return IsAStringType(GetFieldType(tableName, fieldName, passport));
        }

        public static Type GetFieldType(string tableName, string fieldName, Passport passport)
        {
            var fieldLength = default(int);
            return GetFieldType(tableName, fieldName, ref fieldLength, passport);
        }

        public static Type GetFieldType(string tableName, string fieldName, SqlConnection conn)
        {
            var fieldLength = default(int);
            return GetFieldType(tableName, fieldName, ref fieldLength, conn);
        }

        public static Type GetFieldType(string tableName, string fieldName, ref int fieldLength, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(fieldName) || conn is null)
                return typeof(int);

            fieldLength = 1;
            fieldName = MakeSimpleField(fieldName);
            var dt = new DataTable();

            try
            {
                using (var cmd = new SqlCommand(string.Format("SELECT [{0}] FROM [{1}] WHERE 0 = 1", fieldName, tableName), conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                        da.FillSchema(dt, SchemaType.Source);
                        fieldLength = dt.Columns[fieldName].MaxLength;
                        return dt.Columns[fieldName].DataType;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return typeof(int);
            }
        }

        public static Type GetFieldType(string tableName, string fieldName, ref int fieldLength, Passport passport)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(fieldName) || passport is null)
                return typeof(int);
            return GetFieldType(tableName, fieldName, ref fieldLength, passport.StaticConnection());
        }

        public static bool IsADateType(Type type)
        {
            if (ReferenceEquals(type, typeof(DateTime)))
                return true;
            if (ReferenceEquals(type, typeof(DateTime)))
                return true;
            if (ReferenceEquals(type, typeof(TimeSpan)))
                return true;
            return false;
        }

        public static bool IsAStringType(Type type)
        {
            if (ReferenceEquals(type, typeof(char)))
                return true;
            if (ReferenceEquals(type, typeof(string)))
                return true;
            return false;
        }

        public static int GetItemCount(string tableName, string keyField, string keyValue, Passport passport)
        {
            return 0;
            // Using conn As SqlConnection = passport.Connection
            // Using cmd As New SqlCommand(String.Format("SELECT COUNT(*) FROM {0} WHERE [{1}] = @keyValue", tableName, keyField), conn)
            // cmd.Parameters.AddWithValue("@keyValue", keyValue)
            // Return CInt(cmd.ExecuteScalar)
            // End Using
            // End Using
        }

        public static string GetItemName(string tableName, string tableId, Passport passport, SqlConnection conn, DataRow drTableInfo = null)
        {
            return GetItemName(tableName, tableId, passport, conn, false, drTableInfo);
        }

        public static string GetItemName(string tableName, string tableId, Passport passport, DataRow drTableInfo = null)
        {
            using (var conn = passport.Connection())
            {
                return GetItemName(tableName, tableId, passport, false, drTableInfo);
            }
        }

        public static string GetItemName(string tableName, string tableId, Passport passport, SqlConnection conn, bool includePrefix, DataRow drTableInfo = null)
        {
            if (string.IsNullOrEmpty(tableName))
                return tableName + "," + tableId;
            if (string.IsNullOrEmpty(tableId))
                return tableName + "," + tableId;
            if (tableId == "~newrecord")
                return "Adding new record to " + tableName;

            string desc1 = string.Empty;
            string desc2 = string.Empty;
            string descFieldName1 = string.Empty;
            string descFieldName2 = string.Empty;

            if (drTableInfo is null)
                drTableInfo = GetTableInfo(tableName, conn);

            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameOne"].ToString()))
            {
                if ((drTableInfo["DescFieldNameOne"].ToString() ?? "") == (GetPrimaryKeyFieldName(drTableInfo) ?? "") & drTableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc1 = BarcodeText(drTableInfo, tableId);
                }
                else
                {
                    descFieldName1 = drTableInfo["DescFieldNameOne"].ToString();

                }
            }

            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameTwo"].ToString()))
            {
                if ((drTableInfo["DescFieldNameTwo"].ToString() ?? "") == (GetPrimaryKeyFieldName(drTableInfo) ?? "") & drTableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc2 = BarcodeText(drTableInfo, tableId);
                }
                else
                {
                    descFieldName2 = drTableInfo["DescFieldNameTwo"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(descFieldName1) || !string.IsNullOrEmpty(descFieldName2))
            {
                string sql = "SELECT ";
                // cmd = New SqlCommand(String.Format("select [{2}] from [{0}] where {1} = @TableID", tableName, MakeSimpleField(drTableInfo("IdFieldName").ToString), drTableInfo("DescFieldNameOne").ToString), passport.Connection)
                if (!string.IsNullOrEmpty(descFieldName1))
                {
                    sql += drTableInfo["DescFieldNameOne"].ToString();
                    if (!string.IsNullOrEmpty(descFieldName2))
                        sql += ", " + drTableInfo["DescFieldNameTwo"].ToString();
                }
                else
                {
                    sql += drTableInfo["DescFieldNameTwo"].ToString();
                }
                sql += " FROM " + tableName + " WHERE " + MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + " = @tableId";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tableId", tableId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var result = new DataTable();
                        da.Fill(result);

                        try
                        {
                            if (!string.IsNullOrEmpty(descFieldName1))
                            {
                                desc1 = result.AsEnumerable().ElementAtOrDefault(0)[drTableInfo["DescFieldNameOne"].ToString()].ToString();
                                if (!string.IsNullOrEmpty(descFieldName2))
                                {
                                    desc2 = result.AsEnumerable().ElementAtOrDefault(0)[drTableInfo["DescFieldNameTwo"].ToString()].ToString();
                                }
                            }
                            else
                            {
                                desc2 = result.AsEnumerable().ElementAtOrDefault(0)[drTableInfo["DescFieldNameTwo"].ToString()].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(desc1) & string.IsNullOrEmpty(desc2))
            {
                return DisplayFieldsNotConfigured(desc1, desc2, tableName, tableId, drTableInfo, passport);
            }

            if (includePrefix)
            {
                if (!string.IsNullOrEmpty(desc1))
                {
                    string prefix1 = drTableInfo["DescFieldPrefixOne"].ToString();
                    if (!string.IsNullOrEmpty(prefix1))
                        desc1 = prefix1 + " " + desc1;
                }
                if (!string.IsNullOrEmpty(desc2))
                {
                    string prefix2 = drTableInfo["DescFieldPrefixTwo"].ToString();
                    if (!string.IsNullOrEmpty(prefix2))
                        desc2 = prefix2 + " " + desc2;
                }
            }

            if (string.IsNullOrEmpty(desc1))
                return desc2;
            if (string.IsNullOrEmpty(desc2))
                return desc1;
            return desc1 + " " + desc2;
        }

        public static string GetItemName(string tableName, string tableId, Passport passport, bool includePrefix, DataRow drTableInfo = null)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                conn.Open();
                return GetItemName(tableName, tableId, passport, conn, includePrefix, drTableInfo);
            }
        }

        public static DataTable GetItemNames(string tableName, SqlConnection conn, DataRow drTableInfo = null, string ids = "")
        {
            if (string.IsNullOrEmpty(tableName))
                return null; // tableName & "," & tableID
            string desc1Clause = "'' as desc1, ";
            string desc2Clause = "'' as desc2, ";

            if (drTableInfo is null)
                drTableInfo = GetTableInfo(tableName, conn);
            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameOne"].ToString()))
                desc1Clause = drTableInfo["DescFieldNameOne"].ToString() + " AS desc1, ";
            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameTwo"].ToString()))
                desc2Clause = drTableInfo["DescFieldNameTwo"].ToString() + " AS desc2, ";

            string sql = "SELECT " + desc1Clause + desc2Clause + MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + " AS id FROM [" + tableName + "]";
            if (!string.IsNullOrEmpty(ids))
                sql += " WHERE " + MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + " IN (" + ids + ")";

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

        public static DataTable GetItemNamesFromView(string viewName, string Tablename, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(viewName))
                return null;
            string desc1Clause = "'' as desc1, ";
            string desc2Clause = "'' as desc2, ";

            var drTableInfo = GetTableInfo(Tablename, conn);
            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameOne"].ToString()))
                desc1Clause = drTableInfo["DescFieldNameOne"].ToString() + " AS desc1, ";
            if (!string.IsNullOrEmpty(drTableInfo["DescFieldNameTwo"].ToString()))
                desc2Clause = drTableInfo["DescFieldNameTwo"].ToString() + " AS desc2, ";

            string sql = "SELECT " + desc1Clause + desc2Clause + MakeSimpleField(drTableInfo["IdFieldName"].ToString()) + " AS id FROM [" + viewName + "]";

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


        public static DataTable GetItemNames(string tableName, Passport passport, DataRow drTableInfo = null, string ids = "")
        {
            using (var conn = passport.Connection())
            {
                return GetItemNames(tableName, conn, drTableInfo, ids);
            }
        }
        public static string ExtractItemName(string tableName, string tableId, Dictionary<string, DataTable> descriptions, DataTable tablesInfo, Passport Passport)
        {
            var itemNameInfo = tablesInfo.Select("TableName='" + tableName + "'");
            if (itemNameInfo.Count() > 0)
            {
                var itemNames = new DataTable();

                if (descriptions.TryGetValue(tableName, out itemNames))
                {
                    var itemName = itemNames.Select("id='" + tableId.Replace("'", "''") + "'");
                    if (itemName.Count() > 0)
                    {
                        return ItemNamesRowToItemName(itemName[0], itemNameInfo[0], Passport, tableId);
                    }
                    else
                    {
                        return GetItemName(tableName, tableId, Passport, itemNameInfo[0]);
                    }
                }
                else
                {
                    return GetItemName(tableName, tableId, Passport, itemNameInfo[0]);
                }
            }
            else
            {
                return GetItemName(tableName, tableId, Passport);
            }
        }

        public static string ItemNamesRowToItemName(DataRow itemRow, DataRow tableInfo, Passport Passport, string id)
        {
            string desc1 = "";
            string desc2 = "";

            if (!string.IsNullOrEmpty(tableInfo["DescFieldNameOne"].ToString()))
            {
                if ((tableInfo["DescFieldNameOne"].ToString() ?? "") == (GetPrimaryKeyFieldName(tableInfo) ?? "") & tableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc1 = BarcodeText(tableInfo, id);
                }
                else
                {
                    desc1 = itemRow["desc1"].ToString();
                }
            }
            if (!string.IsNullOrEmpty(tableInfo["DescFieldNameTwo"].ToString()))
            {
                if ((tableInfo["DescFieldNameTwo"].ToString() ?? "") == (GetPrimaryKeyFieldName(tableInfo) ?? "") & tableInfo["BarCodePrefix"].ToString().Length > 0)
                {
                    desc2 = BarcodeText(tableInfo, id.ToString());
                }
                // 'Changed by Hasmukh on 06/15/2016 for date format changes
                // desc2 = itemRow("desc2").ToString
                else if (IsADateType(itemRow["desc2"].GetType()))
                {
                    desc2 = DateFormat.get_ConvertCultureDate(itemRow["desc2"].ToString());
                }
                else
                {
                    desc2 = itemRow["desc2"].ToString();
                }
            }

            if (string.IsNullOrEmpty(desc1) & string.IsNullOrEmpty(desc2))
            {
                return DisplayFieldsNotConfigured(desc1, desc2, tableInfo["TableName"].ToString(), id, tableInfo, Passport);
            }
            else if (string.IsNullOrEmpty(desc1))
            {
                return desc2;
            }
            else if (string.IsNullOrEmpty(desc2))
            {
                return desc1;
            }
            else
            {
                return desc1 + " " + desc2;
            }
        }

        public static string ItemRowToItemName(DataRow itemRow, DataRow tableInfo, Passport Passport, string id, bool includePrefix = false)
        {
            string desc1 = "";
            string desc2 = "";

            if (!string.IsNullOrEmpty(tableInfo["DescFieldNameOne"].ToString()))
            {
                desc1 = itemRow[tableInfo["DescFieldNameOne"].ToString()].ToString();
            }
            if (!string.IsNullOrEmpty(tableInfo["DescFieldNameTwo"].ToString()))
            {
                desc2 = itemRow[tableInfo["DescFieldNameTwo"].ToString()].ToString();
            }
            if (includePrefix)
            {
                if (!string.IsNullOrEmpty(desc1))
                {
                    string prefix1 = tableInfo["DescFieldPrefixOne"].ToString();
                    if (!string.IsNullOrEmpty(prefix1))
                        desc1 = prefix1 + " " + desc1;
                }
                if (!string.IsNullOrEmpty(desc2))
                {
                    string prefix2 = tableInfo["DescFieldPrefixTwo"].ToString();
                    if (!string.IsNullOrEmpty(prefix2))
                        desc2 = prefix2 + " " + desc2;
                }
            }
            if (string.IsNullOrEmpty(desc1) & string.IsNullOrEmpty(desc2))
            {
                return DisplayFieldsNotConfigured(desc1, desc2, tableInfo["TableName"].ToString(), id, tableInfo, Passport);
            }
            else if (string.IsNullOrEmpty(desc1))
            {
                return desc2;
            }
            else if (string.IsNullOrEmpty(desc2))
            {
                return desc1;
            }
            else
            {
                return desc1 + " " + desc2;
            }
        }

        public static string BarcodeText(DataRow row, string tableId)
        {
            // Dim length As Integer = 6
            // Dim type As System.Type = GetFieldType(row("TableName").ToString, row("IdFieldName").ToString, passport)
            // If Not IsAStringType(type) AndAlso Not IsADateType(type) Then Return row("BarCodePrefix").ToString & StripLeadingZeros(tableId).PadLeft(length, "0"c)
            return row["BarCodePrefix"].ToString() + Strings.Right("00000" + tableId, 6);
        }

        public static string DisplayFieldsNotConfigured(string desc1, string desc2, string tableName, string tableId, DataRow row, Passport passport)
        {
            var type = GetFieldType(tableName, row["IdFieldName"].ToString(), passport);
            if (!IsAStringType(type) && !IsADateType(type))
                tableId = StripLeadingZeros(tableId);
            if (!string.IsNullOrEmpty(row["UserName"].ToString()))
                tableName = row["UserName"].ToString();

            if (desc1 is null & desc2 is null)
                return tableName + " (" + tableId + ") Display fields are not configured.";
            return tableName + " (" + tableId + ")";
        }

        public static string GetViewTableName(int viewId, SqlConnection conn)
        {
            using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                viewAdapter.Connection = conn;
                var res = viewAdapter.GetViewTableName(viewId);
                return res[0]["TableName"].ToString();
            }
        }

        public static string GetViewTableName(int viewId, Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                return GetViewTableName(viewId, conn);
            }
        }

        public static string GetViewName(int ViewId, SqlConnection conn)
        {
            using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                viewAdapter.Connection = conn;

                try
                {
                    var dr = viewAdapter.GetViewByViewID(ViewId).Rows[0];
                    return dr["ViewName"].ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
        }

        public static string GetViewName(int ViewId, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetViewName(ViewId, conn);
            }
        }

        public static bool ViewExists(string ViewName, SqlConnection conn)
        {
            using (var cmd = new SqlCommand("IF EXISTS(select * FROM sys.views where name = @Name) Select 1 Else Select 0", conn))
            {
                cmd.Parameters.AddWithValue("@Name", ViewName);
                return CBoolean(cmd.ExecuteScalar());
            }
        }

        public static bool ViewExists(string ViewName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return ViewExists(ViewName, conn);
            }
        }

        public static Enums.eLabelExists LabelExists(string TableName, SqlConnection conn)
        {
            var labelTypeExists = Enums.eLabelExists.None;

            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM OneStripJobs WHERE TableName = @TableName AND Inprint = @Inprint", conn))
            {
                cmd.Parameters.AddWithValue("@TableName", TableName);
                cmd.Parameters.AddWithValue("@Inprint", 0);
                try
                {
                    if (Conversions.ToInteger(cmd.ExecuteScalar()) > 0)
                        labelTypeExists = Enums.eLabelExists.BlackAndWhite;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TableName", TableName);
                cmd.Parameters.AddWithValue("@Inprint", 5);
                try
                {
                    if (Conversions.ToInteger(cmd.ExecuteScalar()) > 0)
                        labelTypeExists = labelTypeExists | Enums.eLabelExists.Color;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return labelTypeExists;
            }
        }

        public static Enums.eLabelExists LabelExists(string TableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return LabelExists(TableName, conn);
            }
        }

        public static string GetFieldDataType(string tableName, string fieldName, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(string.Format("select TOP 1 [{0}] from [{1}]", MakeSimpleField(fieldName), tableName), conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt.Columns[0].DataType.ToString();
                }
            }
        }

        public static string GetFieldDataType(string tableName, string fieldName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetFieldDataType(tableName, fieldName, conn);
            }
        }

        public static string GetPrimaryKeyFieldName(string tableName, SqlConnection conn) // ByVal dr As DataRow
        {
            return GetPrimaryKeyFieldName(GetTableInfo(tableName, conn));
        }

        public static string GetPrimaryKeyFieldName(string tableName, Passport passport) // ByVal dr As DataRow
        {
            return GetPrimaryKeyFieldName(GetTableInfo(tableName, passport));
        }

        public static string GetPrimaryKeyFieldName(DataRow dr)
        {
            if (dr["IdFieldName"].ToString().Contains("."))
            {
                return dr["IdFieldName"].ToString().Substring(Strings.InStr(dr["IdFieldName"].ToString(), ".", CompareMethod.Text));
            }
            else
            {
                return dr["IdFieldName"].ToString();
            }
        }

        public static int GetTableDefaultViewId(string tableName, SqlConnection conn)
        {
            using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                viewAdapter.Connection = conn;
                return Conversions.ToInteger(viewAdapter.GetViewsByTableName(tableName).Rows[0]["Id"]);
            }
        }

        public static int GetTableDefaultViewId(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTableDefaultViewId(tableName, conn);
            }
        }

        public static int GetTableFirstSearchableViewId(string tableName, Passport passport, SqlConnection conn)
        {
            using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                viewAdapter.Connection = conn;

                foreach (RecordsManage.ViewsRow row in viewAdapter.GetSearchableViewsByTableName(tableName).Rows)
                {
                    if (passport.CheckPermission(row.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.View))
                    {
                        return row.Id;
                    }
                }
            }

            return 0;
        }

        public static int GetTableFirstSearchableViewId(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTableFirstSearchableViewId(tableName, passport, conn);
            }
        }

        public static int GetTableFirstEligibleViewId(string tableName, Passport passport, SqlConnection conn)
        {
            using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
            {
                viewAdapter.Connection = conn;

                foreach (RecordsManage.ViewsRow row in viewAdapter.GetViewsByTableName(tableName).Rows)
                {
                    if (passport.CheckPermission(row.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.View))
                    {
                        return row.Id;
                    }
                }
            }

            return 0;
        }

        public static int GetTableFirstEligibleViewId(string tableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetTableFirstEligibleViewId(tableName, passport, conn);
            }
        }

        public static string GetRetentionFieldName(string tableName, SqlConnection conn)
        {
            var dr = GetTableInfo(tableName, conn);

            if (dr["RetentionFieldName"].ToString().Contains("."))
            {
                return dr["RetentionFieldName"].ToString().Substring(Strings.InStr(dr["RetentionFieldName"].ToString(), ".", CompareMethod.Text));
            }
            else
            {
                return dr["RetentionFieldName"].ToString();
            }
        }

        public static string GetRetentionFieldName(string tableName, Passport passport)
        {
            var dr = GetTableInfo(tableName, passport);

            if (dr["RetentionFieldName"].ToString().Contains("."))
            {
                return dr["RetentionFieldName"].ToString().Substring(Strings.InStr(dr["RetentionFieldName"].ToString(), ".", CompareMethod.Text));
            }
            else
            {
                return dr["RetentionFieldName"].ToString();
            }
        }

        // Public Shared Function GetDefaultRetentionId(ByVal tableName As String, ByVal passport As Passport) As String
        // Dim dr As DataRow = GetTableInfo(tableName, passport)
        // Return dr("DefaultRetentionId").ToString
        // End Function

        public static string InjectWhereIntoSQL(string sSQL, string sNewWhere, string sOperator = "AND")
        {
            string sInitGroupBy = string.Empty;
            string sInitWhere = string.Empty;
            string sInitOrderBy = string.Empty;
            string sInitSelect = string.Empty;
            int iFromPos;
            int iPos;
            string sRetVal = string.Empty;

            sRetVal = sSQL;

            iPos = Strings.InStrRev(sSQL, " WHERE ", -1, CompareMethod.Text);
            iFromPos = Strings.InStrRev(sSQL, " FROM ", -1, CompareMethod.Text);

            if (iPos > 0 && iPos > iFromPos)
            {
                sInitSelect = Strings.Trim(Strings.Left(sSQL, iPos));
                sInitWhere = Strings.Trim(Strings.Mid(sSQL, iPos + Strings.Len(" WHERE "), Strings.Len(sSQL)));

                iPos = Strings.InStr(sInitWhere, " ORDER BY ", CompareMethod.Text);
                if (iPos > 0)
                {
                    sInitOrderBy = Strings.Trim(Strings.Mid(sInitWhere, iPos + Strings.Len(" ORDER BY "), Strings.Len(sInitWhere)));
                    sInitWhere = Strings.Trim(Strings.Left(sInitWhere, iPos));
                }

                iPos = Strings.InStr(sInitWhere, " GROUP BY ", CompareMethod.Text);

                if (iPos > 0)
                {
                    sInitGroupBy = Strings.Trim(Strings.Mid(sInitWhere, iPos + Strings.Len(" GROUP BY "), Strings.Len(sInitWhere)));
                    sInitWhere = Strings.Trim(Strings.Left(sInitWhere, iPos));
                }
            }
            else
            {
                iPos = Strings.InStr(sSQL, " ORDER BY ", CompareMethod.Text);
                if (iPos > 0 && iPos > iFromPos)
                {
                    sInitOrderBy = Strings.Trim(Strings.Mid(sSQL, iPos + Strings.Len(" ORDER BY "), Strings.Len(sSQL)));
                    sInitSelect = Strings.Trim(Strings.Left(sSQL, iPos));
                }
                else
                {
                    sInitSelect = Strings.Trim(sSQL);
                }

                iPos = Strings.InStr(sInitSelect, " GROUP BY ", CompareMethod.Text);

                if (iPos > 0 && iPos > iFromPos)
                {
                    sInitGroupBy = Strings.Trim(Strings.Mid(sInitSelect, iPos + Strings.Len(" GROUP BY "), Strings.Len(sInitSelect)));
                    sInitSelect = Strings.Trim(Strings.Left(sInitSelect, iPos));
                }
            }

            sRetVal = sInitSelect;

            if (!string.IsNullOrEmpty(sInitWhere))
            {
                if (!string.IsNullOrEmpty(Strings.Trim(sNewWhere)))
                {
                    sRetVal += " WHERE ( " + ParenEncloseStatement(sInitWhere) + " " + sOperator + " " + ParenEncloseStatement(sNewWhere) + " )";
                }
                else
                {
                    sRetVal += " WHERE " + ParenEncloseStatement(sInitWhere);
                }
            }
            else if (!string.IsNullOrEmpty(Strings.Trim(sNewWhere)))
            {
                sRetVal += " WHERE " + ParenEncloseStatement(sNewWhere);
            }

            if (!string.IsNullOrEmpty(sInitGroupBy))
                sRetVal += " GROUP BY " + sInitGroupBy;
            if (!string.IsNullOrEmpty(sInitOrderBy))
                sRetVal += " ORDER BY " + sInitOrderBy;
            return sRetVal;
        }

        private static string ParenEncloseStatement(string sSQL)
        {
            long iParenCount = 0L;
            long iMaxParenCount = 0L;
            int iIndex = 1;
            bool bDoEnclose = false;
            bool bInString = false;
            string sCurChar;

            while (iIndex <= Strings.Len(sSQL) & !bDoEnclose)
            {
                sCurChar = Strings.Mid(sSQL, iIndex, 1);
                if (sCurChar == "\"")
                    bInString = !bInString;

                if (!bInString)
                {
                    if (sCurChar == "(")
                        iParenCount = iParenCount + 1L;
                    if (sCurChar == ")")
                        iParenCount = iParenCount - 1L;
                }

                if (iParenCount > iMaxParenCount)
                    iMaxParenCount = iParenCount;
                if (iParenCount == 0L & iIndex > 1 & iIndex < Strings.Len(sSQL) & iMaxParenCount > 0L)
                    bDoEnclose = true;

                iIndex = iIndex + 1;
            }

            if (iMaxParenCount == 0L)
                bDoEnclose = true;
            if (bDoEnclose)
                return "( " + sSQL + " )";
            return sSQL;
        }

        internal static void GetRetentionPermissions(int userId, ref bool inactive, ref bool archived, ref bool destroyed, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetRetentionPermissionsForUser, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                            return;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row["Name"].ToString().ToLower().Contains("inactive"))
                                inactive = true;
                            if (row["Name"].ToString().ToLower().Contains("archive"))
                                archived = true;
                            if (row["Name"].ToString().ToLower().Contains("destroy"))
                                destroyed = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static bool IsTaskListOffForUser(Passport passport)
        {
            string rtn = GetSetting("TaskListOff", passport.UserId.ToString(), passport);
            if (string.IsNullOrEmpty(rtn))
                rtn = GetSetting("TaskListOff", "*", passport);
            if (string.IsNullOrEmpty(rtn))
                return false;

            try
            {
                if (Information.IsNumeric(rtn))
                    return Conversions.ToInteger(rtn) != 0;
                return Conversions.ToBoolean(rtn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        // aspx model for tasks moti mashiah
        public static List<string> GetTasks(Passport passport)
        {
            return GetTasks(passport, true, true);
        }
        private static List<string> GetTasks(Passport passport, bool forceDisplay, bool checkIfTasksAreOff)
        {
            var list = new List<string>();

            if (!forceDisplay || checkIfTasksAreOff)
            {
                if (IsTaskListOffForUser(passport))
                    return list;
            }

            var dataTable = new RecordsManage.ViewsDataTable();
            string taskSQL = GetSetting("TaskList", "SQL", passport);
            string username = new User(passport, true).UserName;

            using (var conn = passport.Connection())
            {
                if (string.IsNullOrEmpty(taskSQL))
                {
                    using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
                    {
                        viewAdapter.Connection = conn;
                        dataTable = viewAdapter.GetTasks();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(taskSQL, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dataTable);
                        }
                    }
                }

                var count = default(int);
                foreach (RecordsManage.ViewsRow row in dataTable.Rows)
                {
                    if (passport.CheckPermission(row.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.View))
                    {
                        string sql = Navigation.NormalizeString(row.SQLStatement).ToLower();

                        if (!sql.StartsWith("select top") && !sql.StartsWith("select distinct top") && !sql.StartsWith("select distinctrow top"))
                        {
                            sql = sql.Replace("select ", "select top 100 percent ");
                        }
                        sql = Strings.Replace(sql, "@@SL_ROWCOUNT", count.ToString(), 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@SL_ViewName", row.ViewName, 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@SL_UserName", username, 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@Today", DateTime.Today.ToString("d"), 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@Now", DateTime.Today.ToString("g"), 1, -1, CompareMethod.Text);

                        using (var cmd = new SqlCommand(CountSQLStatement(sql), conn))
                        {
                            // Using cmd As New SqlCommand(String.Format("SELECT COUNT(*) FROM ({0}) s", sql), passport.Connection)
                            try
                            {
                                count = Conversions.ToInteger(cmd.ExecuteScalar());
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                count = 0;
                            }

                            if (count > 0)
                            {
                                string display = row.TaskListDisplayString;
                                if (!string.IsNullOrEmpty(display))
                                {
                                    display = Strings.Replace(display, "@@SL_ROWCOUNT", count.ToString(), 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@SL_ViewName", row.ViewName, 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@SL_UserName", username, 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@Today", DateTime.Today.ToString("d"), 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@Now", DateTime.Today.ToString("g"), 1, -1, CompareMethod.Text);
                                    list.Add(string.Format("<a style='color: blue;' href='handler.aspx?tasks=1&viewid={0}'>{1}</a>", (object)row.Id, ChangePlurality(display, count == 1)));
                                }
                            }
                        }
                    }
                }

                return list;
            }
        }
        // add new identical function for mvc model for task. moti mashiah 
        public static List<string> GetTasksMvc(Passport passport)
        {
            return GetTasksMvc(passport, true, true);
        }
        private static List<string> GetTasksMvc(Passport passport, bool forceDisplay, bool checkIfTasksAreOff)
        {
            var list = new List<string>();

            if (!forceDisplay || checkIfTasksAreOff)
            {
                if (IsTaskListOffForUser(passport))
                    return list;
            }

            var dataTable = new RecordsManage.ViewsDataTable();
            string taskSQL = GetSetting("TaskList", "SQL", passport);
            string username = new User(passport, true).UserName;

            using (var conn = passport.Connection())
            {
                if (string.IsNullOrEmpty(taskSQL))
                {
                    using (var viewAdapter = new RecordsManageTableAdapters.ViewsTableAdapter())
                    {
                        viewAdapter.Connection = conn;
                        dataTable = viewAdapter.GetTasks();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(taskSQL, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dataTable);
                        }
                    }
                }

                var count = default(int);
                foreach (RecordsManage.ViewsRow row in dataTable.Rows)
                {
                    if (passport.CheckPermission(row.ViewName, SecureObject.SecureObjectType.View, Permissions.Permission.View))
                    {
                        string sql = Navigation.NormalizeString(row.SQLStatement).ToLower();

                        if (!sql.StartsWith("select top") && !sql.StartsWith("select distinct top") && !sql.StartsWith("select distinctrow top"))
                        {
                            sql = sql.Replace("select ", "select top 100 percent ");
                        }
                        sql = Strings.Replace(sql, "@@SL_ROWCOUNT", count.ToString(), 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@SL_ViewName", row.ViewName, 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@SL_UserName", username, 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@Today", DateTime.Today.ToString("d"), 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@Time", DateTime.Now.ToString("hh:mm tt"), 1, -1, CompareMethod.Text);
                        sql = Strings.Replace(sql, "@@Now", DateTime.Today.ToString("g"), 1, -1, CompareMethod.Text);

                        using (var cmd = new SqlCommand(CountSQLStatement(sql), conn))
                        {
                            // Using cmd As New SqlCommand(String.Format("SELECT COUNT(*) FROM ({0}) s", sql), passport.Connection)
                            try
                            {
                                count = Conversions.ToInteger(cmd.ExecuteScalar());
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                count = 0;
                            }

                            if (count > 0)
                            {
                                string display = row.TaskListDisplayString;
                                if (!string.IsNullOrEmpty(display))
                                {
                                    display = Strings.Replace(display, "@@SL_ROWCOUNT", count.ToString(), 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@SL_ViewName", row.ViewName, 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@SL_UserName", username, 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@Today", DateTime.Today.ToString("d"), 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@Time", DateTime.Now.ToString("hh:mm tt"), 1, -1, CompareMethod.Text);
                                    display = Strings.Replace(display, "@@Now", DateTime.Today.ToString("g"), 1, -1, CompareMethod.Text);
                                    // Dim testString As String = "taskbar.TaskbarLinks({0}, 1, this)"
                                    // If testString.Equals(110) Then
                                    // Ret
                                    // Else
                                    // Return cmd.ExecuteScalar.ToString
                                    // End If

                                    list.Add(string.Format("<a style='color: blue;' onclick=\"taskbar.TaskbarLinks({0}, 1, this)\"'>{1}</a>", (object)row.Id, ChangePlurality(display, count == 1)));
                                    // list.Add(String.Format("<a style='color: blue;' onclick=""getElementById(taskbar.TaskbarLinks(110, 1, this))"" '  href =""www.google.com"" target=""_blank"">{1}</a>", row.Id, ChangePlurality(display, count = 1)))
                                }
                            }
                        }
                    }
                }

                return list;
            }
        }

        private static string ChangePlurality(string displayString, bool singular)
        {
            if (string.IsNullOrEmpty(displayString))
                return string.Empty;
            string rtn = displayString;

            if (singular)
            {
                rtn = rtn.Replace("{s}", string.Empty);
                rtn = rtn.Replace("{S}", string.Empty);
                rtn = rtn.Replace("{es}", string.Empty);
                rtn = rtn.Replace("{Es}", string.Empty);
                rtn = rtn.Replace("{eS}", string.Empty);
                rtn = rtn.Replace("{ES}", string.Empty);
            }
            else
            {
                rtn = rtn.Replace("{s}", "s");
                rtn = rtn.Replace("{S}", "S");
                rtn = rtn.Replace("{es}", "es");
                rtn = rtn.Replace("{Es}", "Es");
                rtn = rtn.Replace("{eS}", "eS");
                rtn = rtn.Replace("{ES}", "ES");
            }

            rtn = ReplacePluralToken(rtn, "{are|is}", singular, 5, 2, 1, 3);
            rtn = ReplacePluralToken(rtn, "{is|are}", singular, 1, 2, 4, 3);
            rtn = ReplacePluralToken(rtn, "{have|has}", singular, 6, 3, 1, 4);
            rtn = ReplacePluralToken(rtn, "{has|have}", singular, 1, 3, 5, 4);
            return rtn;
        }

        private static string ReplacePluralToken(string displayString, string pluralToken, bool singular, int singularStart, int singularLength, int pluralStart, int pluralLength)
        {
            if (!displayString.ToLower().Contains(pluralToken.ToLower()))
                return displayString;

            int position = displayString.ToLower().IndexOf(pluralToken.ToLower());
            pluralToken = displayString.Substring(position, pluralToken.Length);
            if (singular)
                return string.Format("{0}{1}{2}", displayString.Substring(0, position), pluralToken.Substring(singularStart, singularLength), displayString.Substring(position + pluralToken.Length));
            return string.Format("{0}{1}{2}", displayString.Substring(0, position), pluralToken.Substring(pluralStart, pluralLength), displayString.Substring(position + pluralToken.Length));
        }

        public static string CountSQLStatement(string sql)
        {
            string sqlOut;
            int selectPos = sql.ToLower().IndexOf("select ");
            int fromPos = sql.ToLower().IndexOf(" from ");

            if (sql.Substring(selectPos + 6, fromPos - (selectPos + 6)).ToLower().Contains("select "))
            {
                sqlOut = sql.Substring(selectPos + 6);
                return CountSQLStatement("select " + sql.Substring(fromPos + 5));
            }
            else
            {
                return "SELECT COUNT(*)" + sql.Substring(fromPos);
            }
        }

        public static string GetDirectLookup(string lowerTableName, string lowerTableFieldName, string UpperId, string upperDisplayFieldName, Passport passport)
        {
            if (string.IsNullOrEmpty(lowerTableName) | string.IsNullOrEmpty(lowerTableFieldName) | string.IsNullOrEmpty(UpperId) | string.IsNullOrEmpty(upperDisplayFieldName))
            {
                return string.Empty;
            }

            using (var conn = passport.Connection())
            {
                var dt = GetUpperRelationship(lowerTableName, lowerTableFieldName, conn);
                if (dt.Rows.Count == 0)
                    return string.Empty;

                using (var cmd = new SqlCommand(string.Format("SELECT {2} FROM {1} WHERE {0} = @UpperID", dt.Rows[0]["UpperTableFieldName"].ToString(), dt.Rows[0]["UpperTableName"].ToString(), upperDisplayFieldName), conn))
                {
                    cmd.Parameters.AddWithValue("@upperId", UpperId);
                    try
                    {
                        if (cmd.ExecuteScalar() is null)
                        {
                            return string.Empty;
                        }
                        else
                        {
                            return cmd.ExecuteScalar().ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return string.Empty;
                    } // TODO: supressed errors for beta release.  Needs to do lookup without errors.
                }
            }
        }

        public static DataTable GetLowerRelationships(string UpperTableName, string UpperTableFieldName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(UpperTableName) | string.IsNullOrEmpty(UpperTableFieldName))
                return null;
            string sql = "SELECT LowerTableName, LowerTableFieldName FROM Relationships WHERE UpperTableName = @UpperTableName AND UpperTableFieldName = @UpperTableFieldName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UpperTableName", UpperTableName);
                cmd.Parameters.AddWithValue("@UpperTableFieldName", UpperTableName + "." + UpperTableFieldName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetLowerRelationships(string UpperTableName, string UpperTableFieldName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetLowerRelationships(UpperTableName, UpperTableFieldName, conn);
            }
        }

        public static DataTable GetLowerTableInfos(string UpperTableName, string UpperTableFieldName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(UpperTableName) | string.IsNullOrEmpty(UpperTableFieldName))
                return null;
            string sql = "SELECT t.*, LowerTableName, LowerTableFieldName FROM Relationships r, [Tables] t WHERE t.TableName=LowerTableName AND UpperTableName = @UpperTableName AND UpperTableFieldName = @UpperTableFieldName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UpperTableName", UpperTableName);
                cmd.Parameters.AddWithValue("@UpperTableFieldName", UpperTableName + "." + UpperTableFieldName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetLowerTableInfos(string UpperTableName, string UpperTableFieldName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetLowerTableInfos(UpperTableName, UpperTableFieldName, conn);
            }
        }

        public static DataTable GetUpperRelationship(string LowerTableName, string LowerTableFieldName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(LowerTableName) | string.IsNullOrEmpty(LowerTableFieldName))
                return null;
            string sql = "SELECT UpperTableName, UpperTableFieldName FROM Relationships WHERE LowerTableName = @LowerTableName AND LowerTableFieldName = @LowerTableFieldName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LowerTableName", LowerTableName);
                if (LowerTableFieldName.Contains("."))
                {
                    cmd.Parameters.AddWithValue("@LowerTableFieldName", LowerTableFieldName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@LowerTableFieldName", LowerTableName + "." + LowerTableFieldName);
                }

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetUpperRelationship(string LowerTableName, string LowerTableFieldName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetUpperRelationship(LowerTableName, LowerTableFieldName, conn);
            }
        }

        public static DataTable GetUpperRelationships(string LowerTableName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(LowerTableName))
                return null;
            string sql = "SELECT UpperTableName, UpperTableFieldName, lowerTableFieldName FROM Relationships WHERE LowerTableName = @LowerTableName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LowerTableName", LowerTableName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetUpperRelationships(string LowerTableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetUpperRelationships(LowerTableName, conn);
            }
        }

        public static string GetLowerTableForeignKeyField(string upperTableName, string lowerTableName, SqlConnection conn)
        {
            string sql = "SELECT LowerTableFieldName FROM Relationships WHERE LowerTableName = @LowerTableName AND UpperTableName = @UpperTableName";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@LowerTableName", lowerTableName);
                cmd.Parameters.AddWithValue("@UpperTableName", upperTableName);
                return cmd.ExecuteScalar().ToString();
            }
        }

        public static int CheckIftableHasRelations(string tableName, SqlConnection conn)
        {
            int hasRelationship = 0;
            string sql1 = "select COUNT(a.Id) from RelationShips a where a.UpperTableName = @tableName";
            string sql2 = "select COUNT(a.TableId) from Tables a where a.TableName = @tableName and (a.Attachments = 1 or a.Trackable = 1 or a.TrackingTable > 0)";
            using (var cmd = new SqlCommand(sql1, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                hasRelationship += Conversions.ToInteger(cmd.ExecuteScalar());
                cmd.CommandText = sql2;
                hasRelationship += Conversions.ToInteger(cmd.ExecuteScalar());
            }

            return hasRelationship;
        }

        public static string GetLowerTableForeignKeyField(string upperTableName, string lowerTableName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetLowerTableForeignKeyField(upperTableName, lowerTableName, conn);
            }
        }

        public static string MakeSimpleField(string complexFieldName)
        {
            // check raju
            // If complexFieldName.IndexOf(vbNullChar) >= 0 Then complexFieldName = complexFieldName.Substring(0, complexFieldName.IndexOf(vbNullChar))

            string str = Strings.Right(complexFieldName, complexFieldName.Length - Strings.InStr(complexFieldName, ".", CompareMethod.Text));
            str = Strings.Replace(str, "]", "");
            str = Strings.Replace(str, "[", "");
            return str;
        }

        public static string NormalizeString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            s = s.Replace(Constants.vbTab, " ");
            s = s.Replace(Constants.vbCr, " ");
            s = s.Replace(Constants.vbLf, " ");

            while (s.Contains("  "))
                s = s.Replace("  ", " ");

            return s;
        }

        public static void VerifyLegalDeletion(string tableName, string tableId, SqlConnection conn)
        {
            VerifyLegalDeletion(tableName, tableId, 1, conn);
        }

        public static void VerifyLegalDeletion(string tableName, List<string> tableIds, SqlConnection conn)
        {
            foreach (string tableId in tableIds)
                VerifyLegalDeletion(tableName, tableId, tableIds.Count, conn);
        }

        public static void VerifyLegalDeletion(string tableName, string tableId, int tableIdsCount, SqlConnection conn)
        {
            // do not delete if container with items
            if (IsArchivedOrDestroyed(tableName, tableId, Enums.meFinalDispositionStatusType.fdstArchived | Enums.meFinalDispositionStatusType.fdstDestroyed, conn))
            {
                if (tableIdsCount > 1)
                    throw new Exception("At least One Row is archived or destroyed.  Deletion not Allowed.");
                throw new Exception("Row is archived or destroyed.  Deletion not Allowed.");
            }

            VerifyLegalTrackingDeletion(tableName, tableId, conn);
        }

        public static void VerifyLegalDeletion(string tableName, List<string> tableIds, Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                conn.Open();
                VerifyLegalDeletion(tableName, tableIds, conn);
            }
        }
        public static async Task VerifyLegalDeletionAsync(string tableName, List<string> tableIds, Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                await conn.OpenAsync();
                VerifyLegalDeletion(tableName, tableIds, conn);
            }
        }

        private static void VerifyLegalTrackingDeletion(string tableName, string tableId, SqlConnection conn)
        {
            if (Conversions.ToInteger(GetTableInfo(tableName, conn)["TrackingTable"]) <= 0)
                return;
            string trackedTableId = PrepPad(tableName, tableId, conn);

            using (var cmd = new SqlCommand(string.Format("SELECT COUNT(*) FROM [TrackingStatus] WHERE [{0}] = @trackedTableId", Navigation.MakeSimpleField(Tracking.GetTrackedTableKeyField(tableName, conn))), conn))
            {
                cmd.Parameters.AddWithValue("@trackedTableId", trackedTableId);
                // Modified by hemin
                // If CInt(cmd.ExecuteScalar) > 0 Then Throw New Exception("At least one of the containers selected has trackable items in it.  Deletion has been aborted.")
                if (Conversions.ToInteger(cmd.ExecuteScalar()) > 0)
                    throw new Exception("At least one of the containers selected has trackable items in it.  Deletion has been aborted");
                // do not delete if there is tracking history
                cmd.CommandText = string.Format("SELECT COUNT(*) FROM [TrackingHistory] WHERE [{0}] = @trackedTableId", Navigation.MakeSimpleField(Tracking.GetTrackedTableKeyField(tableName, conn)));
                // Modified by hemin
                // If CInt(cmd.ExecuteScalar) > 0 Then Throw New Exception("At least one of the containers selected has trackable items with history.  Deletion has been aborted.")
                if (Conversions.ToInteger(cmd.ExecuteScalar()) > 0)
                    throw new Exception("At least one of the containers selected has trackable items with history.  Deletion has been aborted");
                // do not delete if default container 
                cmd.CommandText = "SELECT COUNT(*) FROM [Tables] WHERE [DefaultTrackingTable] = @tableName AND [DefaultTrackingId] = @tableId";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                // Modified by hemin
                // If CInt(cmd.ExecuteScalar) = 1 Then Throw New Exception("One of the containers is the default tracking location and cannot be deleted.")
                if (Conversions.ToInteger(cmd.ExecuteScalar()) == 1)
                    throw new Exception("One of the containers is the default tracking location and cannot be deleted");
            }
        }

        public class ForeignKeys
        {
            public string UpperKey;
            public string LowerKey;

            public ForeignKeys(string upperKey, string lowerKey)
            {
                UpperKey = upperKey;
                LowerKey = lowerKey;
            }
        }

        public static ForeignKeys GetForeignKeys(string LowerTableName, string UpperTableName, Passport Passport)
        {
            string sql = "SELECT UpperTableFieldName, LowerTableFieldName FROM Relationships WHERE UpperTableName = @UpperTableName AND LowerTableName = @LowerTableName";

            using (var conn = Passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UpperTableName", UpperTableName);
                    cmd.Parameters.AddWithValue("@LowerTableName", LowerTableName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return new ForeignKeys(dt.Rows[0]["UpperTableFieldName"].ToString(), dt.Rows[0]["LowerTableFieldName"].ToString());
                    }
                }
            }
        }

        public static DataTable GetListLookup(ref DataColumn column, Parameters @params, DataRow columnInfo, SqlConnection conn)
        {
            string sql;
            string lowerTableName = Navigation.GetViewTableName(@params.ViewId, conn);
            string lowerTableFieldName = columnInfo["FieldName"].ToString();
            var dtLookup = new DataTable(lowerTableFieldName);

            var dt = GetUpperRelationship(lowerTableName, lowerTableFieldName, conn);

            if (dt.Rows.Count == 1)
            {
                if (Conversions.ToInteger(columnInfo["lookupType"]) == 0)
                {
                    // direct dropdown list lookup
                    sql = "SELECT [" + GetPrimaryKeyFieldName(dt.Rows[0]["UpperTableName"].ToString(), conn) + "] AS Display FROM [" + dt.Rows[0]["UpperTableName"].ToString() + "] ORDER BY Display";
                }
                else
                {
                    // not getting hit
                    sql = "SELECT [" + lowerTableFieldName.Replace(".", "].[") + "] AS Display, [" + GetPrimaryKeyFieldName(dt.Rows[0]["UpperTableName"].ToString(), conn) + "] AS Value FROM [" + dt.Rows[0]["UpperTableName"].ToString() + "] ORDER BY Display, Value";
                }
            }
            else
            {
                try
                {
                    if (Conversions.ToShort(columnInfo["LookupType"]) != 0)
                    {
                        string lookupIdColumnName = Navigation.GetViewColumnNameByOrdinal(@params.ViewId, Conversions.ToInteger(columnInfo["lookupIdCol"]), conn);
                        if (@params.Data.Rows.Count > 0)
                            column.ExtendedProperties["LookupIdValue"] = @params.Data.Rows[0][MakeSimpleField(lookupIdColumnName)];
                        // Debug.Print("GetListLookup - lookupIdColumnName: {0}, LookupIdColumn: {1}, LookupIdValue: {2}, ColumnName: {3}", lookupIdColumnName, column.ExtendedProperties("LookupIdColumn"), column.ExtendedProperties("LookupIdValue"), column.ColumnName)
                        dt = GetUpperRelationship(lowerTableName, lookupIdColumnName, conn);
                        dtLookup.TableName = lookupIdColumnName;
                        sql = "SELECT [" + lowerTableFieldName.Replace(".", "].[") + "] AS Display, " + GetPrimaryKeyFieldName(dt.Rows[0]["UpperTableName"].ToString(), conn) + " AS Value FROM [" + dt.Rows[0]["UpperTableName"].ToString() + "] ORDER BY Display, Value";
                    }
                    // foriegn key lookup
                    // sql = "SELECT [" & DisplayField.Replace(".", "].[") & "] AS Display FROM [" & dt.Rows(0)("UpperTableName").ToString & "]"
                    else
                    {
                        // retention code lookup.
                        sql = "SELECT Id AS Value, Id AS Display FROM SLRetentionCodes ORDER BY Display";
                    }
                }
                catch (Exception ex)
                {
                    // retention code lookup.
                    Debug.WriteLine(ex.Message);
                    sql = "SELECT Id AS Value, Id AS Display FROM SLRetentionCodes ORDER BY Display";
                }
            }

            using (var cmd = new SqlCommand(sql, conn))
            {
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtLookup);
                    return dtLookup;
                }
            }
        }

        public static DataTable GetListLookup(ref DataColumn column, Parameters @params, DataRow columnInfo, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetListLookup(ref column, @params, columnInfo, conn);
            }
        }

        public static string GetEmployeeRequestorName(Passport passport, SqlConnection conn)
        {
            string requestorTable = Tracking.GetRequestorTableName(conn);
            var tblInfo = GetTableInfo(requestorTable, conn);
            // GetItemName(requestorTable, row("id").ToString, passport)
            if (string.IsNullOrEmpty(tblInfo["OperatorsIdField"].ToString()))
                return string.Empty;

            var user = new User(passport, true);

            using (var cmd = new SqlCommand("SELECT " + tblInfo["idfieldname"].ToString() + " FROM " + requestorTable + " WHERE " + tblInfo["OperatorsIdField"].ToString() + " = @userName", conn))
            {
                cmd.Parameters.AddWithValue("@userName", user.UserName);

                try
                {
                    return GetItemName(requestorTable, cmd.ExecuteScalar().ToString(), passport, conn);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
        }

        public static string GetEmployeeRequestorName(Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetEmployeeRequestorName(passport, conn);
            }
        }

        public static string GetUserEmail(Passport passport)
        {
            var user = new User(passport, true);
            if (!string.IsNullOrEmpty(user.Email))
            {
                try
                {
                    var mailAddress = new MailAddress(user.Email);
                    return user.Email; // If no error, then valid email
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            using (var conn = passport.Connection())
            {
                string requestorTable = Tracking.GetRequestorTableName(conn);
                var tblInfo = GetTableInfo(requestorTable, conn);
                if (!string.IsNullOrEmpty(tblInfo["OperatorsIdField"].ToString()))
                {
                    using (var cmd = new SqlCommand("SELECT [" + tblInfo["TrackingEmailFieldName"].ToString() + "] FROM [" + requestorTable + "] WHERE [" + tblInfo["OperatorsIdField"].ToString() + "] = @userName", conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", user.UserName);
                        try
                        {
                            string email = cmd.ExecuteScalar().ToString();
                            var mailAddress = new MailAddress(email);
                            return email; // If no error, then valid email
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
                return GetSMTPEMail(conn);
            }
        }

        public static string GetEmployeeEmailByID(string EmployeeID, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                string requestorTable = Tracking.GetRequestorTableName(conn);
                var tblInfo = GetTableInfo(requestorTable, conn);

                using (var cmd = new SqlCommand("SELECT * FROM [" + requestorTable + "] WHERE " + tblInfo["IdFieldName"].ToString() + " = @EmployeeID", conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtInfo = new DataTable();
                        da.Fill(dtInfo);
                        // " & tblInfo("TrackingEmailFieldName").ToString & "
                        if (dtInfo.Rows.Count > 0)
                        {
                            try
                            {
                                var mailAddress = new MailAddress(dtInfo.Rows[0][tblInfo["TrackingEmailFieldName"].ToString()].ToString());
                                return mailAddress.Address; // If no error, then valid email
                            }
                            catch (Exception ex) // No valid Employee email, check for linked user
                            {
                                Debug.WriteLine(ex.Message);

                                try
                                {
                                    using (var cmd2 = new SqlCommand("SELECT [EMail] FROM [SecureUser] WHERE [UserName] = @userName", conn))
                                    {
                                        cmd2.Parameters.AddWithValue("@userName", dtInfo.Rows[0][tblInfo["OperatorsIdField"].ToString()].ToString());
                                        var mailAddress2 = new MailAddress(cmd2.ExecuteScalar().ToString());
                                        return mailAddress2.Address;
                                    }
                                }
                                catch (Exception ex2)
                                {
                                    Debug.WriteLine(ex2.Message);
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty; // Couldn't find recipient
        }

        public static string GetSMTPEMail(SqlConnection conn)
        {
            if (Conversions.ToBoolean(GetSystemSetting("SMTPAuthentication", conn)))
            {
                string email = GetSystemSetting("SMTPUserAddress", conn);
                try
                {
                    var mailAddress = new MailAddress(email);
                    return email; // If no error, then valid email
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public static DataTable GetChildTotal(string TableName, string TableFieldName, Passport passport)
        {
            string sql;

            using (var conn = passport.Connection())
            {
                var dt = GetLowerRelationships(TableName, TableFieldName, conn);
                sql = "SELECT SUM(" + dt.Rows[0]["LowerTableFieldName"].ToString() + ") AS Total FROM " + dt.Rows[0]["LowerTableName"].ToString();

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dtLookup = new DataTable();
                        da.Fill(dtLookup);
                        return dtLookup;
                    }
                }
            }
        }

        public static List<ChildView> GetChildViews(int parentViewID, Passport passport)
        {
            using (var childAdapter = new RecordsManageTableAdapters.ChildViewsTableAdapter())
            {
                using (var conn = passport.Connection())
                {
                    childAdapter.Connection = conn;
                    var list = new List<ChildView>();
                    var ds = new DataSet();
                    var dt = new DataTable();

                    ds.Tables.Add(dt);
                    ds.EnforceConstraints = false;

                    dt = childAdapter.GetChildViewsByParentViewID(parentViewID);

                    foreach (DataRow row in dt.Rows)
                    {
                        var lstOfviews = GetViewsByTableName(row["ChildTableName"].ToString(), passport);
                        if (passport.CheckPermission(row["ChildTableName"].ToString(), SecureObject.SecureObjectType.Table, Permissions.Permission.View) && lstOfviews.Count > 0)
                        {
                            var child = new ChildView();
                            child.ChildViewID = Conversions.ToInteger(row["childViewID"]);
                            child.ChildViewName = Conversions.ToString(row["ChildViewName"]);
                            child.ChildKeyField = MakeSimpleField(row["ChildKeyField"].ToString());
                            // Right(row("ChildKeyField").ToString, Len(row("ChildKeyField")) - InStr(row("ChildKeyField").ToString, "."))
                            child.ChildTableName = row["ChildTableName"].ToString();
                            child.ChildUserName = row["ChildUserName"].ToString();
                            child.ChildKeyType = GetFieldType(child.ChildTableName, child.ChildKeyField, conn).ToString();
                            list.Add(child);
                        }
                    }

                    return list;
                }
            }
        }

        public class ChildView
        {
            public int ChildViewID;
            public string ChildViewName;
            public string ChildKeyField;
            public string ChildKeyType;
            public string ChildTableName;
            public string ChildUserName;
        }

        public static DataTable GetSendToScripts(Passport passport)
        {
            using (var scriptHeader = new RecordsManageTableAdapters.LinkScriptHeaderTableAdapter())
            {
                scriptHeader.Connection = passport.Connection();
                return scriptHeader.GetSendToScripts();
            }
        }

        public static DataRow GetSingleFieldValue(string tableName, string tableId, string fieldName, SqlConnection conn)
        {
            string sql;

            try
            {
                sql = "SELECT [" + fieldName + "] FROM [" + tableName + "] WHERE [" + GetPrimaryKeyFieldName(tableName, conn) + "] = @tableId";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                sql = "SELECT [" + fieldName + "] FROM [" + tableName + "] WHERE [Id] = @tableId";
            }

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableId", tableId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    return null;
                }
            }
        }

        public static DataRow GetSingleFieldValue(string tableName, string tableId, string fieldName, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                return GetSingleFieldValue(tableName, tableId, fieldName, conn);
            }
        }

        public static DataRow GetSingleRow(DataRow tableInfo, string tableId, string fieldName, Passport passport)
        {
            string sql;

            try
            {
                sql = "SELECT * FROM [" + tableInfo["TableName"].ToString() + "] WHERE [" + GetPrimaryKeyFieldName(tableInfo) + "] = @tableId";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                sql = "SELECT * FROM [" + tableInfo["TableName"].ToString() + "] WHERE [Id] = @tableId";
            }

            using (var conn = passport.Connection())
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tableId", tableId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        return null;
                    }
                }
            }
        }

        public static DataRow GetSingleRow(string tableName, string tableId, string fieldName, Passport passport)
        {
            string sql;

            using (var conn = passport.Connection())
            {
                try
                {
                    sql = "SELECT * FROM [" + tableName + "] WHERE [" + GetPrimaryKeyFieldName(tableName, passport) + "] = @tableId";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    sql = "SELECT * FROM [" + tableName + "] WHERE [Id] = @tableId";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tableId", tableId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                        return null;
                    }
                }
            }
        }

        public static void UpdateSingleField(string tableName, string tableId, string updateFieldName, string updateValue, SqlConnection conn)
        {
            string sql = "UPDATE [" + tableName + "] SET [" + MakeSimpleField(updateFieldName) + "] = @updateValue WHERE [" + GetPrimaryKeyFieldName(tableName, conn) + "] = @tableId";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@updateValue", updateValue);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateSingleField(string tableName, string tableId, string updateFieldName, string updateValue, Passport passport)
        {
            using (var conn = passport.Connection())
            {
                UpdateSingleField(tableName, tableId, updateFieldName, updateValue, conn);
            }
        }

        public static string StripLeadingZeros(string stripThis)
        {
            return StripLeadingZeros(stripThis, false);
        }

        public static string StripLeadingZeros(string stripThis, bool fieldIsString)
        {
            if (string.IsNullOrEmpty(stripThis))
                return string.Empty;
            if (!Information.IsNumeric(stripThis))
                return stripThis;
            if (fieldIsString)
                return stripThis;

            while (stripThis.Trim().Length > 0)
            {
                if (string.Compare(stripThis.Substring(0, 1), "0") != 0)
                    return stripThis.Trim();
                stripThis = stripThis.Substring(1);
            }

            return stripThis.Trim();
        }

        public static bool CBoolean(object value)
        {
            if (value is null || value is DBNull || string.IsNullOrEmpty(value.ToString()))
                return false;

            try
            {
                return Conversions.ToBoolean(value);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool CBoolean(DataRow row, string fieldName)
        {
            try
            {
                return CBoolean(row[fieldName]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static string EncryptString(string inputStr)
        {
            var b = Encoding.ASCII.GetBytes(inputStr);
            string encryptedStr = Convert.ToBase64String(b);
            return System.Net.WebUtility.UrlEncode(encryptedStr);
            // Return HttpUtility.UrlEncode(encryptedStr)
        }

        public static string DecryptString(string encString)
        {
            // encString = HttpUtility.UrlDecode(encString)
            encString = System.Net.WebUtility.UrlDecode(encString);
            var b = Convert.FromBase64String(encString);
            string decryptedString = Encoding.ASCII.GetString(b);
            return decryptedString;
        }

        // 'Function added because .webvb function was not accessible inside Query.vb file
        internal static string EncryptURLParameters(string clearText)
        {
            try
            {
                var clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (var encryptor = new AesCryptoServiceProvider())
                {
                    using (var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.FlushFinalBlock();
                            }

                            clearText = Strings.Chr(225) + Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // do nothing 'HeyReggie
            }
            return clearText;
        }

        // 'Function added because .webvb function was not accessible inside Query.vb file
        // hey moti changed to friend - these both methods encrypt decrypt are duplicated in Common.vb because web doesn't have access to it. 
        internal static string DecryptURLParameters(string cipherText)
        {
            try
            {
                if (!cipherText.StartsWith(Conversions.ToString(Strings.Chr(225))))
                    return cipherText;
                cipherText = DecryptURLParameters(cipherText.Substring(1)).Replace(" ", "+");
                var cipherBytes = Convert.FromBase64String(cipherText);

                using (var encryptor = new AesCryptoServiceProvider())
                {
                    using (var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6E, 0x20, 0x4D, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }))
                    {
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.FlushFinalBlock();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Return "whatever"
            }
            return cipherText;
        }

        public static DateTime ApplyYearEndToDate(DateTime ApplyToDate, double YearsToAdd, bool ForceToEndOfYear, SqlConnection conn)
        {
            var workingDate = ApplyToDate.AddMonths((int)Math.Round(YearsToAdd * 12d));
            if (!ForceToEndOfYear)
                return workingDate;
            return ApplyYearEndToDate(workingDate, ForceToEndOfYear, Conversions.ToInteger(GetSystemSetting("RetentionYearEnd", conn)));
        }

        public static DateTime ApplyYearEndToDate(DateTime ApplyToDate, double YearsToAdd, bool ForceToEndOfYear, Passport passport)
        {
            var workingDate = ApplyToDate.AddMonths((int)Math.Round(YearsToAdd * 12d));
            if (!ForceToEndOfYear)
                return workingDate;
            return ApplyYearEndToDate(workingDate, ForceToEndOfYear, Conversions.ToInteger(GetSystemSetting("RetentionYearEnd", passport)));
        }

        public static DateTime ApplyYearEndToDate(DateTime ApplyToDate, bool ForceToEndOfYear, int YearEndMonth)
        {
            if (!ForceToEndOfYear)
                return ApplyToDate;

            int year = ApplyToDate.Year;
            if (ApplyToDate.Month > YearEndMonth)
                year += 1;
            return new DateTime(year, YearEndMonth, DateTime.DaysInMonth(year, YearEndMonth));
        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj is null | ReferenceEquals(obj, DBNull.Value))
            {
                return default;
            }
            else
            {
                return Conversions.ToGenericParameter<T>(obj);
            }
        }

        public static Dictionary<string, string> StringToDictionary(string str)
        {
            // Assume , and ; as delimiters.  Consider adding optional delimiter parameters.
            char itemDelim = ',';
            char lineDelim = ';';
            var ret = new Dictionary<string, string>();
            try
            {
                var lines = str.Split(lineDelim);
                foreach (string line in lines)
                {
                    var items = line.Split(itemDelim);
                    ret.Add(items[0], items[1]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return ret;

        }

        public static string DictionaryToString(Dictionary<string, string> dict)
        {
            // Assume , and ; as delimiters.  Consider adding optional delimiter parameters.
            string itemDelim = ",";
            string lineDelim = ";";
            string ret = string.Empty;

            foreach (var line in dict)
            {
                if (!string.IsNullOrEmpty(ret))
                {
                    ret += lineDelim;
                }
                ret = ret + line.Key + itemDelim + line.Value;
            }
            return ret;

        }

        // Private Shared Function ApplyExtraMonths(ByVal applyToDate As Date, ByVal yearsWithDecimal As String) As Date
        // If yearsWithDecimal.Contains(".25") Then Return applyToDate.AddMonths(3)
        // If yearsWithDecimal.Contains(".5") Then Return applyToDate.AddMonths(6)
        // If yearsWithDecimal.Contains(".75") Then Return applyToDate.AddMonths(9)
        // Return applyToDate
        // End Function

        public static DataTable ExecuteTableAndViewQuery(Passport passport, string query)
        {
            string sql = query;
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
    }

    public class DateFormat
    {
        public partial class UserPreferences
        {
            private string p_sPreferedLanguage;
            private string p_sPreferedDateFormat;

            public string sPreferedLanguage
            {
                get
                {
                    return p_sPreferedLanguage;
                }
                set
                {
                    p_sPreferedLanguage = value;
                }
            }

            public string sPreferedDateFormat
            {
                get
                {
                    return p_sPreferedDateFormat;
                }
                set
                {
                    p_sPreferedDateFormat = value;
                }
            }
        }
        private static UserPreferences GetUserPreferences
        {
            get
            {
                var pUserPreferences = new UserPreferences();
                // ' Get browser language
                string[] strLanguages;
                string strLanguage = "en-US";
                try
                {
                    var httpcontext = new HttpContextAccessor().HttpContext;
                    strLanguages = httpcontext.Request.Headers["Accept-Language"].ToString().Split(",");

                    var ci = new CultureInfo(strLanguage);

                    // ' Set browser preference
                    pUserPreferences.sPreferedLanguage = strLanguage;
                    pUserPreferences.sPreferedDateFormat = ci.DateTimeFormat.ShortDatePattern;
                    // ' Read preference cookies if exists

                    string strCurrentUserName = "";
                    if (httpcontext is not null)
                    {
                        var UserName = httpcontext.Session.GetString("UserName");
                        if (!string.IsNullOrEmpty(UserName))
                        {

                            strCurrentUserName = UserName;

                            var strCurrentUserNameCookie = httpcontext.Request.Cookies[strCurrentUserName];
                            var UserInfoCookieCollection = strCurrentUserNameCookie.Split("&").Select(s => s.Split("=")).ToDictionary(kvp => kvp[0], kvp => kvp[1]);

                            string pPreferedLanguage = System.Net.WebUtility.HtmlEncode(UserInfoCookieCollection["PreferedLanguage"]);
                            string pPreferedDateFormat = System.Net.WebUtility.HtmlEncode(UserInfoCookieCollection["PreferedDateFormat"]);

                            if (!string.IsNullOrEmpty(pPreferedLanguage))
                            {
                                pUserPreferences.sPreferedLanguage = pPreferedLanguage;
                            }
                            if (!string.IsNullOrEmpty(pPreferedDateFormat))
                            {
                                pUserPreferences.sPreferedDateFormat = pPreferedDateFormat;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return pUserPreferences;
            }
        }

        private static CultureInfo GetCultureCookies
        {
            get
            {
                var httpcontext = new HttpContextAccessor().HttpContext;

                var pUserPreferences = GetUserPreferences;

                var ci = new CultureInfo(pUserPreferences.sPreferedLanguage);
                ci.DateTimeFormat.ShortDatePattern = pUserPreferences.sPreferedDateFormat;
                //var d = httpcontext.Session.GetString("UserName");
                return ci;
            }
        }

        public static string get_ConvertCultureDate(string strDate)
        {
            string dtCutlureFormat = "";
            if (!string.IsNullOrEmpty(strDate))
            {
                try
                {
                    dtCutlureFormat = DateTime.Parse(strDate).ToString(GetCultureCookies.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    dtCutlureFormat = DateTime.ParseExact(strDate, GetCultureCookies.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture).ToShortDateString();
                }
            }
            return dtCutlureFormat;
        }
    }
}