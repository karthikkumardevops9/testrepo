using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Leadtools.Codecs;
using Microsoft.VisualBasic;
using MSRecordsEngine.Properties; // Install-Package Microsoft.VisualBasic
namespace MSRecordsEngine.Imaging
{
    [Serializable()]
    public partial class AttachmentPart
    {
        internal AttachmentPart()
        {
            // 
        }

        public AttachmentPart(DataRow row)
        {
            _fileName = row["FileName"].ToString();
            _imageTableName = row["ImageTableName"].ToString();
            _tableName = row["IndexTable"].ToString();
            _tableID = row["IndexTableID"].ToString();
        }

        public AttachmentPart(Attachment attachment, int groupingNumber, bool isAnImage, DataRow row, bool pageThumbnail, SqlConnection conn)
        {
            _groupingNumber = groupingNumber;
            _fileName = row["FileName"].ToString();
            _imageTableName = row["ImageTableName"].ToString();
            _tableName = row["IndexTable"].ToString();
            _tableID = row["IndexTableID"].ToString();

            string orgFullPath = row["OrgFullPath"].ToString();
            if (Information.IsNumeric(_fileName) && !string.IsNullOrEmpty(orgFullPath))
            {
                if (!orgFullPath.Contains(@"\") && !orgFullPath.Contains(Path.DirectorySeparatorChar))
                    _imageTableName = row["OrgFullPath"].ToString();
            }

            _fullPath = row["FullPath"].ToString();
            _pageCount = 1;
            if (Attachments.IsLeadtools14())
                _pageCount = GetPageCount(attachment.DatabaseName, _imageTableName, _fullPath, isAnImage, conn);
            if (!(row["ScanDirectoriesId"] is DBNull))
                _directoryId = Convert.ToInt32("0" + row["ScanDirectoriesId"].ToString());

            if (isAnImage)
            {
                for (int i = 1, loopTo = _pageCount; i <= loopTo; i++)
                    attachment.PagesList.Add(new ImagePage());
            }
        }
        /// <summary>
        /// AttachmentNumber or VersionNumber
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GroupingNumber
        {
            get
            {
                return _groupingNumber;
            }
        }
        private int _groupingNumber;

        public int DirectoryId
        {
            get
            {
                return _directoryId;
            }
        }
        private int _directoryId = 0;

        public int PageCount
        {
            get
            {
                return _pageCount;
            }
        }
        private int _pageCount;

        public string FileName
        {
            get
            {
                return _fileName;
            }
        }
        private string _fileName = string.Empty;

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
        }
        private string _fullPath = string.Empty;

        public string ImageTableName
        {
            get
            {
                return _imageTableName;
            }
        }
        private string _imageTableName = string.Empty;

        public string TableName
        {
            get
            {
                return _tableName;
            }
        }
        private string _tableName = string.Empty;

        public string TableID
        {
            get
            {
                return _tableID;
            }
        }
        private string _tableID = string.Empty;

        internal void ResetPageCount(int newCount)
        {
            _pageCount = newCount;
        }

        internal void ResetPageCount(Attachment attachment, string databaseName, string imageTableName, string fullPath, Attachments.AttachmentTypes attachmentType, SqlConnection conn)
        {
            int count = GetPageCount(databaseName, imageTableName, fullPath, attachmentType != Attachments.AttachmentTypes.tkWPDoc, conn);

            if (attachmentType != Attachments.AttachmentTypes.tkWPDoc)
            {
                for (int i = 0, loopTo = count - 1; i <= loopTo; i++)
                    attachment.PagesList.Add(new ImagePage());
            }
            _pageCount += count;
        }

        internal int GetPageCount(string databaseName, string imageTableName, string fullPath, bool isAnImage, SqlConnection conn)
        {
            if (isAnImage)
            {
                if (!string.IsNullOrEmpty(imageTableName))
                    return GetDatabaseImagePageCount(databaseName, conn);

                try
                {
                    var codec = new Leadtools.Codecs.RasterCodecs();
                    CodecsImageInfo info = codec.GetInformation(fullPath, true);
                    return info.TotalPages;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }

            return 1;
        }

        private int GetDatabaseImagePageCount(string databaseName, SqlConnection conn)
        {
            if (!Information.IsNumeric(FileName))
                return 0;
            int rtn = 0;

            try
            {
                DataTable dt = Attachments.FillDataTable(string.Format(Resources.GetDatabaseImage, ImageTableName), conn, "@recordId", FileName);
                if (dt.Rows.Count == 0)
                    return 0;

                try
                {
                    rtn = GetDatabaseImagePageCount(dt.Rows[0]);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }

            return Math.Max(rtn, 0);
        }

        private int GetDatabaseImagePageCount(DataRow row)
        {
            try
            {
                var codec = new Leadtools.Codecs.RasterCodecs();
                using (var inputStream = new MemoryStream((byte[])row["ImageField"]))
                {
                    CodecsImageInfo info = codec.GetInformation(inputStream, true);
                    return info.TotalPages;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return 1;
            }
        }

        internal int GetPointerId(VersionInfo versionInfo, int pageNumber, string fileName, SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand(Resources.GetImagePointerId, conn))
                {
                    cmd.Parameters.AddWithValue("@trackablesId", versionInfo.TrackablesId);
                    cmd.Parameters.AddWithValue("@recordVersion", versionInfo.Version);
                    cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}

