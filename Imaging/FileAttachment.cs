using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using MSRecordsEngine.Properties;
//using Exceptions = Smead.RecordsManagement.Imaging.Permissions.ExceptionString;
namespace MSRecordsEngine.Imaging
{
    [Serializable()]
    public partial class FileAttachment : Attachment
    {

        private FileAttachment() : base()
        {
            PointerTableName = "PCFilesPointers";
        }

        public FileAttachment(int userId, string tableName, string tableId, string databaseName, System.Drawing.Size thumbSize, DataRow row, SqlConnection conn) : base(userId, tableName, tableId, databaseName, thumbSize, row, conn)
        {
            PointerTableName = "PCFilesPointers";
        }

        internal override Attachments.AttachmentTypes AttachmentType
        {
            get
            {
                return Attachments.AttachmentTypes.tkWPDoc;
            }
        }

        internal Page Pages(int index, int OcrDpiForDocuments)
        {
            try
            {
                if (index < 0)
                    index = 0;
                if (index >= PagesList.Count)
                    return AddNewPage(index, OcrDpiForDocuments);

                if (PagesList[index].PageNumber == -1)
                {
                    PagesList[index].GetPage(this, index);
                    return PagesList[index];
                }

                return AddNewPage(index);
            }
            catch (Exception ex)
            {
                if (string.Compare(ex.Message, Permissions.ExceptionString.FileNotFound) == 0)
                    throw new Exception(string.Concat(ex.Message, ": ", ex.InnerException.Message));
                throw;
            }
        }

        internal override Page AddNewPage(int index)
        {
            return AddNewPage(index, 0);
        }

        internal Page AddNewPage(int index, int OcrDpiForDocuments)
        {
            if (index < 0)
                index = 0;
            PagesList.Add(new FilePage(this, index, OcrDpiForDocuments, OutputFormat));
            if (index > PagesList.Count - 1)
                index = PagesList.Count - 1;
            return PagesList[index];
        }

        internal override Thumb Thumbs(int index)
        {
            try
            {
                if (index < 0)
                    index = 0;
                return ThumbsList[index];
            }
            catch (Exception ex)
            {
                if (PagesList.Count > 0)
                {
                    return ThumbsList[PagesList.Count - 1];
                }
                else
                {
                    //Logs.LoginWarning(string.Format("Error \"{0}\" in FileAttachment.Thumbs (index: {1})", ex.Message, index));
                    throw;
                }
            }
        }

        internal override int CopyRecord(string currentId, int newPageNumber, int newVersionNumber, string newPath, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.CopyPCFilesPointers, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", currentId);
                    cmd.Parameters.AddWithValue("@versionNumber", newVersionNumber);
                    cmd.Parameters.AddWithValue("@fileName", Path.GetFileName(newPath));
                    return Conversions.ToInteger(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                //Logs.LoginWarning(string.Format("Error \"{0}\" in FileAttachment.CopyRecord (currentId: {1}, newPageNumber: {2}, newVersionNumber: {3}, newPath: {4})", ex.Message, currentId, newPageNumber, newVersionNumber, newPath));
                return 0;
            }
        }

        internal override string CopyFileWithDotPrefix(string imageTableName, string fullPath, SqlConnection conn)
        {
            string newPath = Path.Combine(Path.GetDirectoryName(fullPath), "." + Path.GetFileName(fullPath));
            string rtn = CopyFile(imageTableName, fullPath, newPath, conn);
            if (string.IsNullOrEmpty(rtn))
                return newPath;
            return string.Empty;
        }

        internal override void BurstMultiPage(DataTable dt, int newVersionNumber, SqlConnection conn)
        {
            foreach (DataRow row in dt.Rows)
                CopyPointers(row, 1, newVersionNumber, true, conn);
        }

        internal override void BurstMultiPage(DataTable dt, int versionNumber, SqlConnection conn, int totalPages)
        {
            BurstMultiPage(dt, versionNumber, conn);
        }

    }
}
