using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
//using Exceptions = Smead.RecordsManagement.Imaging.Permissions.ExceptionString;

namespace MSRecordsEngine.Imaging
{
    [Serializable()]
    public partial class ErrorAttachment : Attachment
    {

        private ErrorAttachment() : base()
        {
            _hasErrors = true;
        }

        public ErrorAttachment(Exception ex, int userId, string databaseName, string tableName, string tableId) : this()
        {
            _userID = userId;
            _databaseName = databaseName;
            _tableName = tableName;
            _tableId = tableId;
            if (ex is null)
                return;

            if (!string.IsNullOrEmpty(ex.Message))
                _message = ex.Message;
            if (!string.IsNullOrEmpty(ex.StackTrace))
                _stackTrace = ex.StackTrace;
            if (ex.InnerException is not null)
                _innerException = ex.InnerException.Message;
        }

        public ErrorAttachment(Exception ex, int userId, string databaseName, string tableName, string tableId, SecurityInfo security, SqlConnection conn) : this(ex, userId, databaseName, tableName, tableId)
        {
            this.Security = security;
            if (ex is null || string.Compare(ex.Message, Permissions.ExceptionString.NoRecords) != 0)
                return;
            if (conn is null || conn.State != ConnectionState.Open)
                return;

            using (var cmd = new SqlCommand("SELECT TOP 1 RenameOnScan FROM System", conn))
            {
                try
                {
                    _renameOnScan = Conversions.ToBoolean(cmd.ExecuteScalar());
                }
                catch (Exception localex)
                {
                    Debug.WriteLine(localex.Message);
                    _renameOnScan = false;
                }
            }
        }

        public bool HasErrors
        {
            get
            {
                return _hasErrors;
            }
        }
        private bool _hasErrors = false;

        public string Message
        {
            get
            {
                return _message;
            }
        }
        private string _message = string.Empty;

        public string InnerException
        {
            get
            {
                return _innerException;
            }
        }
        private string _innerException = string.Empty;

        public string StackTrace
        {
            get
            {
                return _stackTrace;
            }
        }
        private string _stackTrace = string.Empty;

        internal override Attachments.AttachmentTypes AttachmentType
        {
            get
            {
                return Attachments.AttachmentTypes.tkNone;
            }
        }

        internal override Page AddNewPage(int index)
        {
            throw new Exception("Pages are not allowed on an ErrorAttachment");
        }

        internal override Thumb Thumbs(int index)
        {
            throw new Exception("Thumbnails are not allowed on an ErrorAttachment");
        }

        internal override int CopyRecord(string currentId, int pageNumber, int versionNumber, string newImageTableId, SqlConnection conn)
        {
            return 0;
        }

        internal override void BurstMultiPage(System.Data.DataTable dt, int versionNumber, SqlConnection conn, int totalPages)
        {
            // do nothing
        }

        internal override void BurstMultiPage(System.Data.DataTable dt, int versionNumber, SqlConnection conn)
        {
            // do nothing
        }

        internal override string CopyFileWithDotPrefix(string imageTableName, string fullPath, SqlConnection conn)
        {
            return string.Empty;
        }
    }
}
