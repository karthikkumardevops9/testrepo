using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
//using System.Management;
using System.Reflection;
using System.Security;
//using System.ServiceModel.Configuration;
//using Leadtools;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using MSRecordsEngine.Properties;
using Smead.Security;

namespace MSRecordsEngine.Imaging
{

    public class Attachments
    {
        public const string OrphanName = "Orphans";
        private const string ImagePointerName = "ImagePointers";
        private const int UserLinkIndexTableIdSize = 30;
        private const int Leadtools14 = 14;

        private static int _leadToolsMajorVersion = -1;
        private static int _userLinkIndexTableIdSize = 0;
        //private static slimShared.CSlimAPI _slim = new slimShared.CSlimAPI("Smeadlink", true);
        private static Size _thumbSize = new Size(41, 44);

        public enum AttachmentTypes
        {
            tkNone,
            tkImage,
            tkFax,
            tkCOLD,
            tkWPDoc = 5 // see remark above
        }

        internal enum AnnotationsDrawMode
        {
            None,
            RedactionOnly,
            All
        }

        private static bool AttachmentPermission(Attachment attachment, Permissions.Attachment permissionName)
        {
            if (attachment is null)
                return true;
            return attachment.SecurityInfo.AttachmentPermissions.Contains(permissionName.ToString().ToLower());
        }

        private static bool OrphanPermission(Attachment attachment, Permissions.Orphan permissionName)
        {
            if (attachment is null)
                return true;
            return Attachments.OrphanPermission(attachment.SecurityInfo, permissionName);
        }

        private static bool OrphanPermission(SecurityInfo security, Permissions.Orphan permissionName)
        {
            if (security is null || security.OrphanPermissions is null)
                return false;
            return security.OrphanPermissions.Contains(permissionName.ToString().ToLower());
        }

        private static bool VolumePermission(Attachment attachment, Permissions.Volume permissionName)
        {
            if (attachment is null)
                return true;
            return Attachments.VolumePermission(attachment.SecurityInfo, permissionName);
        }

        private static bool VolumePermission(SecurityInfo security, Permissions.Volume permissionName)
        {
            if (security is null || security.VolumePermissions is null)
                return false;

            switch (permissionName)
            {
                case Permissions.Volume.Access:
                case Permissions.Volume.View:
                    {
                        return security.VolumePermissions.Contains(Permissions.Volume.Access.ToString().ToLower()) | security.VolumePermissions.Contains(Permissions.Volume.View.ToString().ToLower());
                    }

                default:
                    {
                        return security.VolumePermissions.Contains(permissionName.ToString().ToLower());
                    }
            }
        }

        public static Image CreateErrorImage(Image baseImage, string message)
        {
            var bmp = new Bitmap(baseImage);
            var gfx = Graphics.FromImage(bmp);

            var arialFont = new Font("Arial", 10f);
            SizeF fSize;
            var fmt = new StringFormat();
            fmt.Alignment = StringAlignment.Center;

            fSize = gfx.MeasureString(message, arialFont, new SizeF(bmp.Width - 6, bmp.Height - 6), fmt);
            gfx.DrawString(message, arialFont, Brushes.Black, new RectangleF((bmp.Width - fSize.Width) / 2f, 3f, fSize.Width, fSize.Height), fmt);
            gfx.Dispose();
            fmt.Dispose();

            return bmp;
        }

        private static string GetAppSetting(string value)
        {
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                if (string.Compare(key, "DebugFlyout", true) == 0)
                    return ConfigurationManager.AppSettings[key];
            }

            return string.Empty;
        }

        public static void DrawTextOnErrorImage(Bitmap bmp, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            using (var gfx = Graphics.FromImage(bmp))
            {
                using (var arial = new Font("Arial", 10f))
                {
                    using (var fmt = StringFormatting(StringAlignment.Center, false))
                    {
                        var fSize = gfx.MeasureString(message, arial, new SizeF(bmp.Width - 6, bmp.Height - 6), fmt);
                        gfx.DrawString(message, arial, Brushes.Black, new RectangleF((bmp.Width - fSize.Width) / 2f, 4f, fSize.Width, fSize.Height), fmt);
                    }
                }
            }
        }

        private static StringFormat StringFormatting(StringAlignment alignment, bool useEllipsis)
        {
            var sf = StringFormat.GenericTypographic;
            sf.Alignment = alignment;
            sf.LineAlignment = StringAlignment.Center;
            sf.Trimming = StringTrimming.None;
            sf.FormatFlags = StringFormatFlags.FitBlackBox;

            if (useEllipsis)
                sf.Trimming = StringTrimming.EllipsisPath;
            return sf;
        }

        //public static Attachment GetAttachment(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, bool populateThumbnails, string operatorStr, bool childAttachmentsOn, bool rebuildCache, int startPage)
        //{
        //    return GetAttachment(ClientIpAddress, Ticket, userId, databaseName, tableName, tableId, 0, 0, _thumbSize, populateThumbnails, operatorStr, childAttachmentsOn, rebuildCache, startPage, true);
        //}

        //public static Attachment GetAttachment(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, int attachmentNumber, bool populateThumbnails, string operatorStr, bool childAttachmentsOn, bool rebuildCache, int startPage)
        //{
        //    return GetAttachment(ClientIpAddress, userId, databaseName, tableName, tableId, attachmentNumber, 0, _thumbSize, populateThumbnails, operatorStr, childAttachmentsOn, false, rebuildCache, startPage, false);
        //}

        public static Attachment GetAttachment(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, Size thumbsize, bool populateThumbnails, string operatorStr, bool childAttachmentsOn, bool rebuildCache, int startPage, bool forImageFlyout)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);
            return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, thumbsize, populateThumbnails, operatorStr, childAttachmentsOn, false, rebuildCache, startPage, forImageFlyout);
        }

        private static Attachment GetAttachment(string ClientIpAddress, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, Size thumbsize, bool populateThumbnails, bool childAttachmentsOn, bool suppressAuditRecord, bool rebuildCache, int startPage)
        {
            return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, thumbsize, populateThumbnails, "=", childAttachmentsOn, suppressAuditRecord, rebuildCache, startPage, false);
        }
        private static Attachment GetAttachment(string ClientIpAddress, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, Size thumbsize, bool populateThumbnails, string operatorStr, bool childAttachmentsOn, bool suppressAuditRecord, bool rebuildCache, int startPage, bool forImageFlyout)
        {
            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, thumbsize, populateThumbnails, operatorStr, childAttachmentsOn, suppressAuditRecord, rebuildCache, startPage, conn, forImageFlyout);
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
        }

        private static string GetAttachmentPath(int userId, string databaseName, string tableName, string tableId, SqlConnection conn, ref bool validAttachment, ref string fullPath)
        {
            string paddedTableId = PadTableId(tableName, tableId, conn);

            fullPath = string.Empty;
            validAttachment = false;
            string sql = string.Format(GetSql(userId, tableName, paddedTableId, 0, 0, conn), ">=", "ASC");
            var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, 0, 0, sql, conn);
            if (dt.Rows.Count == 0)
                return Permissions.ExceptionString.NoRecords;

            int attachmentNumber = SafeInt(dt.Rows[0], "AttachmentNumber", tableName, paddedTableId, conn);
            var rtn = GetValidAttachment(userId, databaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
            if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                return Permissions.ExceptionString.NoRecords;
            if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                return Permissions.ExceptionString.NoRecords;

            string cachedPath = Path.Combine(Path.GetDirectoryName(dt.Rows[0]["FullPath"].ToString()), Attachment.CachedFlyouts, string.Format("{0}.{1}", Path.GetFileNameWithoutExtension(dt.Rows[0]["FullPath"].ToString()), Output.Format.Jpg.ToString().ToLower()));
            validAttachment = File.Exists(cachedPath);
            if (!validAttachment)
            {
                fullPath = dt.Rows[0]["FullPath"].ToString();
            }

            return cachedPath;
        }

        private static Attachment GetAttachment(string ClientIpAddress, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, Size thumbsize, bool populateThumbnails, string operatorStr, bool childAttachmentsOn, bool suppressAuditRecord, bool rebuildCache, int startPage, SqlConnection conn, bool forImageFlyout)
        {
            _thumbSize = thumbsize;
            string paddedTableId = PadTableId(tableName, tableId, conn);
            string SQLdirection = "ASC";
            if (string.Compare(operatorStr, "<") == 0 || string.Compare(operatorStr, "<=") == 0)
                SQLdirection = "DESC";

            string sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, versionNumber, conn), operatorStr, SQLdirection);
            var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, versionNumber, sql, conn);
            if (dt.Rows.Count == 0)
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

            string fieldName = "AttachmentNumber";
            attachmentNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);

            if (versionNumber != 0)
            {
                fieldName = "RecordVersion";
                versionNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);
            }

            var rtn = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
            if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
            if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);

            if (!forImageFlyout)
            {
                if (versionNumber != 0)
                {
                    for (int index = 0, loopTo = dt.Rows.Count - 1; index <= loopTo; index++)
                    {
                        int currentNumber = SafeInt(dt.Rows[index], fieldName, tableName, tableId, conn);
                        if (currentNumber > versionNumber)
                            break;
                        if (currentNumber == versionNumber)
                            rtn.AddParts(attachmentNumber, dt.Rows[index], true, conn);
                    }
                }
                else
                {
                    rtn.AddParts(dt.Rows, fieldName, attachmentNumber, tableName, true, conn);
                }
            }

            DeleteCache(rtn, rebuildCache);
            if (populateThumbnails && !forImageFlyout)
                rtn.AddThumbnails(0, startPage, userId, attachmentNumber, dt.Rows, true, false, attachmentNumber, conn);
            if (!suppressAuditRecord)
                AuditConfidentialDataAccess(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, string.Format("Displayed: Attachment id {0}; version {1}", (object)rtn.AttachmentNumber, (object)rtn.VersionInfo.Version), conn);
            return rtn;
        }

        private static void DeleteCache(DataRowCollection rows, bool rebuildCache)
        {
            if (!rebuildCache || rows is null)
                return;

            foreach (DataRow row in rows)
            {
                try
                {
                    if (!(row["FullPath"] is DBNull))
                        Attachment.DeleteCache(row["FullPath"].ToString().Trim(), false);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        private static void DeleteCache(Attachment attachment, bool rebuildCache)
        {
            if (!rebuildCache || attachment is null || attachment is ErrorAttachment)
                return;

            foreach (AttachmentPart part in attachment.AttachmentParts)
                Attachment.DeleteCache(part.FullPath, false);
        }

        internal static string PadTableId(string tableName, string tableId, SqlConnection conn)
        {
            if (IdFieldIsString(tableName, conn))
                return tableId;
            tableId = StripLeadingZeros(tableId);
            return tableId.PadLeft(SetUserlinkIdSize(conn), '0');
        }

        public static string StripLeadingZeros(string stripThis)
        {
            if (string.IsNullOrEmpty(stripThis))
                return string.Empty;
            if (!Information.IsNumeric(stripThis))
                return stripThis;

            while (stripThis.Trim().Length > 0)
            {
                if (string.Compare(stripThis.Substring(0, 1), "0") != 0)
                    return stripThis.Trim();
                stripThis = stripThis.Substring(1);
            }

            return stripThis.Trim();
        }

        public static Attachment GetAllAttachments(string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, Size thumbsize, bool childAttachmentsOn, bool rebuildCache)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            return GetAllAttachments(userId, pass, tableName, tableId, attachmentNumber, thumbsize, null, childAttachmentsOn, rebuildCache);
        }

        private static Attachment GetAllAttachments(int userId, Passport pass, string tableName, string tableId, int attachmentNumber, Size thumbsize, Attachment returnAttachment, bool childAttachmentsOn, bool rebuildCache)
        {
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    _thumbSize = thumbsize;
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    string sql = string.Format(GetSqlAll(userId, tableName, paddedTableId, attachmentNumber, 0, conn), "=", "ASC");
                    var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);

                    if (dt.Rows.Count == 0 && returnAttachment is null)
                    {
                        returnAttachment = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, AttachmentTypes.tkImage, attachmentNumber, null, conn);
                    }
                    else
                    {
                        if (returnAttachment is null)
                            returnAttachment = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
                        DeleteCache(dt.Rows, rebuildCache);

                        string fieldName = "AttachmentNumber";
                        if (attachmentNumber > 0)
                            fieldName = "RecordVersion";
                        int lastNumber = -1;
                        int currentNumber = 0;

                        for (int index = 0, loopTo = dt.Rows.Count - 1; index <= loopTo; index++)
                        {
                            currentNumber = SafeInt(dt.Rows[index], fieldName, tableName, paddedTableId, conn);

                            if (lastNumber != currentNumber)
                            {
                                var tempAttachment = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[index]["RecordType"], attachmentNumber, dt.Rows[index], conn);

                                if (Attachments.AttachmentPermission(tempAttachment, Permissions.Attachment.View) && Attachments.VolumePermission(tempAttachment, Permissions.Volume.View))
                                {
                                    lastNumber = currentNumber;
                                    returnAttachment.AddParts(dt.Rows, fieldName, lastNumber, tableName, false, conn);
                                    returnAttachment.AddThumbnails(index, -1, userId, SafeInt(dt.Rows[index], "AttachmentNumber", tableName, paddedTableId, conn), dt.Rows, 1, false, string.Compare(fieldName, "RecordVersion", true) == 0, index + 1, conn);

                                    if (!Attachments.AttachmentPermission(returnAttachment, Permissions.Attachment.View) || !Attachments.VolumePermission(returnAttachment, Permissions.Volume.View))
                                    {
                                        returnAttachment.AttachmentNumber = tempAttachment.AttachmentNumber;
                                        returnAttachment.VersionInfo.Version = tempAttachment.VersionInfo.Version;
                                    }
                                }
                            }
                        }
                    }

                    childAttachmentsOn = GetPermissionChildAttachments(childAttachmentsOn, tableName, userId, conn);
                    if (childAttachmentsOn)
                        GetRelatedChildTables(userId, pass, tableName, tableId, ref returnAttachment, thumbsize, childAttachmentsOn, rebuildCache, conn);
                    if (returnAttachment.ThumbsList.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, returnAttachment.SecurityInfo, conn);

                    return returnAttachment;
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
        }

        internal static string GetDisplayFields(DataRow row, SqlConnection conn)
        {
            string tableId = row["IndexTableId"].ToString();
            if (!IdFieldIsString(row["IndexTable"].ToString(), conn))
                tableId = StripLeadingZeros(tableId);
            string fields = AppendDisplayFields(row);
            if (string.IsNullOrEmpty(fields))
                return string.Format("{0} ({1})", row["IndexTable"].ToString(), tableId);
            string sql = string.Format("SELECT {0} FROM [{1}] WHERE [{2}] = @tableId", fields, row["IndexTable"].ToString(), row["IdFieldName"].ToString().Replace(".", "].["));

            try
            {
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tableId", tableId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return string.Format("{0} ({1})", row["IndexTable"].ToString(), tableId);
                    return ReturnDisplayFields(row, dt.Rows[0], tableId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return string.Format("{0} ({1})", row["IndexTable"].ToString(), tableId);
        }

        private static string AppendDisplayFields(DataRow row)
        {
            var sb = new System.Text.StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(row["DescFieldNameOne"].ToString()))
                    sb.Append(string.Format("{0},", row["DescFieldNameOne"].ToString()));
            }
            catch (Exception ex)
            {
                //Logs.Loginformation(string.Format("Error \"{0}\" in Attachments.AppendDisplayFields(DescFieldNameOne)", ex.Message));
            }

            try
            {
                if (!string.IsNullOrEmpty(row["DescFieldNameTwo"].ToString()))
                    sb.Append(string.Format("{0},", row["DescFieldNameTwo"].ToString()));
            }
            catch (Exception ex)
            {
                // Logs.Loginformation(string.Format("Error \"{0}\" in Attachments.AppendDisplayFields(DescFieldNameTwo)", ex.Message));
            }

            if (sb.Length < 3)
                return string.Empty;
            return sb.ToString().Substring(0, sb.Length - 1);
        }

        private static string ReturnDisplayFields(DataRow row, DataRow data, string tableId)
        {
            var sb = new System.Text.StringBuilder();

            if (!string.IsNullOrEmpty(row["DescFieldNameOne"].ToString()) && !string.IsNullOrEmpty(data[row["DescFieldNameOne"].ToString()].ToString()))
            {
                if (!string.IsNullOrEmpty(row["DescFieldPrefixOne"].ToString()))
                    sb.Append(string.Format("{0} ", row["DescFieldPrefixOne"].ToString()));
                sb.Append(string.Format("{0} ", data[row["DescFieldNameOne"].ToString()].ToString()));
            }
            if (!string.IsNullOrEmpty(row["DescFieldNameTwo"].ToString()) && !string.IsNullOrEmpty(data[row["DescFieldNameTwo"].ToString()].ToString()))
            {
                if (!string.IsNullOrEmpty(row["DescFieldPrefixTwo"].ToString()))
                    sb.Append(string.Format("{0} ", row["DescFieldPrefixTwo"].ToString()));
                sb.Append(string.Format("{0} ", data[row["DescFieldNameTwo"].ToString()].ToString()));
            }

            if (sb.Length < 3)
                return string.Format("{0} ({1})", row["IndexTable"].ToString(), tableId);
            return sb.ToString().Substring(0, sb.Length - 1);
        }

        private static bool GetPermissionChildAttachments(bool childAttachmentsOn, string tableName, int userId, SqlConnection conn)
        {
            if (!childAttachmentsOn)
                return false;

            try
            {
                using (var cmd = new SqlCommand(Resources.GetPermissionChildAttachments, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var value = cmd.ExecuteScalar();
                    if (value is null || !Information.IsNumeric(value))
                        return false;
                    return Convert.ToInt32(value) > 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private static void GetRelatedChildTables(int userId, Passport pass, string tableName, string upperTableID, ref Attachment returnAttachment, Size thumbsize, bool childAttachmentsOn, bool rebuildCache, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetTableRelationships, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                            GetRelatedChildRecords(userId, pass, row["LowerTableName"].ToString(), row["IDFieldName"].ToString(), row["LowerTableFieldName"].ToString(), upperTableID, ref returnAttachment, thumbsize, childAttachmentsOn, rebuildCache, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void GetRelatedChildRecords(int userId, Passport pass, string lowerTableName, string idField, string lowerTableFieldName, string upperTableFieldValue, ref Attachment returnAttachment, Size thumbsize, bool childAttachmentsOn, bool rebuildCache, SqlConnection conn)
        {
            string SQL = GetDefaultViewSQLStatement(userId, lowerTableName, conn);
            if (string.IsNullOrEmpty(SQL))
                return;
            SQL = InjectWhereIntoSQL(SQL, string.Format("[{0}] = '{1}'", lowerTableFieldName.Replace(".", "].["), upperTableFieldValue));

            bool includeInactiveRecords = false;
            bool includeArchivedRecords = false;
            bool includeDestroyedRecords = false;

            GetRetentionPermissions(userId, ref includeInactiveRecords, ref includeArchivedRecords, ref includeDestroyedRecords, conn);

            if (IsInactivityOnForTable(lowerTableName, conn) && !includeInactiveRecords)
                SQL = InjectWhereIntoSQL(SQL, "([%slRetentionInactiveFinal] = 0 OR [%slRetentionInactiveFinal] IS Null)");
            if (IsRetentionOnForTable(lowerTableName, conn) && !includeArchivedRecords)
                SQL = InjectWhereIntoSQL(SQL, "([%slRetentionDispositionStatus] <> 1 OR [%slRetentionDispositionStatus] IS Null)");
            if (IsRetentionOnForTable(lowerTableName, conn) && !includeDestroyedRecords)
                SQL = InjectWhereIntoSQL(SQL, "([%slRetentionDispositionStatus] <> 2 OR [%slRetentionDispositionStatus] IS Null)");

            try
            {
                using (var cmd = new SqlCommand(SQL, conn))
                {
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                            GetAllAttachments(userId, pass, lowerTableName, row[0].ToString(), 0, thumbsize, returnAttachment, childAttachmentsOn, rebuildCache);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void GetRetentionPermissions(int userId, ref bool inactive, ref bool archived, ref bool destroyed, SqlConnection conn)
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

        private static bool IsRetentionOnForTable(string tableName, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsRetentionOnForTable, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                            return false;
                        return Convert.ToBoolean(dt.Rows[0][0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private static bool IsInactivityOnForTable(string tableName, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsRetentionOnForTable, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                            return false;
                        return Convert.ToBoolean(dt.Rows[0][0]);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private static string GetDefaultViewSQLStatement(int userId, string tableName, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetAllViewsForTable, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        string SQL = string.Empty;
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                            return string.Empty;
                        foreach (DataRow row in dt.Rows)
                        {
                            SQL = row["SQLStatement"].ToString();
                            if (UserHasViewPermission(userId, row["ViewName"].ToString(), conn))
                                return SQL;
                        }
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private static bool UserHasViewPermission(int userId, string viewName, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetViewPermissionsForUser, conn))
                {
                    cmd.Parameters.AddWithValue("@viewName", viewName);
                    cmd.Parameters.AddWithValue("@userID", userId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                            return false;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (string.Compare(row[0].ToString(), "view", true) == 0)
                                return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static string InjectWhereIntoSQL(string sSQL, string sNewWhere, string sOperator = "AND")
        {
            string sInitGroupBy = string.Empty;
            string sInitWhere = string.Empty;
            string sInitOrderBy = string.Empty;
            string sInitSelect = string.Empty;
            int iPos;
            string sRetVal = string.Empty;

            sRetVal = sSQL;

            iPos = Strings.InStr(Strings.UCase(sSQL), " WHERE ");
            if (iPos > 0)
            {
                sInitSelect = Strings.Trim(Strings.Left(sSQL, iPos));
                sInitWhere = Strings.Trim(Strings.Mid(sSQL, iPos + Strings.Len(" WHERE "), Strings.Len(sSQL)));

                iPos = Strings.InStr(Strings.UCase(sInitWhere), " ORDER BY ");
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
                iPos = Strings.InStr(Strings.UCase(sSQL), " ORDER BY ");
                if (iPos > 0)
                {
                    sInitOrderBy = Strings.Trim(Strings.Mid(sSQL, iPos + Strings.Len(" ORDER BY "), Strings.Len(sSQL)));
                    sInitSelect = Strings.Trim(Strings.Left(sSQL, iPos));
                }
                else
                {
                    sInitSelect = Strings.Trim(sSQL);
                }

                iPos = Strings.InStr(sInitSelect, " GROUP BY ", CompareMethod.Text);

                if (iPos > 0)
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

        private static string ParenEncloseWhere(string sSQL)
        {
            return InjectWhereIntoSQL(sSQL, "");
        }



        public static Attachment AddAnAttachment(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, string outputSettingName, string fileName, string originalFullPath, string extension, bool childAttachmentsOn, string renameFileName, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    long currentFileSize = new FileInfo(fileName).Length;
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    var outputSettingTable = GetOutputSetting(outputSettingName, userId, currentFileSize, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, currentFileSize, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingOutputSetting), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    string sql = Resources.GetFirstAttachment;
                    if (attachmentNumber == 0)
                        sql = Resources.GetLastAttachment;
                    var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);

                    if (dt.Rows.Count == 0)
                    {
                        attachmentNumber = 1;
                    }
                    else
                    {
                        attachmentNumber = SafeInt(dt.Rows[0], "AttachmentNumber", tableName, paddedTableId, conn) + 1;
                    }

                    var update = new UpdateAttachment(outputSettingTable.Rows[0], GetUserName(userId, conn), extension, conn);
                    update.CreateAttachmentRecord(tableName, paddedTableId, attachmentNumber, 1, false, 0, fileName, originalFullPath, renameFileName, IsAnImage, totalPages, height, width, sizeOnDisk);

                    if (TableHasOfficialRecordEnabled(tableName, conn))
                        MarkOfficialRecord(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, 1, conn);

                    string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, attachmentNumber, 1);
                    if (!string.IsNullOrEmpty(renameFileName))
                        data = string.Format("{0}{1}New Attachment Name: {2}", Constants.vbCrLf, data, renameFileName);
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Added Attachment", string.Empty, data, AuditType.AttachmentViewerActionType.AddAttachment, conn);

                    sql = string.Format(GetSql(userId, tableName, paddedTableId, 0, 0, conn), "=", "ASC");
                    dt = GetRowsOfAttachments(userId, tableName, paddedTableId, 0, 0, sql, conn);
                    if (dt.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    string fieldName = "AttachmentNumber";
                    attachmentNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);
                    var rtn = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
                    if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
                    if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
            finally
            {
                Exporter.DeleteFile(fileName);
            }
        }

        public static Attachment AddAttachment(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, string outputSettingName, string fileName, string originalFullPath, string extension, bool childAttachmentsOn, string renameFileName)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    return AddAttachment(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId, attachmentNumber, outputSettingName, fileName, originalFullPath, extension, childAttachmentsOn, renameFileName, conn);
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
            finally
            {
                Exporter.DeleteFile(fileName);
            }
        }

        private static Attachment AddAttachment(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, int attachmentNumber, string outputSettingName, string fileName, string originalFullPath, string extension, bool childAttachmentsOn, string renameFileName, SqlConnection conn)
        {
            long currentFileSize = new FileInfo(fileName).Length;
            string paddedTableId = PadTableId(tableName, tableId, conn);
            var outputSettingTable = GetOutputSetting(outputSettingName, userId, currentFileSize, conn);
            if (outputSettingTable.Rows.Count == 0)
                outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, currentFileSize, conn);
            if (outputSettingTable.Rows.Count == 0)
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingOutputSetting), userId, databaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

            string sql = Resources.GetFirstAttachment;
            if (attachmentNumber == 0)
                sql = Resources.GetLastAttachment;
            var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);

            if (dt.Rows.Count == 0)
            {
                attachmentNumber = 1;
            }
            else
            {
                attachmentNumber = SafeInt(dt.Rows[0], "AttachmentNumber", tableName, paddedTableId, conn) + 1;
            }

            var update = new UpdateAttachment(outputSettingTable.Rows[0], GetUserName(userId, conn), extension, conn);
            update.CreateAttachmentRecord(tableName, paddedTableId, attachmentNumber, 1, false, 0, fileName, originalFullPath, renameFileName);

            if (TableHasOfficialRecordEnabled(tableName, conn))
                MarkOfficialRecord(ClientIpAddress, Ticket, userId, databaseName, tableName, paddedTableId, attachmentNumber, 1, conn);

            string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, attachmentNumber, 1);
            if (!string.IsNullOrEmpty(renameFileName))
                data = string.Format("{0}{1}New Attachment Name: {2}", Constants.vbCrLf, data, renameFileName);
            Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Added Attachment", string.Empty, data, AuditType.AttachmentViewerActionType.AddAttachment, conn);

            sql = string.Format(GetSql(userId, tableName, paddedTableId, 0, 0, conn), "=", "ASC");
            dt = GetRowsOfAttachments(userId, tableName, paddedTableId, 0, 0, sql, conn);
            if (dt.Rows.Count == 0)
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, databaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

            string fieldName = "AttachmentNumber";
            attachmentNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);
            var rtn = GetValidAttachment(userId, databaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
            if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, databaseName, tableName, tableId, rtn.SecurityInfo, conn);
            if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, databaseName, tableName, tableId, rtn.SecurityInfo, conn);

            return rtn;
        }

        public static Attachment AddAVersion(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, string outputSettingName, string fileName, string originalFullPath, bool AsOfficialRecord, string extension, bool childAttachmentsOn, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    if (IsCheckedOut(tableName, paddedTableId, attachmentNumber, conn) > 0)
                        return new ErrorAttachment(new Exception(string.Format(Permissions.ExceptionString.CannotAddWhenCheckedOut, "Versions")), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    long fileLength = new FileInfo(fileName).Length;
                    var outputSettingTable = GetOutputSetting(outputSettingName, userId, fileLength, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, fileLength, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingOutputSetting), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    var dt = Attachments.GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, Resources.GetAllVersions, conn);
                    if (dt.Rows.Count == 0)
                        return AddAttachment(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId, attachmentNumber, outputSettingName, fileName, originalFullPath, extension, childAttachmentsOn, string.Empty, null);

                    // Added by akruti to fix the issue FUS-4577
                    string attachmentName = dt.Rows[0]["OrgFileName"].ToString();

                    versionNumber = Convert.ToInt32(dt.Rows[0]["RecordVersion"]) + 1;
                    var update = new UpdateAttachment(outputSettingTable.Rows[0], GetUserName(userId, conn), extension, conn);
                    update.CreateAttachmentRecord(tableName, paddedTableId, attachmentNumber, versionNumber, AsOfficialRecord, Convert.ToInt32(dt.Rows[0]["TrackablesID"]), fileName, originalFullPath, attachmentName, IsAnImage, totalPages, height, width, sizeOnDisk);
                    if (AsOfficialRecord)
                        MarkOfficialRecord(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, versionNumber, conn);

                    string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, attachmentNumber, versionNumber);
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Added Version", string.Empty, data, AuditType.AttachmentViewerActionType.AddVersion, conn);

                    string sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, 0, conn), "=", "ASC");
                    dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);
                    if (dt.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    string fieldName = "AttachmentNumber";
                    attachmentNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);
                    var rtn = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
                    if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
                    if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);

                    return rtn;
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
            finally
            {
                Exporter.DeleteFile(fileName);
            }
        }


        //this method call from the controller and get an exception before get in.
        public static string AddOrphan(string Ticket, int userId, Passport pass, string outputSettingName, string fileName, dynamic info, string extension, bool returnSuccess)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, OrphanName, string.Empty))
                throw new Exception(Permissions.ExceptionString.NotAuthorized);

            string rtn = string.Empty;
            if (returnSuccess)
                rtn = Permissions.ExceptionString.Successful;

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    long fileLength = new FileInfo(info.FilePath).Length;
                    var outputSettingTable = GetOutputSetting(outputSettingName, userId, fileLength, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, fileLength, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        return Permissions.ExceptionString.MissingOutputSetting;

                    var update = new UpdateAttachment(outputSettingTable.Rows[0], GetUserName(userId, conn), extension, conn);
                    //var info = UpdateAttachment.GetCodecInfoFromFile(fileName, extension);
                    int orphanId = 0;

                    if (info.Ispcfile)
                    {
                        orphanId = update.CreateOrphanRecord(fileName, info.FilePath, false, 1, 0, 0, 0L);
                    }
                    else
                    {
                        orphanId = update.CreateOrphanRecord(fileName, info.FilePath, true, info.TotalPages, info.Height, info.Width, info.SizeDisk);
                    }

                    if (orphanId == 0)
                        return Permissions.ExceptionString.FileNotCreated;
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                Exporter.DeleteFile(info.FilePath);
            }
        }



        public static OcrText OcrImageClip(string Ticket, int userId, string databaseName, string tableName, string tableId, byte[] byteArray)
        {
            var rtn = new OcrText();
            if (!Encrypt.ValidateTicket(Ticket, userId, databaseName, tableName, tableId))
                rtn.ErrorMessage = Permissions.ExceptionString.NotAuthorized;
            if (string.IsNullOrEmpty(tableName))
                rtn.ErrorMessage = Permissions.ExceptionString.MissingParameters;
            if (!string.IsNullOrEmpty(rtn.ErrorMessage))
                return rtn;

            Ocr ocr = null;

            try
            {
                ocr = new Ocr();
                rtn.OcredText = ocr.Recognize(byteArray);
            }
            catch (Exception ex)
            {
                rtn.ErrorMessage = ex.Message;
            }
            finally
            {
                ocr = null;
            }

            return rtn;
        }

        private static int GetNewDirectoryId(int directoryId, int userId, int pointerId, string fileName, bool replaceExisting, string outputSettingName, long currentFileSize, SqlConnection conn)
        {
            if (directoryId != 0)
                return directoryId;

            var dt = GetOutputSettingWithoutDirectory(outputSettingName, userId, currentFileSize, conn);
            if (dt.Rows.Count > 0 && (Convert.ToBoolean(dt.Rows[0]["Inactive"]) || !Convert.ToBoolean(dt.Rows[0]["VolumeActive"])))
                return directoryId;
            if (dt.Rows.Count == 0)
                return directoryId;
            directoryId = Convert.ToInt32(dt.Rows[0]["DirectoriesId"]);
            if (!replaceExisting)
                Attachment.UpdatePointerFileName(pointerId, fileName, dt.Rows[0]["ImageTableName"].ToString(), directoryId, 0, true, conn);
            return directoryId;
        }

        private static bool TableHasOfficialRecordEnabled(string tableName, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetOfficialRecordSettingForTable, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    return Convert.ToBoolean(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        private static DataTable CreateNewDirectory(string outputSettingName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(outputSettingName))
                outputSettingName = GetDefaultOutputSetting(conn);
            var Volumes = GetVolumesByID(GetVolumesIDFromOutputSetting(outputSettingName, conn), conn);
            if (Volumes.Rows.Count == 0)
                return new DataTable();

            string DirPath = string.Empty;
            string NewPart = string.Empty;
            string Path = Volumes.Rows[0]["FullPath"].ToString();
            if (!Path.EndsWith(@"\"))
                Path += @"\";

            try
            {
                for (int i = 1; i <= (int)Math.Round(Math.Pow(36d, 5d)) - 1; i++)
                {
                    NewPart = "SL" + ConvertToBase36(i);
                    DirPath = Path + NewPart;
                    // DB Image based
                    if (!string.IsNullOrEmpty(Volumes.Rows[0]["ImageTableName"].ToString()))
                    {
                        return UpdateOutputSettingWithDirectoryID(CreateNewDirectoryRecord(Convert.ToInt32(Volumes.Rows[0]["ID"]), NewPart, conn), outputSettingName, conn);
                    }
                    // Directory based
                    if (!Directory.Exists(DirPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(DirPath);
                        }
                        catch (Exception ex)
                        {
                            //Logs.Loginformation(string.Format("Error \"{0}\" in Attachments.CreateNewDirectory (path: {1} outputSettingName: {2})", ex.Message, DirPath, outputSettingName));
                            return new DataTable();
                        }

                        return UpdateOutputSettingWithDirectoryID(CreateNewDirectoryRecord(Convert.ToInt32(Volumes.Rows[0]["ID"]), NewPart, conn), outputSettingName, conn);
                    }
                }

                return new DataTable();
            }
            catch (Exception ex)
            {
                //Logs.Loginformation(string.Format("Error \"{0}\" in Attachments.CreateNewDirectory (outputSettingName: {1})", ex.Message, outputSettingName));
                return new DataTable();
            }
        }

        private static DataTable UpdateOutputSettingWithDirectoryID(int directoriesID, string outputSettingName, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.UpdateOutputSettingWithDirectoryID, conn))
            {
                cmd.Parameters.AddWithValue("@directoriesID", directoriesID);
                cmd.Parameters.AddWithValue("@name", outputSettingName);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private static string ConvertToBase36(int startingValue)
        {
            if (startingValue == -1)
                return string.Empty;

            string convertedValue = string.Empty;
            string base36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            while (startingValue != 0)
            {
                convertedValue = base36.Substring(startingValue % 36, 1) + convertedValue;
                startingValue = startingValue / 36;
            }

            if (string.IsNullOrEmpty(convertedValue))
                return string.Empty;
            return new string('0', 6 - convertedValue.Length) + convertedValue;
        }

        private static int CreateNewDirectoryRecord(int volumesID, string newPart, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.InsertNewDirectory, conn))
                {
                    cmd.Parameters.AddWithValue("@description", "Auto Created " + volumesID.ToString() + " " + newPart);
                    cmd.Parameters.AddWithValue("@dirFullFlag", false);
                    cmd.Parameters.AddWithValue("@path", @"\" + newPart);
                    cmd.Parameters.AddWithValue("@volumesID", volumesID);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static int LinkedRecordCount(int trackableId, SqlConnection conn)
        {
            object value = null;

            try
            {
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM UserLinks WHERE TrackablesId = @trackableId", conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", trackableId);
                    value = cmd.ExecuteScalar();
                    if (value is not null)
                        return Convert.ToInt32(value);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public static Attachment DeleteAttachment(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, bool childAttachmentsOn)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    string message = DeleteAttachment(ClientIpAddress, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, versionNumber, false, conn);
                    if (!string.IsNullOrEmpty(message))
                        return new ErrorAttachment(new Exception(message), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);
                }

                if (versionNumber > 0)
                    return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, 0, _thumbSize, true, childAttachmentsOn, false, false, -1);
                return GetAllAttachments(Ticket, userId, pass, tableName, tableId, 0, _thumbSize, childAttachmentsOn, false);
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
        }

        public static string DeleteAttachmentsForRow(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId)
        {
            var sb = new System.Text.StringBuilder();
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return Permissions.ExceptionString.NotAuthorized;
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return Permissions.ExceptionString.MissingParameters;

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    var dt = Attachments.GetRowsOfAttachments(userId, tableName, paddedTableId, 0, 0, Resources.GetAllAttachments, conn);
                    if (dt.Rows.Count == 0)
                        return string.Empty;

                    string message = string.Empty;
                    int attachmentNumber = -1;
                    string fieldName = "AttachmentNumber";

                    foreach (DataRow row in dt.Rows)
                    {
                        if (attachmentNumber != SafeInt(row, fieldName, tableName, paddedTableId, conn))
                        {
                            attachmentNumber = SafeInt(row, fieldName, tableName, paddedTableId, conn);
                            message = DeleteAttachment(ClientIpAddress, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, 0, true, conn);

                            if (!string.IsNullOrEmpty(message) && string.Compare(message, Permissions.ExceptionString.NoRecords) != 0)
                            {
                                sb.Append(string.Format("{0}{1}", message, Constants.vbCrLf));
                            }
                        }
                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string DeleteAttachment(string ClientIpAddress, int userId, string databaseName, string tableName, string paddedTableId, int attachmentNumber, int versionNumber, bool deleteAll, SqlConnection conn)
        {
            if (IsCheckedOut(tableName, paddedTableId, attachmentNumber, conn) > 0)
            {
                string message = string.Format(Permissions.ExceptionString.CannotDeleteWhenCheckedOut, (object)attachmentNumber);
                if (versionNumber > 0)
                    message = string.Format(Permissions.ExceptionString.CannotDeleteVersionWhenCheckedOut, (object)attachmentNumber);
                return message;
            }

            string sql;
            if (!deleteAll)
                deleteAll = attachmentNumber > 0 && versionNumber <= 0;

            if (deleteAll)
            {
                sql = string.Format(GetSqlAll(userId, tableName, paddedTableId, attachmentNumber, 0, conn), "=", "ASC");
            }
            else
            {
                sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, versionNumber, conn), "=", "ASC");
            }

            var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, versionNumber, sql, conn);
            if (dt.Rows.Count == 0)
                return Permissions.ExceptionString.NoRecords;

            int lastNumber = -1;
            string fieldName = "AttachmentNumber";
            if (versionNumber > 0)
                fieldName = "RecordVersion";
            int linkedCount = 0;
            if (versionNumber == 0)
                linkedCount = LinkedRecordCount(SafeInt(dt.Rows[0], "TrackablesId"), conn);

            if (linkedCount > 1)
            {
                DeleteUserlink(tableName, paddedTableId, SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn), conn);
            }
            else
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (versionNumber > 0 || deleteAll || lastNumber != SafeInt(row, fieldName, tableName, paddedTableId, conn))
                    {
                        lastNumber = SafeInt(row, fieldName, tableName, paddedTableId, conn);
                        DeletePointerParts(userId, row, false, conn);
                    }
                }

                if (lastNumber != -1)
                    DeleteTrackables(tableName, paddedTableId, attachmentNumber, versionNumber, dt.Rows[0], conn);
                DeleteIndexer(tableName, paddedTableId, attachmentNumber, versionNumber, conn);
                // The following line is to delete old SLIndexer records prior to attachment # being added to the table
                if (attachmentNumber == 1)
                    DeleteIndexer(tableName, paddedTableId, 0, 0, conn);
            }

            string action = "Deleted Attachment";
            string data = string.Format("Attachment: {0}", dt.Rows[0]["AttachmentNumber"]);

            if (linkedCount > 1)
            {
                action = "Deleted Attachment Link";
                data = string.Format("{1}{0}TrackablesId: {2}", Constants.vbCrLf, data, dt.Rows[0]["TrackablesId"]);
            }
            else if (versionNumber > 0)
            {
                action = "Deleted Version";
                data = string.Format("{1}{0}Version: {2}", Constants.vbCrLf, data, dt.Rows[0]["RecordVersion"]);
            }

            Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, action, data, string.Empty, AuditType.AttachmentViewerActionType.DeleteAttachment, conn);
            return string.Empty;
        }

        public static Attachment DeleteOrphan(string ClientIpAddress, string Ticket, int userId, Passport pass, int trackableID)
        {
            // If Not Encrypt.ValidateTicket(Ticket, userId, databaseName, OrphanName, String.Empty) Then Return New ErrorAttachment(New Exception(Exceptions.NotAuthorized), userId, databaseName, OrphanName, String.Empty)

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    var dt = GetOrphanRows(trackableID, conn);
                    if (dt.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.DatabaseName, OrphanName, string.Empty, new SecurityInfo(OrphanName, userId, conn), conn);

                    foreach (DataRow row in dt.Rows)
                        DeletePointerParts(userId, row, false, conn);

                    DeleteOrphanTrackables(trackableID, conn);
                    string data = string.Format("Orphan Id: {0}", trackableID);
                    Attachments.AuditUpdate(userId, string.Empty, string.Empty, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Deleted Orphan", data, string.Empty, AuditType.AttachmentViewerActionType.DeleteOrphan, conn);
                    return GetAllOrphans(Ticket, userId, pass, _thumbSize, false);
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.DatabaseName, OrphanName, string.Empty);
            }
        }

        private static void DeleteTrackables(string tableName, string tableId, int attachmentNumber, int versionNumber, DataRow row, SqlConnection conn)
        {
            int trackableId = 0;
            object value = null;
            string sql = Resources.DeleteAllTrackables;
            if (versionNumber > 0)
                sql = Resources.DeleteTrackable;

            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", Convert.ToInt32(row["TrackablesId"]));
                    if (versionNumber > 0)
                        cmd.Parameters.AddWithValue("@versionNumber", versionNumber);
                    value = cmd.ExecuteScalar();
                    if (value is not null)
                        trackableId = Convert.ToInt32(value);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                trackableId = -1;
            }

            if (trackableId == 0)
                DeleteUserlink(tableName, tableId, attachmentNumber, conn);
        }

        private static void DeleteOrphanTrackables(int trackableID, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.DeleteAllTrackables, conn))
                {
                    cmd.Parameters.AddWithValue("@trackableId", trackableID);
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void DeleteUserlink(string tableName, string tableId, int attachmentNumber, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.DeleteUserlink, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void DeleteIndexer(string tableName, string tableId, int attachmentNumber, int versionNumber, SqlConnection conn)
        {
            try
            {
                string versionString = string.Empty;
                if (versionNumber > 0)
                    versionString = " AND [RecordVersion] = " + versionNumber;
                if (!IdFieldIsString(tableName, conn))
                    tableId = StripLeadingZeros(tableId);

                using (var cmd = new SqlCommand(string.Format(Resources.DeleteIndexerRecords, versionString), conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static Attachment MarkOfficialRecord(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, bool childAttachmentsOn)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    MarkOfficialRecord(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, versionNumber, conn);
                    return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, _thumbSize, true, childAttachmentsOn, false, false, -1);
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
        }

        private static void MarkOfficialRecord(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string paddedTableId, int attachmentNumber, int versionNumber, SqlConnection conn)
        {
            var cmd = new SqlCommand(Resources.MarkOfficialRecord, conn);
            cmd.Parameters.AddWithValue("@tableName", tableName);
            cmd.Parameters.AddWithValue("@tableId", paddedTableId);
            cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
            cmd.Parameters.AddWithValue("@versionNumber", versionNumber);

            if (cmd.ExecuteNonQuery() > 0)
            {
                string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, attachmentNumber, versionNumber);
                Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Marked Official Record", string.Empty, data, AuditType.AttachmentViewerActionType.MarkOfficial, conn);
            }
        }

        public static string DeletePage(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, int pageNumber)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return Permissions.ExceptionString.NotAuthorized;
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return Permissions.ExceptionString.MissingParameters;

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    int currentPage = 0;
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    if (IsCheckedOutToMe(userId, tableName, paddedTableId, attachmentNumber, conn) == 0)
                        return "Pages can only be deleted when an attachment is checked out to the current user.";

                    var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, versionNumber, string.Format(Resources.GetVersion, "=", "ASC"), conn);
                    if (dt.Rows.Count == 0)
                        return string.Empty;
                    if ((AttachmentTypes)dt.Rows[0]["RecordType"] == AttachmentTypes.tkWPDoc)
                        return "Pages in non-image attachments can only be deleted from the native application.";

                    foreach (DataRow row in dt.Rows)
                    {
                        currentPage += 1;
                        if (pageNumber == currentPage)
                        {
                            DeletePointerParts(userId, row, true, conn);
                            string data = string.Format("Attachment: {1}{0}Version: {2}{0}Page: {3}", Constants.vbCrLf, attachmentNumber, versionNumber, pageNumber);
                            Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Deleted Page", data, string.Empty, AuditType.AttachmentViewerActionType.DeletePage, conn);
                            return string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public static Attachment GetAllOrphans(string Ticket, int userId, Passport pass, Size thumbsize, bool rebuildCache)
        {
            // If Not Encrypt.ValidateTicket(Ticket, userId, databaseName, OrphanName, String.Empty) Then Return New ErrorAttachment(New Exception(Exceptions.NotAuthorized), userId, databaseName, OrphanName, String.Empty)
            _thumbSize = thumbsize;

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    var security = new SecurityInfo(OrphanName, userId, conn);
                    // If security IsNot Nothing AndAlso Not OrphanPermission(security, Permissions.Orphan.View) Then Return New ErrorAttachment(New Exception(Exceptions.NoRecords), userId, databaseName, OrphanName, String.Empty, security, conn)

                    using (var da = new SqlDataAdapter(new SqlCommand(Resources.GetAllOrphansWCF, conn)))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                            return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, OrphanName, string.Empty, security, conn);
                        var rtn = GetValidAttachment(userId, pass.ServerAndDatabaseName, string.Empty, string.Empty, (AttachmentTypes)dt.Rows[0]["RecordType"], 1, dt.Rows[0], conn);

                        string fieldName = "TrackablesId";
                        int lastNumber = -1;
                        var itemNumber = default(int);

                        for (int index = 0, loopTo = dt.Rows.Count - 1; index <= loopTo; index++)
                        {
                            if (lastNumber != Convert.ToInt32(dt.Rows[index][fieldName]))
                            {
                                lastNumber = Convert.ToInt32(dt.Rows[index][fieldName]);
                                rtn.AddParts(lastNumber, dt.Rows[index], false, conn);
                                rtn.AddThumbnails(index, -1, userId, lastNumber, dt.Rows, 1, false, false, itemNumber + 1, conn);
                                itemNumber += 1;
                            }
                            else if (lastNumber != -1)
                            {
                                var part = rtn.AttachmentParts[rtn.AttachmentParts.Count - 1];
                                part.ResetPageCount(rtn, pass.ServerAndDatabaseName, dt.Rows[index]["ImageTableName"].ToString(), dt.Rows[index]["FullPath"].ToString(), (AttachmentTypes)dt.Rows[0]["RecordType"], conn);

                                if (part.PageCount > 1)
                                {
                                    rtn.ThumbsList[rtn.ThumbsList.Count - 1].MultiPage = true;
                                }
                            }
                        }

                        return rtn;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, OrphanName, string.Empty);
            }
        }
        public static DataTable GetAllOrphans(Passport passport, string filter, int pageIndex, int PerPageRecord)
        {
            if (!(pageIndex == 0))
            {
                pageIndex = (pageIndex - 1) * PerPageRecord;
            }
            if (filter is null)
            {
                filter = "";
            }
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                using (var cmd = new SqlCommand(Resources.GetAllOrphans, conn))
                {
                    cmd.Parameters.AddWithValue("@OFFSET", pageIndex);
                    cmd.Parameters.AddWithValue("@FILTER", filter);
                    cmd.Parameters.AddWithValue("@PerPageRecord", PerPageRecord);
                    cmd.Parameters.AddWithValue("@userid", passport.UserId);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        public static DataTable GetOrphansPerPageRecord(Passport passport)
        {
            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                using (var cmd = new SqlCommand("select ItemValue from settings where Section = 'Vault' and Item = 'PerPageRecord'", conn))
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
        public static DataTable GetAllOrphansCount(Passport passport, string filter)
        {
            string Query = Resources.GetAllOrphansCount;
            if (filter is null)
            {
                filter = "";
            }

            using (var conn = new SqlConnection(passport.ConnectionString))
            {
                using (var cmd = new SqlCommand(Query, conn))
                {
                    cmd.Parameters.AddWithValue("@FILTER", filter);
                    cmd.Parameters.AddWithValue("@userid", passport.UserId);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        private static DataTable GetOrphanRows(int orphanId, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(string.Format(Resources.GetOrphan, "=", "ASC"), conn))
                {
                    cmd.Parameters.AddWithValue("@orphanId", orphanId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataTable();
            }
        }
        public static bool CheckOrphanDeleted(string TrackableIDs, Passport pass)
        {
            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    var dt = new DataTable();
                    using (var cmd = new SqlCommand("select COUNT(*) from Trackables where id in (" + TrackableIDs + ")", conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    if (dt.Rows.Count > 0)
                    {
                        int co = Convert.ToInt32(dt.Rows[0][0]);
                        int length = TrackableIDs.Split(',').Length;
                        if (co != length)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool CheckOrphanAvailable(string TrackableIDs, Passport pass)
        {
            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    var dt = new DataTable();
                    // Orphan==0 (Orphan is not attached to any record)
                    using (var cmd = new SqlCommand("select COUNT(*) from Trackables where id in (" + TrackableIDs + ") and Orphan = 0", conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    if (dt.Rows.Count > 0)
                    {
                        int co = Convert.ToInt32(dt.Rows[0][0]);
                        if (co > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static int GetVersionCount(string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber)
        {
            return GetCount(Ticket, userId, pass, tableName, tableId, attachmentNumber, 0);
        }

        public static int GetPageCount(string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber)
        {
            return GetCount(Ticket, userId, pass, tableName, tableId, attachmentNumber, versionNumber);
        }

        public static int GetCount(string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return 0;
            return GetCount(userId, pass, tableName, tableId, attachmentNumber, versionNumber);
        }

        private static int GetCount(int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber)
        {
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return 0;

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    tableId = PadTableId(tableName, tableId, conn);
                    string sql = string.Format(GetSqlAll(userId, tableName, tableId, attachmentNumber, versionNumber, conn), "=", "ASC");
                    var dt = GetRowsOfAttachments(userId, tableName, tableId, attachmentNumber, versionNumber, sql, conn);
                    if (versionNumber > 0)
                        return CountPages(pass.ServerAndDatabaseName, dt.Rows, conn);

                    string fieldName = "AttachmentNumber";
                    if (attachmentNumber > 0)
                        fieldName = "RecordVersion";
                    return CountDistinctRows(dt.Rows, tableName, userId, fieldName, conn);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static int CountPages(string databaseName, DataRowCollection rows, SqlConnection conn)
        {
            var count = default(int);

            foreach (DataRow row in rows)
            {
                bool isAnImage = (AttachmentTypes)row["RecordType"] != AttachmentTypes.tkWPDoc;
                var part = new AttachmentPart(row);
                count += part.GetPageCount(databaseName, row["ImageTableName"].ToString(), row["FullPath"].ToString(), isAnImage, conn);
            }

            return count;
        }

        private static int CountDistinctRows(DataRowCollection rows, string tableName, int userId, string fieldName, SqlConnection conn)
        {
            int rtn = 0;
            int lastNumber = -1;

            foreach (DataRow row in rows)
            {
                if (lastNumber != Convert.ToInt32(row[fieldName]))
                {
                    lastNumber = Convert.ToInt32(row[fieldName]);
                    var security = new SecurityInfo(tableName, userId, SafeInt(row, "ScanDirectoriesId"), conn);
                    if (Attachments.VolumePermission(security, Permissions.Volume.Access))
                        rtn += 1;
                }
            }

            return rtn;
        }

        private static string GetSqlAll(int userId, string tableName, string paddedTableId, int attachmentNumber, int versionNumber, SqlConnection conn)
        {
            int checkedOutVersionNumber = 0;

            using (var cmd = new SqlCommand(Resources.IsCheckedOutToMe, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                if (attachmentNumber > 0)
                {
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@attachmentNumber", 1);
                }
                cmd.Parameters.AddWithValue("@userId", userId);
                checkedOutVersionNumber = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return GetSqlAll(attachmentNumber, versionNumber, checkedOutVersionNumber > 0);
        }

        private static string GetSqlAll(int attachmentNumber, int versionNumber, bool checkedOutToMe)
        {
            if (versionNumber != 0)
            {
                if (checkedOutToMe)
                    return Resources.GetCheckedOutVersion;
                return Resources.GetVersion;
            }
            if (attachmentNumber > 0)
            {
                if (checkedOutToMe)
                    return Resources.GetAllVersionsWithCheckedOut;
                return Resources.GetAllVersions;
            }

            if (checkedOutToMe)
                return Resources.GetAllAttachmentsWithCheckOut;
            return Resources.GetAllAttachments;
        }

        private static string GetSql(int userId, string tableName, string tableId, int attachmentNumber, int versionNumber, SqlConnection conn)
        {
            int checkedOutVersionNumber = 0;

            using (var cmd = new SqlCommand(Resources.IsCheckedOutToMe, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                if (attachmentNumber > 0)
                {
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@attachmentNumber", 1);
                }
                cmd.Parameters.AddWithValue("@userId", userId);
                checkedOutVersionNumber = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return GetSql(attachmentNumber, versionNumber, checkedOutVersionNumber > 0);
        }

        private static string GetSql(int attachmentNumber, int versionNumber, bool checkedOutToMe)
        {
            if (attachmentNumber > 0 && versionNumber != 0)
            {
                if (checkedOutToMe)
                    return Resources.GetCheckedOutVersion;
                return Resources.GetVersion;
            }
            if (attachmentNumber > 0)
            {
                if (checkedOutToMe)
                    return Resources.GetCheckedOutVersion; // GetAllVersionsWithCheckedOut
                return Resources.GetAttachment;
            }

            // If checkedOutToMe Then Return ""
            return Resources.GetFirstAttachment;
        }
        //moti mashiah
        private static Attachment GetValidAttachment(int userId, string databaseName, string tableName, string paddedTableId, AttachmentTypes attachmentType, int attachmentNumber, DataRow row, SqlConnection conn)
        {
            try
            {
                switch (attachmentType)
                {
                    case AttachmentTypes.tkImage:
                    case AttachmentTypes.tkFax:
                        {
                            return new ImageAttachment(userId, tableName, paddedTableId, databaseName, _thumbSize, row, conn);
                        }
                    case AttachmentTypes.tkWPDoc:
                        {
                            return new FileAttachment(userId, tableName, paddedTableId, databaseName, _thumbSize, row, conn);
                        }

                    default:
                        {
                            return new ErrorAttachment(new Exception("Invalid Record Type"), userId, databaseName, tableName, paddedTableId, new SecurityInfo(tableName, userId, conn), conn);
                        }
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, databaseName, tableName, paddedTableId, new SecurityInfo(tableName, userId, conn), conn);
            }

            // If rtn.Security.VolumePermissions.Count > 0 Then Return rtn
            // Return New ErrorAttachment(New Exception(permissions.NotAuthorized))
        }

        public static DataTable GetRowsOfAttachments(int userId, string tableName, string paddedTableId, string sql, SqlConnection conn)
        {
            return GetRowsOfAttachments(userId, tableName, paddedTableId, 0, 0, sql, conn);
        }

        public static DataTable GetRowsOfAttachments(int userId, string tableName, string paddedtableId, int attachmentNumber, int versionNumber, string sql, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", paddedtableId);
                cmd.Parameters.AddWithValue("@userId", userId);
                if (attachmentNumber > 0)
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                if (versionNumber != 0)
                    cmd.Parameters.AddWithValue("@versionNumber", versionNumber);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        //public static Page GetPage(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, int attachmentNumber, int versionNumber, int pageNumber, string pagesOnlyTableId, bool childAttachmentsOn, bool suppressAuditRecord, int OcrDpiForDocuments)
        //{
        //    return GetPage(ClientIpAddress, Ticket, userId, databaseName, tableName, tableId, attachmentNumber, versionNumber, pageNumber, pagesOnlyTableId, childAttachmentsOn, suppressAuditRecord, OcrDpiForDocuments, 1);
        //}

        //public static Page GetPage(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, int attachmentNumber, int versionNumber, int pageNumber, string pagesOnlyTableId, bool childAttachmentsOn, bool suppressAuditRecord)
        //{
        //    return GetPage(ClientIpAddress, Ticket, userId, databaseName, tableName, tableId, attachmentNumber, versionNumber, pageNumber, pagesOnlyTableId, childAttachmentsOn, suppressAuditRecord, 0);
        //}

        //public static Page GetPage(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, int attachmentNumber, int versionNumber, int pageNumber, string pagesOnlyTableId, bool childAttachmentsOn, bool suppressAuditRecord, int OcrDpiForDocuments, int attachmentType)
        //{
        //    Attachment attachment = null;

        //    if (string.IsNullOrEmpty(tableName) | string.Compare(tableName, OrphanName) == 0)
        //    {
        //        if (!Encrypt.ValidateTicket(Ticket, userId, databaseName, OrphanName, string.Empty))
        //            throw new Exception(Permissions.ExceptionString.NotAuthorized);
        //        attachmentNumber = -1;
        //        if (!string.IsNullOrEmpty(pagesOnlyTableId))
        //        {
        //            attachment = GetOrphanById(Ticket, userId, databaseName, pagesOnlyTableId, pageNumber, attachmentType, _thumbSize);
        //        }
        //        else
        //        {
        //            attachment = GetOrphan(Ticket, userId, databaseName, versionNumber, _thumbSize, false, false);
        //        }
        //    }
        //    else
        //    {
        //        if (!Encrypt.ValidateTicket(Ticket, userId, databaseName, tableName, tableId))
        //            throw new Exception(Permissions.ExceptionString.NotAuthorized);
        //        if (attachmentNumber <= 0)
        //            attachmentNumber = 1;
        //        if (!string.IsNullOrEmpty(pagesOnlyTableId))
        //            tableId = pagesOnlyTableId;
        //        attachment = GetAttachment(ClientIpAddress, userId, databaseName, tableName, tableId, attachmentNumber, versionNumber, _thumbSize, false, childAttachmentsOn, suppressAuditRecord, false, -1);
        //        if (!Attachments.AttachmentPermission(attachment, Permissions.Attachment.View))
        //            throw new Exception(Permissions.ExceptionString.NoRecords);
        //        if (!Attachments.VolumePermission(attachment, Permissions.Volume.View))
        //            throw new Exception(Permissions.ExceptionString.NoRecords);
        //    }

        //    if (pageNumber <= 0)
        //        pageNumber = 1;

        //    if (attachment is ErrorAttachment)
        //    {
        //        throw new Exception(((ErrorAttachment)attachment).Message);
        //    }
        //    else if (attachment is ImageAttachment)
        //    {
        //        int pageCount = attachment.get_PageCount(attachmentNumber, tableName, tableId);
        //        if (pageNumber > pageCount)
        //            pageNumber = pageCount;
        //        return ((ImageAttachment)attachment).Pages(pageNumber - 1);
        //    }
        //    else
        //    {
        //        return ((FileAttachment)attachment).Pages(pageNumber - 1, OcrDpiForDocuments);
        //    }
        //}

        //public static Page GetPageById(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, string pointerId, int pageNumber, int attachmentType)
        //{
        //    Attachment attachment = null;
        //    attachment = GetAttachmentById(ClientIpAddress, Ticket, userId, databaseName, tableName, tableId, pointerId, pageNumber, attachmentType, _thumbSize);
        //    if (!Attachments.AttachmentPermission(attachment, Permissions.Attachment.View))
        //        throw new Exception(Permissions.ExceptionString.NoRecords);
        //    if (!Attachments.VolumePermission(attachment, Permissions.Volume.View))
        //        throw new Exception(Permissions.ExceptionString.NoRecords);
        //    if (pageNumber <= 0)
        //        pageNumber = 1;

        //    if (attachment is ErrorAttachment)
        //    {
        //        throw new Exception(((ErrorAttachment)attachment).Message);
        //    }
        //    else if (attachment is ImageAttachment)
        //    {
        //        if (pageNumber > attachment.get_PageCount(Convert.ToInt32(pointerId), tableName, tableId))
        //            pageNumber = attachment.get_PageCount(Convert.ToInt32(pointerId), tableName, tableId);
        //        if (pageNumber <= 0)
        //            pageNumber = 1;
        //        return ((ImageAttachment)attachment).Pages(pageNumber - 1);
        //    }
        //    else
        //    {
        //        return ((FileAttachment)attachment).Pages(pageNumber - 1, 0);
        //    }
        //}

        //public static Page GetRecognizedWords(string ClientIpAddress, string Ticket, int userId, string databaseName, string tableName, string tableId, int attachmentNumber, int versionNumber, int pageNumber, string pagesOnlyTableId, int annotationDrawMode, int OcrDpiForDocuments)
        //{
        //    var page = GetPage(ClientIpAddress, Ticket, userId, databaseName, tableName, tableId, attachmentNumber, versionNumber, pageNumber, pagesOnlyTableId, false, true, OcrDpiForDocuments);

        //    if (page is not null)
        //    {
        //        page.FillRecognizedWords(annotationDrawMode, true);
        //        page.Annotations = GetSetting(databaseName, "Annotations", "FoundTextColor");
        //    }

        //    return page;
        //}

        public static Page GetRecognizedWords(string Ticket, int userId, string databaseName, string tableName, string tableId, Bitmap bmp, int annotationDrawMode, int OcrDpiForDocuments, bool Sorted)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, databaseName, tableName, tableId))
                return null;

            SetupCodec();
            var ocrPage = new ImagePage();
            var codec = new Leadtools.Codecs.RasterCodecs();
            ocrPage.FillRecognizedWords(bmp, annotationDrawMode, Sorted);
            return ocrPage;
        }



        private static byte[] GetStream(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length <= 0L | fs.Length > int.MaxValue)
                    return null;
                int read = 0;
                var buffer = new byte[((int)fs.Length)];
                int chunk = fs.Read(buffer, read, buffer.Length - read);

                while (chunk > 0)
                {
                    read += chunk;
                    // If we've reached the end of our buffer, check to see if there's any more information
                    if (read == buffer.Length)
                    {
                        int nextByte = fs.ReadByte();
                        // End of stream? If so, we're done
                        if (nextByte == -1)
                            return buffer;
                        // Otherwise, resize the buffer, put in the byte we've just read, and continue
                        var NewBuffer = new byte[buffer.Length * 2 + 1];
                        Array.Copy(buffer, NewBuffer, buffer.Length);
                        NewBuffer[read] = (byte)nextByte;
                        buffer = NewBuffer;
                        read += 1;
                    }

                    chunk = fs.Read(buffer, read, buffer.Length - read);
                }
                // Buffer is now too big so let's shrink it.
                var rtn = new byte[read];
                Array.Copy(buffer, rtn, read);
                buffer = null;
                return rtn;
            }
        }

        internal static byte[] GetDatabaseImageStream(string databaseName, string imageTableName, string fileName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(fileName) || !Information.IsNumeric(fileName))
                throw new Exception(Permissions.ExceptionString.InvalidImage);

            var dt = FillDataTable(string.Format(Resources.GetDatabaseImage, imageTableName), conn, "@recordId", fileName);
            if (dt.Rows.Count == 0)
                throw new Exception(Permissions.ExceptionString.ImageNotFound);
            return (byte[])dt.Rows[0]["ImageField"];
        }

        private static int IsCheckedOut(string tableName, string tableId, int attachmentNumber, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsCheckedOut, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static int IsCheckedOutToMe(int userId, string tableName, string tableId, int attachmentNumber, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsCheckedOutToMe, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public static Attachment CheckIn(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, string outputSettingName, bool asOfficialRecord, string extension, bool isAnImage, string fileName, bool childAttachmentsOn)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    long currentFileSize = 0L;
                    int newVersionNumber;
                    string paddedTableId = PadTableId(tableName, tableId, conn);

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        currentFileSize = new FileInfo(fileName).Length;
                    }
                    else
                    {
                        currentFileSize = GetNewAttachmentSize(userId, tableName, paddedTableId, attachmentNumber, conn);
                    }

                    var outputSetting = FindValidOutputSetting(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, versionNumber, outputSettingName, currentFileSize, conn);
                    if (outputSetting.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingOutputSetting), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    using (var cmd = new SqlCommand(Resources.CheckIn, conn))
                    {
                        cmd.Parameters.AddWithValue("@tableName", tableName);
                        cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                        cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                        cmd.Parameters.AddWithValue("@officialRecord", asOfficialRecord);
                        newVersionNumber = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (asOfficialRecord)
                        MarkOfficialRecord(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, newVersionNumber, conn);

                    UpdatePointers(newVersionNumber, tableName, paddedTableId, attachmentNumber, isAnImage, conn);
                    CopyNewFiles(userId, tableName, paddedTableId, attachmentNumber, newVersionNumber, extension, isAnImage, fileName, outputSetting.Rows[0], conn);
                    var rtn = GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, 0, _thumbSize, IsLeadtools14(), childAttachmentsOn, false, false, -1);
                    string data = string.Format("Attachment: {1}{0}Version: {2}{0}Official Record: {3}", Constants.vbCrLf, (object)rtn.AttachmentNumber, (object)rtn.VersionInfo.Version, asOfficialRecord.ToString());
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "CheckIn", string.Empty, data, AuditType.AttachmentViewerActionType.CheckIn, conn);
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
            finally
            {
                Exporter.DeleteFile(fileName);
            }
        }

        internal static DataTable GetOutputSetting(string outputSettingName, int userId, long currentFileSize, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(outputSettingName))
                return new DataTable();

            try
            {
                using (var cmd = new SqlCommand(Resources.GetOutputSetting, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", outputSettingName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0 && (Convert.ToBoolean(dt.Rows[0]["Inactive"]) || !Convert.ToBoolean(dt.Rows[0]["VolumeActive"])))
                            return new DataTable();
                        if (dt.Rows.Count == 0)
                            dt = GetOutputSettingWithoutDirectory(outputSettingName, userId, currentFileSize, conn);
                        if (dt.Rows.Count == 0)
                            return CreateNewDirectory(outputSettingName, conn);
                        return PositionToAvailableDirectory(dt.Rows[0]["ImageTableName"].ToString(), Convert.ToInt32(dt.Rows[0]["VolumesId"]), outputSettingName, currentFileSize, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                //Logs.Loginformation(string.Format("Error \"{0}\" in Attachments.GetOutputSetting (outputSettingName: {1}, currentFileSize: {2})", ex.Message, outputSettingName, currentFileSize));
                return new DataTable();
            }
        }

        private static DataTable GetOutputSettingWithoutDirectory(string outputSettingName, int userId, long currentFileSize, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(outputSettingName))
                return new DataTable();

            try
            {
                using (var cmd = new SqlCommand(Resources.GetOutputSettingWithoutDirectory, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", outputSettingName);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count > 0 && (Convert.ToBoolean(dt.Rows[0]["Inactive"]) || !Convert.ToBoolean(dt.Rows[0]["VolumeActive"])))
                            return new DataTable();
                        if (dt.Rows.Count == 0)
                            return CreateNewDirectory(outputSettingName, conn);
                        return PositionToAvailableDirectory(dt.Rows[0]["ImageTableName"].ToString(), Convert.ToInt32(dt.Rows[0]["VolumesId"]), outputSettingName, currentFileSize, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Attachments.GetOutputSettingWithoutDirectory (outputSettingName: {1}, currentFileSize: {2})", ex.Message, outputSettingName, currentFileSize));
            }
        }

        private static DataTable PositionToAvailableDirectory(string imageTableName, int volumeId, string outputSettingName, long currentFileSize, SqlConnection conn)
        {
            try
            {
                string imageTableFilter = "AND ((Volumes.ImageTableName = '') OR (Volumes.ImageTableName IS NULL))";
                if (!string.IsNullOrEmpty(imageTableName))
                    imageTableFilter = string.Empty;

                using (var cmd = new SqlCommand(string.Format(Resources.GetAllDirectoriesByVolumeId, imageTableFilter), conn))
                {
                    cmd.Parameters.AddWithValue("@volumeId", volumeId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                            return CreateNewDirectory(outputSettingName, conn);

                        foreach (DataRow row in dt.Rows)
                        {
                            int directoryId = Convert.ToInt32(row["DirectoriesId"].ToString());
                            if (!DirectoryLimitsExceeded(row, imageTableName, directoryId, currentFileSize, conn))
                            {
                                return UpdateOutputSettingWithDirectoryID(directoryId, outputSettingName, conn);
                            }
                        }

                        return CreateNewDirectory(outputSettingName, conn);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Attachments.PositionToAvailableDirectory (imageTableName: {1}, volumeId: {2}, outputSettingName: {3}, currentFileSize: {4})", ex.Message, imageTableName, volumeId, outputSettingName, currentFileSize));
            }
        }

        private static bool DirectoryLimitsExceeded(DataRow row, string imageTableName, int directoryId, long currentFileSize, SqlConnection conn)
        {
            if (!string.IsNullOrEmpty(imageTableName))
                return false;

            string fullPath = row["FullPath"].ToString();
            if (!Directory.Exists(fullPath))
                return true;

            var directoryFull = default(bool);
            if (!(row["DirFullFlag"] is DBNull))
                directoryFull = Convert.ToBoolean(row["DirFullFlag"].ToString());
            if (directoryFull)
                return true;

            int directoryFileLimit = 0;
            long directorySizeLimit = 0L;
            if (!(row["DirCountLimitation"] is DBNull))
                directoryFileLimit = Convert.ToInt32(row["DirCountLimitation"].ToString());
            if (!(row["DirDiskMBLimitation"] is DBNull))
                directorySizeLimit = Convert.ToInt64(row["DirDiskMBLimitation"].ToString());

            if (directorySizeLimit == 0L && directoryFileLimit == 0)
                return false;

            var numberofFiles = default(int);
            double diskSize = (GetFolderSize(fullPath, ref numberofFiles) + currentFileSize) / 1048576.0d;
            var rtn = default(bool);

            if (directorySizeLimit != 0L && diskSize >= directorySizeLimit)
                rtn = true;
            if (directoryFileLimit != 0 && numberofFiles >= directoryFileLimit)
                rtn = true;

            if (rtn)
            {
                try
                {
                    using (var cmd = new SqlCommand(Resources.UpdateDirectoryDirFullFlag, conn))
                    {
                        cmd.Parameters.AddWithValue("@flag", 1);
                        cmd.Parameters.AddWithValue("@Id", directoryId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return rtn;
        }

        [SecuritySafeCritical]
        private static double GetFolderSize(string path, ref int numberOfFiles)
        {
            double rtn;
            Scripting.Folder oFolder;
            Scripting.FileSystemObject oFileSystem;

            rtn = -1.0d;
            numberOfFiles = (int)-1L;

            try
            {
                oFileSystem = new Scripting.FileSystemObject();
                oFolder = oFileSystem.GetFolder(path);
                rtn = Convert.ToDouble(oFolder.Size);
                numberOfFiles = oFolder.Files.Count;
            }
            catch (Exception ex)
            {
                //Logs.Loginformation(string.Format("Error \"{0}\" in Attachments.GetFolderSize (path: {1}, numberOfFiles: {2})", ex.Message, path, numberOfFiles));
            }
            finally
            {
                oFolder = null;
                oFileSystem = null;
            }

            return rtn;
        }

        private static DataTable FindValidOutputSetting(int userId, string databaseName, string tableName, string paddedTableId, int attachmentNumber, int versionNumber, string outputSettingName, long currentFileSize, SqlConnection conn)
        {
            try
            {
                var dt = GetOutputSetting(outputSettingName, userId, currentFileSize, conn);
                if (dt.Rows.Count == 0)
                    dt = FindValidOutputSetting(userId, databaseName, tableName, paddedTableId, attachmentNumber, versionNumber, conn);

                if (dt.Rows.Count > 0)
                {
                    outputSettingName = dt.Rows[0]["Id"].ToString();
                    dt = GetOutputSetting(outputSettingName, userId, currentFileSize, conn);
                }

                if (dt.Rows.Count == 0)
                    dt = GetOutputSetting(GetDefaultOutputSetting(conn), userId, currentFileSize, conn);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        private static DataTable FindValidOutputSetting(int userId, string databaseName, string tableName, string paddedTableId, int attachmentNumber, int versionNumber, SqlConnection conn)
        {
            try
            {
                string sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, versionNumber, conn), "=", "ASC");
                var attachmentTable = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, versionNumber, sql, conn);
                if (attachmentTable.Rows.Count == 0)
                    return attachmentTable;
                return FindValidOutputSetting(Convert.ToInt32(attachmentTable.Rows[0]["ScanDirectoriesId"]), conn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        public static DataTable FindValidOutputSetting(int directoriesId, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetOutputSettingByDirectoriesId, conn))
                {
                    cmd.Parameters.AddWithValue("@directoriesId", directoriesId);

                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0 && (Convert.ToBoolean(dt.Rows[0]["Inactive"]) || !Convert.ToBoolean(dt.Rows[0]["VolumeActive"])))
                        return new DataTable();
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        internal static string GetDefaultOutputSetting(SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetDefaultOutputSettingName, conn))
            {
                return cmd.ExecuteScalar().ToString();
            }
        }

        private static int GetDefaultOutputSettingDirectoryID(SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetDefaultOutputSettingDirectoryID, conn))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static int GetVolumesIDFromOutputSetting(string outputSettingName, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetVolumeIDFromOutputSetting, conn))
            {
                cmd.Parameters.AddWithValue("@Id", outputSettingName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private static DataTable GetVolumesByID(int volumesID, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetVolumeByID, conn))
            {
                cmd.Parameters.AddWithValue("@Id", volumesID);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private static void CopyNewFiles(int userId, string tableName, string tableId, int attachmentNumber, int versionNumber, string extension, bool isAnImage, string fileName, DataRow row, SqlConnection conn, int totalPages)
        {
            using (var cmd = new SqlCommand(string.Format(Resources.GetVersion, "=", "ASC"), conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.Parameters.AddWithValue("@versionNumber", versionNumber);

                using (var da = new SqlDataAdapter(cmd))
                {
                    string pcFileExtension = string.Empty;
                    if (!string.IsNullOrEmpty(fileName))
                        pcFileExtension = extension;
                    var dt = new DataTable();
                    da.Fill(dt);
                    var update = new UpdateAttachment(row, GetUserName(userId, conn), pcFileExtension, conn);
                    update.CopyNewFiles(dt, isAnImage, fileName, totalPages);
                }
            }
        }

        private static void CopyNewFiles(int userId, string tableName, string tableId, int attachmentNumber, int versionNumber, string extension, bool isAnImage, string fileName, DataRow row, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(string.Format(Resources.GetVersion, "=", "ASC"), conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.Parameters.AddWithValue("@versionNumber", versionNumber);

                using (var da = new SqlDataAdapter(cmd))
                {
                    string pcFileExtension = string.Empty;
                    if (!string.IsNullOrEmpty(fileName))
                        pcFileExtension = extension;
                    var dt = new DataTable();
                    da.Fill(dt);
                    var update = new UpdateAttachment(row, GetUserName(userId, conn), pcFileExtension, conn);
                    update.CopyNewFiles(dt, isAnImage, fileName);
                }
            }
        }

        private static long GetNewAttachmentSize(int userId, string tableName, string paddedTableId, int attachmentNumber, SqlConnection conn)
        {
            using (var cmd = new SqlCommand(Resources.GetCheckedOutVersion, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.Parameters.AddWithValue("@userID", userId);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0)
                        return 0L;

                    long totalSize = 0L;
                    foreach (DataRow row in dt.Rows)
                    {
                        string fileName = row["FileName"].ToString();
                        string fullPath = row["FullPath"].ToString();

                        if (!string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
                        {
                            if (!string.IsNullOrEmpty(fileName) && Information.IsNumeric(fileName))
                            {
                                fullPath = string.Empty;
                            }
                        }

                        if (!string.IsNullOrEmpty(fullPath))
                        {
                            var fileInfo = new FileInfo(fullPath);
                            totalSize += fileInfo.Length;
                        }
                    }
                    return totalSize;
                }
            }
        }

        public static Attachment CheckOut(string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, string checkedOutFolder, string ClientIpAddress, string MACAddress, bool childAttachmentsOn)
        {
            return CheckOut(Ticket, userId, pass, tableName, tableId, attachmentNumber, versionNumber, checkedOutFolder, ClientIpAddress, MACAddress, childAttachmentsOn, true);
        }

        public static Attachment CheckOut(string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, string checkedOutFolder, string ClientIpAddress, string MACAddress, bool childAttachmentsOn, bool persistedCheckout)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    if (IsCheckedOut(tableName, paddedTableId, attachmentNumber, conn) > 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.AlreadyCheckedOut), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);
                    string sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, versionNumber, conn), "=", "ASC");
                    var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, versionNumber, sql, conn);
                    if (dt.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, -1, conn), conn);

                    attachmentNumber = Convert.ToInt32(dt.Rows[0]["AttachmentNumber"]);
                    var tempAttachment = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);

                    if ((AttachmentTypes)dt.Rows[0]["RecordType"] == AttachmentTypes.tkWPDoc)
                    {
                        checkedOutFolder = string.Format(@"{0}\.{1}", checkedOutFolder, dt.Rows[0]["FileName"].ToString());
                    }
                    else
                    {
                        checkedOutFolder = string.Empty;
                    }

                    int newVersionNumber = CheckOutAttachment(userId, tableName, paddedTableId, attachmentNumber, versionNumber, checkedOutFolder, ClientIpAddress, MACAddress, persistedCheckout, conn);

                    if (newVersionNumber < 0)
                    {
                        tempAttachment.BurstMultiPage(dt, newVersionNumber, conn);
                        string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, (object)tempAttachment.AttachmentNumber, (object)tempAttachment.VersionInfo.Version);
                        Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "CheckOut", string.Empty, data, AuditType.AttachmentViewerActionType.CheckOut, conn);
                    }

                    return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, _thumbSize, IsLeadtools14(), childAttachmentsOn, false, false, -1);
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
        }
        private static int CheckOutAttachment(int userId, string tableName, string tableId, int attachmentNumber, int versionNumber, string checkedOutFolder, string IPAddress, string MACAddress, bool persistedCheckout, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(string.Format(Resources.CheckOut, (object)Math.Abs(Convert.ToInt32(persistedCheckout))), conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    cmd.Parameters.AddWithValue("@versionNumber", versionNumber);
                    cmd.Parameters.AddWithValue("@checkedOutFolder", checkedOutFolder);
                    cmd.Parameters.AddWithValue("@userName", GetUserName(userId, conn));
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
                    cmd.Parameters.AddWithValue("@MACAddress", MACAddress);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static void DeletePointerParts(int userId, DataRow row, bool updatePageCount, SqlConnection conn)
        {
            int pointerId = Convert.ToInt32(row["PointerId"]);
            string imageTableName = row["ImageTableName"].ToString();
            string fullPath = row["FullPath"].ToString();

            if (!string.IsNullOrEmpty(imageTableName))
                fullPath = row["FileName"].ToString();
            if (updatePageCount)
                UpdateTrackablePageCount(Convert.ToInt32(row["TrackablesId"]), Convert.ToInt32(row["RecordVersion"]), userId, imageTableName, fullPath, conn);

            Attachment.DeleteFile(imageTableName, fullPath, conn);
            Attachment.DeleteAnnotations((AttachmentTypes)row["RecordType"], pointerId.ToString(), conn);
            Attachment.DeletePointer((AttachmentTypes)row["RecordType"], pointerId, conn);
        }

        private static void UpdateTrackablePageCount(int trackableId, int versionNumber, int userId, string imageTableName, string fullPath, SqlConnection conn)
        {
            byte[] fileStream = null;

            if (string.IsNullOrEmpty(imageTableName))
            {
                fileStream = Output.FileToByteArray(fullPath);
            }
            else
            {
                fileStream = Attachment.BlobImageToStream(imageTableName, fullPath, conn);
            }

            if (fileStream is not null)
            {
                int count = 1;
                var info = UpdateAttachment.GetCodecInfoFromStream(fileStream);
                if (info is not null)
                    count = info.TotalPages;

                UpdateAttachment.UpdateTrackablePageCount(trackableId, versionNumber, userId, -count, conn);
            }
        }

        private static void UpdatePointers(int newVersionNumber, string tablename, string tableId, int attachmentNumber, bool isAnImage, SqlConnection conn)
        {
            string sql = Resources.UpdatePCFilesPointers;
            if (isAnImage)
                sql = Resources.UpdateImagePointers;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", tablename);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.Parameters.AddWithValue("@newVersionNumber", newVersionNumber);
                int i = cmd.ExecuteNonQuery();
            }
        }

        private static string GetUserName(int userId, SqlConnection conn)
        {
            string argdisplayName = null;
            return GetUserName(userId, ref argdisplayName, conn);
        }

        private static string GetUserName(int userId, ref string displayName, SqlConnection conn)
        {
            string rtn = string.Empty;

            try
            {
                using (var cmd = new SqlCommand(Resources.GetUserName, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);

                        try
                        {
                            rtn = dt.Rows[0]["UserName"].ToString();
                        }
                        catch (Exception innerex)
                        {
                            Debug.WriteLine(innerex.Message);
                            rtn = userId.ToString();
                        }

                        if (displayName is not null)
                        {
                            try
                            {
                                displayName = dt.Rows[0]["DisplayName"].ToString();
                            }
                            catch (Exception innerex2)
                            {
                                Debug.WriteLine(innerex2.Message);
                                displayName = rtn;
                            }

                            if (string.IsNullOrWhiteSpace(displayName))
                                displayName = rtn;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                rtn = userId.ToString();
            }

            return rtn;
        }

        public static Attachment UndoCheckOut(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, bool childAttachmentsOn)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    var dt = Attachments.GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, Resources.GetCheckedOutToAnyoneVersion, conn);
                    var tempAttachment = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);

                    foreach (DataRow row in dt.Rows)
                        DeletePointerParts(userId, row, false, conn);

                    using (var cmd = new SqlCommand(Resources.CheckOutUndo, conn))
                    {
                        cmd.Parameters.AddWithValue("@tableName", tableName);
                        cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                        cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                        cmd.ExecuteNonQuery();
                    }

                    string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, (object)tempAttachment.AttachmentNumber, (object)(tempAttachment.VersionInfo.Version - 1));
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Undo Check Out", data, string.Empty, AuditType.AttachmentViewerActionType.UndoCheckOut, conn);
                    return GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, _thumbSize, true, "<=", childAttachmentsOn, false, false, -1, conn, false);
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
        }

        internal static SqlConnection GetConnection(string databaseName)
        {


            return null;
        }



        internal static DataTable FillDataTable(string sql, SqlConnection conn)
        {
            try
            {
                using (var da = new SqlDataAdapter(new SqlCommand(sql, conn)))
                {
                    var dt = new DataTable();
                    da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                    da.FillSchema(dt, SchemaType.Source);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        internal static DataTable FillDataTable(string sql, SqlConnection conn, string parameter, string value)
        {
            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(parameter, value);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private static string GetIdFieldName(string tableName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName))
                return string.Empty;

            try
            {
                var dt = Attachments.FillDataTable(Resources.GetIdFieldFromTable, conn, "@tableName", tableName);
                return dt.Rows[0]["IDFieldName"].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        private static bool IdFieldIsString(string tableName, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(tableName))
                return true;
            if (conn is null || conn.State != ConnectionState.Open)
                return true;

            string fieldName = GetIdFieldName(tableName, conn);
            if (string.IsNullOrEmpty(fieldName))
                return true;

            try
            {
                var dt = FillDataTable(string.Format(Resources.GetIdFieldIsString, fieldName.Replace(".", "].["), tableName), conn);
                if (dt.Columns[0].DataType.Equals(typeof(string)))
                    return true;
                if (dt.Columns[0].DataType.Equals(typeof(char)))
                    return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return true;
            }

            return false;
        }

        private static int SetUserlinkIdSize(SqlConnection conn)
        {
            if (_userLinkIndexTableIdSize > 0)
                return _userLinkIndexTableIdSize;

            try
            {
                var dt = Attachments.FillDataTable(Resources.GetUserlinkSize, conn);
                _userLinkIndexTableIdSize = dt.Columns[0].MaxLength;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _userLinkIndexTableIdSize = UserLinkIndexTableIdSize;
            }

            return _userLinkIndexTableIdSize;
        }

        internal static string GetAnnotation(AttachmentTypes attachmentType, int pageNumber, int pointerId, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetAnnotations, conn))
                {
                    string tableId = string.Format("{0}{1}{2}", ((int)attachmentType).ToString().PadLeft(2, '0'), pageNumber.ToString().PadLeft(4, '0'), pointerId.ToString().PadLeft(24, '0'));
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        internal static void SaveAnnotation(string ClientIpAddress, int userId, string tableName, string paddedTableId, AttachmentTypes attachmentType, int pageNumber, int pointerId, string data, byte[] annotation, bool createAudit, string action, AuditType.AttachmentViewerActionType actionType, SqlConnection conn)
        {
            try
            {
                string sql = Resources.UpdateAnnotation;
                string existingAnnotation = GetAnnotation(attachmentType, 1, pointerId, conn);

                if (!string.IsNullOrEmpty(existingAnnotation))
                {
                    if (annotation.Length == 0)
                    {
                        using (var cmd = new SqlCommand(Resources.DeleteAnnotations, conn))
                        {
                            string currentId = string.Format("{0}____{1}", ((int)attachmentType).ToString().PadLeft(2, '0'), pointerId.ToString().PadLeft(24, '0'));
                            cmd.Parameters.AddWithValue("@currentId", currentId);
                            cmd.ExecuteScalar();
                        }

                        if (createAudit)
                            AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, action, data, data, actionType, conn);
                        return;
                    }
                }
                else
                {
                    if (annotation.Length == 0)
                        return;
                    sql = Resources.InsertAnnotation;
                }

                string userName = GetUserName(userId, conn);

                using (var cmd = new SqlCommand(sql, conn))
                {
                    string annTableId = string.Format("{0}0001{1}", ((int)attachmentType).ToString().PadLeft(2, '0'), pointerId.ToString().PadLeft(24, '0'));
                    cmd.Parameters.AddWithValue("@tableId", annTableId);
                    cmd.Parameters.AddWithValue("@annotation", Output.ByteArrayToString(annotation));
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.ExecuteNonQuery();
                }

                if (createAudit)
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Edited Annotations", data, data, AuditType.AttachmentViewerActionType.EditAnnotations, conn);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        private static bool AuditConfidentialDataAccess(int userId, string tableName, string paddedTableId, string networkUserName, string computerName, string ClientIpAddress, string MACAddress, string action, SqlConnection conn)
        {
            string userName = GetUserName(userId, conn);
            string domainName = GetDomainName();
            if (string.IsNullOrWhiteSpace(computerName))
                computerName = GetComputerName();
            // Do NOT fill in ClientIpAddress if it's blank.  RVW 11/16/2021
            // If String.IsNullOrWhiteSpace(ClientIpAddress) Then ClientIpAddress = GetClientIpAddress()
            if (string.IsNullOrWhiteSpace(networkUserName))
                networkUserName = GetComputerUserName();

            try
            {
                using (var cmd = new SqlCommand(Resources.InsertAuditConfData, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                    cmd.Parameters.AddWithValue("@computerName", computerName);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@networkUserName", networkUserName);
                    cmd.Parameters.AddWithValue("@domain", domainName);
                    cmd.Parameters.AddWithValue("@ipAddress", ClientIpAddress);
                    cmd.Parameters.AddWithValue("@macAddress", MACAddress);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        internal static void AuditUpdateByTableId(int userId, string tableName, string tableId, string networkUserName, string computerName, string ClientIpAddress, string MACAddress, string action, string dataBefore, string dataAfter, AuditType.AttachmentViewerActionType actionType, SqlConnection conn)
        {
            string paddedTableId = PadTableId(tableName, tableId, conn);
            AuditUpdate(userId, tableName, paddedTableId, networkUserName, computerName, ClientIpAddress, MACAddress, action, dataBefore, dataAfter, actionType, conn);
        }

        internal static void AuditUpdate(int userId, string tableName, string paddedTableId, string networkUserName, string computerName, string ClientIpAddress, string MACAddress, string action, string dataBefore, string dataAfter, AuditType.AttachmentViewerActionType actionType, SqlConnection conn)
        {
            int Id;
            string userName = GetUserName(userId, conn);
            string domainName = GetDomainName();
            if (string.IsNullOrWhiteSpace(computerName))
                computerName = GetComputerName();
            // Do NOT fill in ClientIpAddress if it's blank.  RVW 11/16/2021
            // If String.IsNullOrWhiteSpace(ClientIpAddress) Then ClientIpAddress = GetClientIpAddress()
            if (string.IsNullOrWhiteSpace(networkUserName))
                networkUserName = GetComputerUserName();

            try
            {
                using (var cmd = new SqlCommand(Resources.InsertAuditUpdates, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                    cmd.Parameters.AddWithValue("@computerName", computerName);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@networkUserName", networkUserName);
                    cmd.Parameters.AddWithValue("@domain", domainName);
                    cmd.Parameters.AddWithValue("@ipAddress", ClientIpAddress);
                    cmd.Parameters.AddWithValue("@macAddress", MACAddress);
                    cmd.Parameters.AddWithValue("@module", "Attachment Viewer");
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@dataBefore", dataBefore);
                    cmd.Parameters.AddWithValue("@dataAfter", dataAfter);
                    cmd.Parameters.AddWithValue("@actionType", (int)actionType);
                    Id = Convert.ToInt32("0" + cmd.ExecuteScalar().ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Id = 0;
            }

            if (Id != 0)
                AuditUpdateChildren(Id, tableName, paddedTableId, conn);
        }

        private static void AuditUpdateChildren(int auditUpdateId, string tableName, string paddedTableId, SqlConnection conn)
        {
            InsertAuditUpdateChildren(auditUpdateId, tableName, paddedTableId, conn);

            SqlCommand cmd;
            var dt = WalkUpRelationshipsForAuditUpdates(tableName, conn);
            if (dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string sql = string.Format("SELECT {0} FROM {1} WHERE {2} = {3}", row["LowerTableFieldName"].ToString(), tableName, row["IdFieldName"].ToString(), paddedTableId);

                cmd = new SqlCommand(sql, conn);

                try
                {
                    paddedTableId = cmd.ExecuteScalar().ToString();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    paddedTableId = string.Empty;
                }

                if (!string.IsNullOrEmpty(paddedTableId))
                {
                    tableName = row["UpperTableName"].ToString();
                    paddedTableId = PadTableId(tableName, paddedTableId, conn);
                    AuditUpdateChildren(auditUpdateId, tableName, paddedTableId, conn);
                }
            }
        }

        private static DataTable WalkUpRelationshipsForAuditUpdates(string tableName, SqlConnection conn)
        {
            try
            {
                var cmd = new SqlCommand(Resources.WalkUpRelationshipsForAuditUpdates, conn);
                cmd.Parameters.AddWithValue("@tableName", tableName);

                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new DataTable();
            }
        }

        private static void InsertAuditUpdateChildren(int auditUpdateId, string tableName, string paddedTableId, SqlConnection conn)
        {
            var cmd = new SqlCommand(Resources.InsertAuditUpdateChildren, conn);
            cmd.Parameters.AddWithValue("@auditUpdateId", auditUpdateId);
            cmd.Parameters.AddWithValue("@tableName", tableName);
            cmd.Parameters.AddWithValue("@tableId", paddedTableId);
            cmd.ExecuteNonQuery();
        }

        private static string GetDomainName()
        {
            return Environment.UserDomainName;
        }

        private static string GetComputerName()
        {
            return "Web access";
        }

        private static string GetComputerUserName()
        {
            return "web access";
        }

        internal static bool IsLeadtools14()
        {
            if (_leadToolsMajorVersion > 0)
                return _leadToolsMajorVersion <= Leadtools14;

            try
            {
                _leadToolsMajorVersion = Assembly.ReflectionOnlyLoad("Leadtools").GetName().Version.Major;
                return _leadToolsMajorVersion <= Leadtools14;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _leadToolsMajorVersion = -1;
            }

            return false;
        }

        internal static void SetupCodec()
        {

            //SlimShared.AppName = "TAB FusionRMS Image Service";
            // RasterSupport.SetLicense(Resources.LEADTOOLSLICENSE, Resources.LeadToolsKey);
            // RasterSupport.SetLicense("D:\Codes\FS11.1\TabFusionRMS.WebCS\LEADTOOLS\LICENSE.lic", "jcwLXo5T0paqbVvDbtFCk5KRcsASLmsI5d3u3oZp7DM=")
        }

        private static void UpdateOrphanTrackableRecord(int oldTrackableID, int newTrackableID, int newVersionNumber, SqlConnection conn)
        {
            string sql = Resources.UpdateOrphanTrackableRecord;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@oldTrackableID", oldTrackableID);
                cmd.Parameters.AddWithValue("@newTrackableID", newTrackableID);
                cmd.Parameters.AddWithValue("@newVersion", newVersionNumber);
                cmd.ExecuteNonQuery();
            }
        }

        private static void UpdateOrphanUserLinkRecord(int trackableId, string tableName, string recordId, int attachmentNumber, SqlConnection conn)
        {
            string sql = Resources.UpdateOrphanUserLinkRecord;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@trackableId", trackableId);
                cmd.Parameters.AddWithValue("@recordId", recordId);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.ExecuteNonQuery();
            }
        }

        private static void UpdateOrphanUserLinkAndTrackableRecord(int trackableId, string tableName, string recordId, int attachmentNumber, SqlConnection conn)
        {
            string sql = Resources.UpdateOrphanUserLinkAndTrackableRecord;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@trackableId", trackableId);
                cmd.Parameters.AddWithValue("@recordId", recordId);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.ExecuteNonQuery();
            }
        }

        private static void UpdatePointerRecordsWithNewTrackable(int oldTrackableID, int newTrackableID, int versionNumber, SqlConnection conn)
        {
            string sql = Resources.UpdatePointerRecordsWithNewTrackable;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@oldTrackableID", oldTrackableID);
                cmd.Parameters.AddWithValue("@newTrackableID", newTrackableID);
                cmd.Parameters.AddWithValue("@newVersion", versionNumber + 1);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool LinkOrphanAttachmnet(int trackableId, string tableName, int recordId, Passport pass, int userId)
        {
            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {

                    string paddedTableId = PadTableId(tableName, recordId.ToString(), conn);
                    int attachmentNumber = 0;

                    string sql = Resources.GetFirstAttachment;
                    if (attachmentNumber == 0)
                        sql = Resources.GetLastAttachment;
                    var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);

                    if (dt.Rows.Count == 0)
                    {
                        attachmentNumber = 1;
                    }
                    else
                    {
                        attachmentNumber = SafeInt(dt.Rows[0], "AttachmentNumber", tableName, paddedTableId, conn) + 1;
                    }

                    UpdateOrphanUserLinkAndTrackableRecord(trackableId, tableName, paddedTableId, attachmentNumber, conn);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

        }

        public static bool RenameAttachment(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, string pointerId, bool isAnImage, string newAttachmentName)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                throw new Exception(Permissions.ExceptionString.NotAuthorized);
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(pointerId) || !Information.IsNumeric(pointerId))
                throw new Exception(Permissions.ExceptionString.InvalidParameters);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    conn.Open();
                    int attachmentType = 5;
                    if (isAnImage)
                        attachmentType = 1;

                    var dt = new DataTable();

                    using (var cmd = new SqlCommand(Resources.GetFullPathByPointerId, conn))
                    {
                        cmd.Parameters.AddWithValue("@pointerId", Convert.ToInt32(pointerId));
                        cmd.Parameters.AddWithValue("@attachmentType", attachmentType);
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }

                    if (dt.Rows.Count == 0)
                        throw new Exception(string.Format(Permissions.ExceptionString.DoesNotExist, "attachment"));
                    string oldAttachmentName = "Old Attachment Name: ";

                    if (!string.IsNullOrEmpty(dt.Rows[0]["OrgFileName"].ToString()))
                    {
                        oldAttachmentName += dt.Rows[0]["OrgFileName"].ToString();
                    }
                    else if (!string.IsNullOrEmpty(dt.Rows[0]["OrgFullPath"].ToString()))
                    {
                        oldAttachmentName += Path.GetFileName(dt.Rows[0]["OrgFullPath"].ToString());
                    }

                    using (var cmd = new SqlCommand(Resources.RenamePointerOrgFileNameById, conn))
                    {
                        cmd.Parameters.AddWithValue("@attachmentType", attachmentType);

                        if (string.IsNullOrEmpty(newAttachmentName))
                        {
                            cmd.Parameters.AddWithValue("@orgFileName", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@orgFileName", newAttachmentName);
                        }

                        cmd.Parameters.AddWithValue("@pointerId", Convert.ToInt32(pointerId));
                        cmd.ExecuteNonQuery();
                    }

                    newAttachmentName = "New Attachment Name: " + newAttachmentName;
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, string.Format("Renamed Attachment: {0}", attachmentNumber), oldAttachmentName, newAttachmentName, AuditType.AttachmentViewerActionType.RenameAttachment, conn);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private static int GetImagePointerIdByAttachment(string tableName, string tableId, int attachmentNumber, int pageNumber, SqlConnection conn)
        {
            try
            {
                var cmd = new SqlCommand(Resources.GetImagePointerIdByAttachment, conn);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", tableId);
                cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static int GetImagePointerIdForOrphan(int trackableID, SqlConnection conn)
        {
            try
            {
                var cmd = new SqlCommand(Resources.GetImagePointerIdForOrphan, conn);
                cmd.Parameters.AddWithValue("@trackablesId", trackableID);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static int GetLastVersionForTrackable(int trackableID, SqlConnection conn)
        {
            try
            {
                var cmd = new SqlCommand(Resources.GetLastVersionNumberForTrackable, conn);
                cmd.Parameters.AddWithValue("@trackablesId", trackableID);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 1;
            }
        }

        private static string GetExtensionFromImagePointer(int pointerID, SqlConnection conn)
        {
            try
            {
                var cmd = new SqlCommand(Resources.GetImagePointerFilename, conn);
                cmd.Parameters.AddWithValue("@pointerId", pointerID);
                string fileName = cmd.ExecuteScalar().ToString();
                if (!fileName.Contains("."))
                    return string.Empty;
                return fileName.Substring(fileName.IndexOf(".") + 1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return string.Empty;
            }
        }

        //public static UploadResponse UploadStream(FileUpload message)
        //{
        //    if (!Encrypt.ValidateTicket(message.Metadata.Ticket, message.Metadata.UserId, message.Metadata.DatabaseName, message.Metadata.TableName, message.Metadata.TableId))
        //        throw new Exception(Permissions.ExceptionString.NotAuthorized);

        //    var response = new UploadResponse();
        //    response.TempFileName = Attachments.SaveStreamToFile(message.UpStream);
        //    message.UpStream.Close();
        //    if (string.IsNullOrEmpty(response.TempFileName) || !File.Exists(response.TempFileName))
        //        throw new Exception(Permissions.ExceptionString.StreamUploadFailed);
        //    return response;
        //}

        public static string SaveStreamToFile(Stream stream)
        {
            try
            {
                string filePath = Path.GetTempFileName();
                Exporter.DeleteFile(filePath);

                using (var outstream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
                {
                    CopyStream(stream, outstream);
                    outstream.Flush();
                }

                return filePath;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Attachments.SaveStreamToFile", ex.Message));
                return string.Empty;
            }
        }

        private static void CopyStream(Stream instream, FileStream outstream)
        {
            // read from the input stream in 64K chunks and save to output stream
            const int bufferLen = 65536;
            var buffer = new byte[65537];
            int count = instream.Read(buffer, 0, bufferLen);

            while (count > 0)
            {
                outstream.Write(buffer, 0, count);
                count = instream.Read(buffer, 0, bufferLen);
            }
        }

        public static MemoryStream CopyStream(Stream instream)
        {
            // read from the input stream in 64K chunks and save to output stream
            const int bufferLen = 65536;
            var buffer = new byte[65537];
            instream.Seek(0L, SeekOrigin.Begin);
            int count = instream.Read(buffer, 0, bufferLen);
            var outstream = new MemoryStream();

            while (count > 0)
            {
                outstream.Write(buffer, 0, count);
                count = instream.Read(buffer, 0, bufferLen);
            }

            return outstream;
        }

        internal static int SafeInt(DataRow row, string fieldName)
        {
            if (row is null)
                return 0;

            try
            {
                if (row[fieldName] is DBNull)
                    return 0;
                return Convert.ToInt32(row[fieldName]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        internal static int SafeInt(DataRow row, string fieldName, string tableName, string paddedTableId, SqlConnection conn)
        {
            int rtn = SafeInt(row, fieldName);
            if (rtn > 0)
                return rtn;

            if (string.Compare(fieldName, "attachmentNumber", true) == 0)
                return UpdateUserlinkAttachmentNumber(tableName, paddedTableId, conn);
            return rtn;
        }

        private static int UpdateUserlinkAttachmentNumber(string tableName, string paddedTableId, SqlConnection conn)
        {
            try
            {
                var cmd = new SqlCommand(Resources.UpdateUserlinkAttachmentNumber, conn);
                cmd.Parameters.AddWithValue("@tableName", tableName);
                cmd.Parameters.AddWithValue("@tableId", paddedTableId);
                cmd.ExecuteNonQuery();
                return 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        private static int CollectionsAdd(int userId, SqlConnection conn)
        {
            using (var cmd = new SqlCommand())
            {
                int rtn = 0;
                cmd.Connection = conn;
                cmd.CommandText = Resources.GetCollection;
                cmd.Parameters.AddWithValue("@userId", userId);

                try
                {
                    rtn = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    rtn = 0;
                }

                if (rtn != 0)
                    return rtn;
                cmd.CommandText = Resources.InsertCollection;

                try
                {
                    rtn = Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    rtn = 0;
                }

                return rtn;
            }
        }

        #region Import Attachment

        public static Attachment AddAttachmentForImport(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, string outputSettingName, string fileName, string originalFullPath, string extension, bool childAttachmentsOn, string renameFileName, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    long currentFileSize = new FileInfo(fileName).Length;
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    var outputSettingTable = GetOutputSetting(outputSettingName, userId, currentFileSize, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, currentFileSize, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingOutputSetting), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    string sql = Resources.GetFirstAttachment;
                    if (attachmentNumber == 0)
                        sql = Resources.GetLastAttachment;
                    var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);

                    if (dt.Rows.Count == 0)
                    {
                        attachmentNumber = 1;
                    }
                    else
                    {
                        attachmentNumber = SafeInt(dt.Rows[0], "AttachmentNumber", tableName, paddedTableId, conn) + 1;
                    }

                    var update = new UpdateAttachment(outputSettingTable.Rows[0], GetUserName(userId, conn), extension, conn);
                    update.CreateAttachmentRecord(tableName, paddedTableId, attachmentNumber, 1, false, 0, fileName, originalFullPath, renameFileName, IsAnImage, totalPages, height, width, sizeOnDisk);

                    if (TableHasOfficialRecordEnabled(tableName, conn))
                        MarkOfficialRecord(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, 1, conn);

                    string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, attachmentNumber, 1);
                    if (!string.IsNullOrEmpty(renameFileName))
                        data = string.Format("{0}{1}New Attachment Name: {2}", Constants.vbCrLf, data, renameFileName);
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Added Attachment", string.Empty, data, AuditType.AttachmentViewerActionType.AddAttachment, conn);

                    sql = string.Format(GetSql(userId, tableName, paddedTableId, 0, 0, conn), "=", "ASC");
                    dt = GetRowsOfAttachments(userId, tableName, paddedTableId, 0, 0, sql, conn);
                    if (dt.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    string fieldName = "AttachmentNumber";
                    attachmentNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);
                    var rtn = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
                    if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
                    if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
                    return rtn;
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
            finally
            {
                // Exporter.DeleteFile(fileName)
            }
        }

        public static Attachment AddVersionForImport(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, string outputSettingName, string fileName, string originalFullPath, bool AsOfficialRecord, string extension, bool childAttachmentsOn, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.NotAuthorized), userId, pass.ServerAndDatabaseName, tableName, tableId);
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingParameters), userId, pass.ServerAndDatabaseName, tableName, tableId);

            try
            {
                using (var conn = new SqlConnection(pass.ConnectionString))
                {
                    string paddedTableId = PadTableId(tableName, tableId, conn);
                    if (IsCheckedOut(tableName, paddedTableId, attachmentNumber, conn) > 0)
                        return new ErrorAttachment(new Exception(string.Format(Permissions.ExceptionString.CannotAddWhenCheckedOut, "Versions")), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    long fileLength = new FileInfo(fileName).Length;
                    var outputSettingTable = GetOutputSetting(outputSettingName, userId, fileLength, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, fileLength, conn);
                    if (outputSettingTable.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.MissingOutputSetting), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    var dt = Attachments.GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, Resources.GetAllVersions, conn);
                    if (dt.Rows.Count == 0)
                        return AddAttachment(ClientIpAddress, Ticket, userId, pass, tableName, tableId, attachmentNumber, outputSettingName, fileName, originalFullPath, extension, childAttachmentsOn, string.Empty);

                    // Added by akruti to fix the issue FUS-4577
                    string attachmentName = dt.Rows[0]["OrgFileName"].ToString();

                    versionNumber = Convert.ToInt32(dt.Rows[0]["RecordVersion"]) + 1;
                    var update = new UpdateAttachment(outputSettingTable.Rows[0], GetUserName(userId, conn), extension, conn);
                    update.CreateAttachmentRecord(tableName, paddedTableId, attachmentNumber, versionNumber, AsOfficialRecord, Convert.ToInt32(dt.Rows[0]["TrackablesID"]), fileName, originalFullPath, attachmentName, IsAnImage, totalPages, height, width, sizeOnDisk);
                    if (AsOfficialRecord)
                        MarkOfficialRecord(ClientIpAddress, Ticket, userId, pass.ServerAndDatabaseName, tableName, paddedTableId, attachmentNumber, versionNumber, conn);

                    string data = string.Format("Attachment: {1}{0}Version: {2}", Constants.vbCrLf, attachmentNumber, versionNumber);
                    Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Added Version", string.Empty, data, AuditType.AttachmentViewerActionType.AddVersion, conn);

                    string sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, 0, conn), "=", "ASC");
                    dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, sql, conn);
                    if (dt.Rows.Count == 0)
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, new SecurityInfo(tableName, userId, conn), conn);

                    string fieldName = "AttachmentNumber";
                    attachmentNumber = SafeInt(dt.Rows[0], fieldName, tableName, paddedTableId, conn);
                    var rtn = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
                    if (!Attachments.AttachmentPermission(rtn, Permissions.Attachment.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);
                    if (!Attachments.VolumePermission(rtn, Permissions.Volume.View))
                        return new ErrorAttachment(new Exception(Permissions.ExceptionString.NoRecords), userId, pass.ServerAndDatabaseName, tableName, tableId, rtn.SecurityInfo, conn);

                    return rtn;
                }
            }
            catch (Exception ex)
            {
                return new ErrorAttachment(ex, userId, pass.ServerAndDatabaseName, tableName, tableId);
            }
            finally
            {
                // Exporter.DeleteFile(fileName)
            }
        }

        public static string AddPageForImport(string ClientIpAddress, string Ticket, int userId, Passport pass, string tableName, string tableId, int attachmentNumber, int versionNumber, string outputSettingName, string fileName, string originalFullPath, string extension, bool childAttachmentsOn, bool forceCheckout, bool keepOfficialRecord, bool returnSuccess, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            if (!Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                return Permissions.ExceptionString.NotAuthorized;
            if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                return Permissions.ExceptionString.MissingParameters;
            bool validated = true;

            try
            {
                if (!validated && !Encrypt.ValidateTicket(Ticket, userId, pass.ServerAndDatabaseName, tableName, tableId))
                    return Permissions.ExceptionString.NotAuthorized;
                if (string.IsNullOrEmpty(tableName) | string.IsNullOrEmpty(tableId))
                    return Permissions.ExceptionString.MissingParameters;

                string rtn = string.Empty;
                if (returnSuccess)
                    rtn = Permissions.ExceptionString.Successful;

                try
                {
                    using (var conn = new SqlConnection(pass.ConnectionString))
                    {
                        string paddedTableId = PadTableId(tableName, tableId, conn);

                        if (!forceCheckout)
                        {
                            if (keepOfficialRecord)
                            {
                                if (IsCheckedOut(tableName, paddedTableId, attachmentNumber, conn) != 0)
                                    return string.Format(Permissions.ExceptionString.CannotAddWhenCheckedOut, "Pages");
                            }
                            else if (IsCheckedOutToMe(userId, tableName, paddedTableId, attachmentNumber, conn) == 0)
                                return string.Format(Permissions.ExceptionString.CanOnlyAddWhenCheckedOutToMe, "Pages");
                        }

                        long fileLength = new FileInfo(fileName).Length;
                        var outputSettingTable = GetOutputSetting(outputSettingName, userId, fileLength, conn);
                        if (outputSettingTable.Rows.Count == 0)
                            outputSettingTable = GetOutputSetting(GetDefaultOutputSetting(conn), userId, fileLength, conn);
                        if (outputSettingTable.Rows.Count == 0)
                            return Permissions.ExceptionString.MissingOutputSetting;

                        string sql = string.Format(GetSql(userId, tableName, paddedTableId, attachmentNumber, versionNumber, conn), "=", "ASC");
                        var dt = GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, versionNumber, sql, conn);
                        if (dt.Rows.Count == 0)
                        {
                            Attachment attachment;
                            attachment = AddAnAttachment(ClientIpAddress, Ticket, userId, pass, tableName, tableId, 0, outputSettingName, fileName, originalFullPath, extension, childAttachmentsOn, string.Empty, IsAnImage, totalPages, height, width, sizeOnDisk);
                            if (attachment is ErrorAttachment)
                                return ((ErrorAttachment)attachment).Message;
                            return rtn;
                        }

                        Attachment workingAttachment = null;

                        if (forceCheckout)
                        {
                            if (IsCheckedOut(tableName, paddedTableId, attachmentNumber, conn) != 0)
                                return string.Format(Permissions.ExceptionString.CannotAddWhenCheckedOut, "Pages");
                            workingAttachment = CheckOut(Ticket, userId, pass, tableName, tableId, attachmentNumber, versionNumber, string.Empty, string.Empty, string.Empty, childAttachmentsOn);
                            if (workingAttachment is ErrorAttachment)
                                return ((ErrorAttachment)workingAttachment).Message;
                            dt = Attachments.GetRowsOfAttachments(userId, tableName, paddedTableId, attachmentNumber, 0, Resources.GetCheckedOutVersion, conn);
                            if (keepOfficialRecord && dt.Rows.Count > 0)
                                keepOfficialRecord = Convert.ToBoolean(dt.Rows[0]["OfficialRecord"]);
                        }
                        else if (keepOfficialRecord)
                        {
                            workingAttachment = GetAttachment(ClientIpAddress, userId, pass, tableName, tableId, attachmentNumber, versionNumber, _thumbSize, true, childAttachmentsOn, true, false, -1);
                        }
                        else
                        {
                            workingAttachment = GetValidAttachment(userId, pass.ServerAndDatabaseName, tableName, paddedTableId, (AttachmentTypes)dt.Rows[0]["RecordType"], attachmentNumber, dt.Rows[0], conn);
                        }
                        int pageCount = workingAttachment.get_PageCount(versionNumber, tableName, tableId);
                        if (pageCount <= 0)
                            pageCount = dt.Rows.Count;
                        // keepOfficialRecord serves double duty; in the following line it actually means "attach to the existing version".
                        int pages = workingAttachment.AddAPages(fileName, originalFullPath, GetUserName(userId, conn), extension, outputSettingTable.Rows[0], pageCount, !forceCheckout & keepOfficialRecord, conn, IsAnImage, totalPages, height, width, sizeOnDisk);

                        if (pages > 0)
                        {
                            string data = string.Format("Attachment: {1}{0}Version: {2}{0}Pages added: {3}", Constants.vbCrLf, (object)workingAttachment.AttachmentNumber, (object)workingAttachment.VersionInfo.Version, pages);
                            Attachments.AuditUpdate(userId, tableName, paddedTableId, string.Empty, string.Empty, ClientIpAddress, string.Empty, "Added page(s)", string.Empty, data, AuditType.AttachmentViewerActionType.AddPage, conn);
                        }

                        if (forceCheckout)
                            workingAttachment = Attachments.CheckIn(ClientIpAddress, Ticket, userId, pass, tableName, tableId, attachmentNumber, workingAttachment.VersionInfo.Version, outputSettingName, keepOfficialRecord, extension, true, fileName, childAttachmentsOn);
                        if (workingAttachment is ErrorAttachment)
                            return ((ErrorAttachment)workingAttachment).Message;
                        return rtn;
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                // Exporter.DeleteFile(fileName)
            }
        }

        public static int IsCheckedOutPublic(string tableName, string tableId, int attachmentNumber, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsCheckedOut, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }

        public static int IsCheckedOutToMePublic(int userId, string tableName, string tableId, int attachmentNumber, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.IsCheckedOutToMe, conn))
                {
                    cmd.Parameters.AddWithValue("@tableName", tableName);
                    cmd.Parameters.AddWithValue("@tableId", tableId);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@attachmentNumber", attachmentNumber);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }


        #endregion
    }
}