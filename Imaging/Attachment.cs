using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.Controls;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using Microsoft.VisualBasic.CompilerServices; // Install-Package Microsoft.VisualBasic
using MSRecordsEngine.Properties;

namespace MSRecordsEngine.Imaging 
{
    [DataContract()]
    public abstract partial class Attachment : IDisposable
    {

        public static System.Drawing.Size FlyoutSize = new System.Drawing.Size(450, 590);
        protected Output.Format OutputFormat = Output.Format.Jpg;

        internal const string CachedPages = "Pages";
        internal const string CachedFlyouts = "Flyouts";
        internal const string CachedThumbs = "Thumbs";
        private const string DisplayTextDivider = "\u0001";

        public enum DisplayTypes
        {
            Undefined = -1,
            None,
            FileName,
            DisplayFields,
            FileNameIncludeInFooter,
            DisplayFieldsIncludeInFooter
        }

        [DataMember()]
        private Pages _pages;
        [DataMember()]
        private Thumbs _thumbs;
        private DateTime _redactionAddedDate = new DateTime(2013, 10, 26);
        private DateTime _scaleToGrayDate = new DateTime(2016, 2, 8);
        internal System.Drawing.Size ThumbSize;
        protected string PointerTableName;
        protected List<string> FileNames;

        internal abstract Page AddNewPage(int index);
        internal abstract Thumb Thumbs(int index);
        internal abstract Attachments.AttachmentTypes AttachmentType { get; }
        internal abstract int CopyRecord(string currentId, int pageNumber, int versionNumber, string newImageTableId,SqlConnection conn);
        internal abstract string CopyFileWithDotPrefix(string imageTableName, string fullPath,SqlConnection conn);
        internal abstract void BurstMultiPage(DataTable dt, int newVersionNumber, SqlConnection conn);
        internal abstract void BurstMultiPage(DataTable dt, int newVersionNumber, SqlConnection conn, int totalPages);

        public Attachment()
        {
            _pages = new Pages();
            _thumbs = new Thumbs();

            try
            {
                string value = ConfigurationManager.AppSettings["OutputFormat"];
                if (!string.IsNullOrEmpty(value))
                    OutputFormat = (Output.Format)Enum.Parse(typeof(Output.Format), value, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                OutputFormat = Output.Format.Jpg;
            }

            //Logs.Loginformation(string.Format("Attachment.OutputFormat: {0}", OutputFormat.ToString()));
        }

        protected Attachment(int userId, string tableName, string tableId, string databaseName, System.Drawing.Size thumbSize, DataRow row, SqlConnection conn) : this()
        {
            _tableName = tableName;
            _tableId = tableId;
            _databaseName = databaseName;
            _userID = userId;
            ThumbSize = thumbSize;

            if (row is not null)
            {
                if (row["PrintImageFooter"] is DBNull)
                {
                    _printImageFooter = false;
                }
                else
                {
                    _printImageFooter = (bool)row["PrintImageFooter"];
                }
                if (row["RenameOnScan"] is DBNull)
                {
                    _renameOnScan = false;
                }
                else
                {
                    _renameOnScan = (bool)row["RenameOnScan"];
                }

                _displayNative = GetOverrideSetting(row["FileName"].ToString(), conn);
                _tableUserName = row["TableUserName"].ToString();
                _versionInfo = new VersionInfo(row);
                _checkOutInfo = new CheckOutInfo(row);
                ScanBatch = new ScanBatchInfo(row);

                if (_versionInfo.Orphan)
                {
                    _attachmentNumber = _versionInfo.TrackablesId;
                }
                else
                {
                    _attachmentNumber = Math.Max(Attachments.SafeInt(row, "AttachmentNumber", tableName, tableId, conn), 1);
                }
                Security = new SecurityInfo(tableName, userId, Attachments.SafeInt(row, "ScanDirectoriesId"), conn);
            }
            else
            {
                _attachmentNumber = 1;
                Security = new SecurityInfo(tableName, userId, conn);
            }

            Attachments.SetupCodec();
        }

        public int AttachmentNumber
        {
            get
            {
                return _attachmentNumber;
            }
            set
            {
                _attachmentNumber = value;
            }
        }
        [DataMember()]
        private int _attachmentNumber;

        public List<AttachmentPart> AttachmentParts
        {
            get
            {
                return _attachmentParts;
            }
        }
        [DataMember()]
        private List<AttachmentPart> _attachmentParts = new List<AttachmentPart>();

        public string DatabaseName
        {
            get
            {
                return _databaseName;
            }
        }
        [DataMember()]
        protected string _databaseName;

        public int get_PageCount(int groupingNumber, string tableName, string tableId)
        {
            int count = 0;
            foreach (AttachmentPart part in AttachmentParts)
            {
                if (part.GroupingNumber == groupingNumber & string.Compare(part.TableName, tableName, true) == 0 & string.Compare(Attachments.StripLeadingZeros(part.TableID), Attachments.StripLeadingZeros(tableId), true) == 0)
                {
                    count += part.PageCount;
                }
            }
            return count;
        }

        public int get_PageCount(int versionNumber, bool versionGrouping, string tableName, string tableId)
        {
            if (versionGrouping)
                return get_PageCount(versionNumber, tableName, tableId);
            return get_PageCount(_attachmentNumber, tableName, tableId);
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }
        }
        [DataMember()]
        protected string _tableName = string.Empty;

        public string TableUserName
        {
            get
            {
                return _tableUserName;
            }
        }
        [DataMember()]
        private string _tableUserName = string.Empty;

        public string TableId
        {
            get
            {
                return _tableId;
            }
        }
        [DataMember()]
        protected string _tableId = string.Empty;

        public bool PrintImageFooter
        {
            get
            {
                return _printImageFooter;
            }
        }
        [DataMember()]
        private bool _printImageFooter = true;

        public VersionInfo VersionInfo
        {
            get
            {
                return _versionInfo;
            }
        }
        [DataMember()]
        private VersionInfo _versionInfo = new VersionInfo();

        public SecurityInfo SecurityInfo
        {
            get
            {
                return Security;
            }
        }
        [DataMember()]
        protected SecurityInfo Security = new SecurityInfo();

        public CheckOutInfo CheckOutInfo
        {
            get
            {
                return _checkOutInfo;
            }
        }
        [DataMember()]
        private CheckOutInfo _checkOutInfo = new CheckOutInfo();

        public int UserID
        {
            get
            {
                return _userID;
            }
        }
        [DataMember()]
        protected int _userID;

        public bool DisplayNative
        {
            get
            {
                return _displayNative;
            }
        }
        [DataMember()]
        private bool _displayNative;

        public DisplayTypes DisplayType
        {
            get
            {
                return _displayType;
            }
        }
        [DataMember()]
        private DisplayTypes _displayType = DisplayTypes.Undefined;

        public ScanBatchInfo ScanBatchInfo
        {
            get
            {
                return ScanBatch;
            }
        }
        [DataMember()]
        private ScanBatchInfo ScanBatch = new ScanBatchInfo();

        internal Pages PagesList
        {
            get
            {
                return _pages;
            }
        }

        public bool RenameOnScan
        {
            get
            {
                return _renameOnScan;
            }
        }
        [DataMember()]
        protected bool _renameOnScan;

        internal Page Pages(int index)
        {
            return Pages(index, false);
        }

        internal Page Pages(int index, bool throwException)
        {
            try
            {
                if (index >= PagesList.Count)
                    index = PagesList.Count - 1;
                if (index < 0)
                    index = 0;

                if (index >= PagesList.Count)
                    return AddNewPage(index);

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
                if (PagesList.Count > 0 && !throwException)
                    return PagesList[PagesList.Count - 1];
               throw new Exception(string.Format("Error \"{0}\" in Attachment.Pages (index: {1})", ex.Message, index));
                throw;
            }
        }

        internal Thumbs ThumbsList
        {
            get
            {
                return _thumbs;
            }
        }

        internal void CopyAnnotations(string currentId, int newId, int pageNumber, SqlConnection conn)
        {
            if (newId == 0 || !(this is ImageAttachment))
                return;

            try
            {
                using (var cmd = new SqlCommand(Resources.CopyAnnotations, conn))
                {
                    currentId = AttachmentType.ToString().PadLeft(2, '0') + pageNumber.ToString().PadLeft(4, '0') + currentId.PadLeft(24, '0');

                    cmd.Parameters.AddWithValue("@currentId", currentId);
                    cmd.Parameters.AddWithValue("@newId", AttachmentType.ToString().PadLeft(2, '0') + 1.ToString().PadLeft(4, '0') + newId.ToString().PadLeft(24, '0'));
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Attachment.CopyAnnotations (currentId: {1}, newId: {2}, pageNumber: {3})", ex.Message, currentId, newId, pageNumber));
            }
        }

        internal static void DeleteAnnotations(Attachments.AttachmentTypes attachmentType, string currentId, SqlConnection conn)
        {
            string noteTable = "PCFilesPointers";
            if (attachmentType != Attachments.AttachmentTypes.tkWPDoc)
                noteTable = "ImagePointers";

            try
            {
                using (var cmd = new SqlCommand(Resources.DeleteNoteByTableNameId, conn))
                {
                    cmd.Parameters.AddWithValue("@noteTable", noteTable);
                    cmd.Parameters.AddWithValue("@currentId", currentId.PadLeft(30, '0'));
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Attachment.DeleteAnnotations(Notes) (currentId: {1})", ex.Message, currentId));
            }

            if (attachmentType != Attachments.AttachmentTypes.tkImage && attachmentType != Attachments.AttachmentTypes.tkFax)
                return;

            try
            {
                using (var cmd = new SqlCommand(Resources.DeleteAnnotations, conn))
                {
                    currentId = attachmentType.ToString().PadLeft(2, '0') + new string('_', 4) + currentId.PadLeft(24, '0');
                    cmd.Parameters.AddWithValue("@currentId", currentId);
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
               throw new Exception(string.Format("Error \"{0}\" in Attachment.DeleteAnnotations (currentId: {1})", ex.Message, currentId));
            }
        }

        internal static void DeletePointer(Attachments.AttachmentTypes attachmentType, int currentId, SqlConnection conn)
        {
            if (currentId == 0)
                return;
            string tableName = "ImagePointers";
            if (attachmentType == Attachments.AttachmentTypes.tkWPDoc)
                tableName = "PCFilesPointers";

            try
            {
                using (var cmd = new SqlCommand(string.Format(Resources.DeletePointers, tableName), conn))
                {
                    cmd.Parameters.AddWithValue("@currentId", currentId);
                    cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in Attachment.DeletePointer (currentId: {1})", ex.Message, currentId));
            }
        }

        internal void AddParts(DataRowCollection rows, string fieldName, int currentNumber, string tableName, bool pageThumbnail, SqlConnection conn)
        {
            foreach (DataRow row in rows)
            {
                if (string.Compare(fieldName, "TrackablesId", true) == 0 || string.Compare(fieldName, "PointerId", true) == 0)
                {
                    AddParts(currentNumber, row, pageThumbnail, conn);
                }
                else if (Attachments.SafeInt(row, fieldName, tableName, TableId, conn) == currentNumber & string.Compare(row["IndexTable"].ToString(), tableName, true) == 0)
                    AddParts(currentNumber, row, pageThumbnail, conn);
            }
        }

        internal void AddParts(int currentNumber, DataRow row, bool pageThumbnail, SqlConnection conn)
        {
            var part = new AttachmentPart(this, currentNumber, (Attachments.AttachmentTypes)row["RecordType"] != Attachments.AttachmentTypes.tkWPDoc, row, pageThumbnail, conn);
            AttachmentParts.Add(part);
        }

        internal void AddThumbnails(int index, int startPage, int userId, int currentNumber, DataRowCollection rows, bool pageThumbnail, bool versionGrouping, int itemNumber, SqlConnection conn)
        {
            AddThumbnails(index, startPage, userId, currentNumber, rows, get_PageCount(currentNumber, rows[0]["IndexTable"].ToString(), rows[0]["IndexTableID"].ToString()), pageThumbnail, versionGrouping, itemNumber, conn);
        }

        internal void AddThumbnails(int index, int startPage, int userId, int currentNumber, DataRowCollection rows, int count, bool pageThumbnail, bool versionGrouping, int itemNumber, SqlConnection conn)
        {
            
            if ((Attachments.AttachmentTypes)rows[index]["RecordType"] == Attachments.AttachmentTypes.tkWPDoc)
            {
                AddFileThumbs(index, startPage, currentNumber, (int)rows[index]["RecordVersion"], count, pageThumbnail, userId, rows, versionGrouping, itemNumber, rows[index]["IndexTable"].ToString(), rows[index]["IndexTableID"].ToString(), rows[index]["TableUserName"].ToString(), conn);
            }
            else
            {
                AddImageThumbs(index, startPage, currentNumber, (int)rows[index]["RecordVersion"], count, pageThumbnail, userId, rows, versionGrouping, itemNumber, rows[index]["IndexTable"].ToString(), rows[index]["IndexTableID"].ToString(), rows[index]["TableUserName"].ToString(), conn);
            }
            //Logs.Loginformation(string.Format("Finished AddThumbnails - Table: {0}, TableId: {1}, Version: {2} - Type: {3}", rows[index]["IndexTable"].ToString(), rows[index]["IndexTableID"].ToString(), rows[index]["RecordVersion"].ToString(), rows[index]["RecordType"].ToString()));
        }

        public void CopyPointers(DataRow row, int pageNumber, int versionNumber, bool wasMultiPage, SqlConnection conn)
        {
            string currentId = row["PointerId"].ToString();
            string imageTableName = row["ImageTableName"].ToString();
            string fullPath = row["FullPath"].ToString();
            string fileName = row["FileName"].ToString();

            if (!string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
            {
                if (!string.IsNullOrEmpty(fileName) && Information.IsNumeric(fileName))
                {
                    imageTableName = row["OrgFullPath"].ToString();
                    fullPath = row["FileName"].ToString();
                }
            }

            string newImageTableId = CopyFileWithDotPrefix(imageTableName, fullPath, conn);
            int newId = CopyRecord(currentId, pageNumber, versionNumber, newImageTableId, conn);
            int currentPageNumber = 1;
            if (wasMultiPage)
                currentPageNumber = (int)row["PageNumber"];
            CopyAnnotations(currentId, newId, currentPageNumber, conn);
            //Logs.Loginformation(string.Format("Finished Attachment.CopyPointers - Table: {0}, TableId: {1}, Current Id: {2}, NewId: {3}", row["IndexTable"].ToString(), row["IndexTableID"].ToString(), currentId, newId));
        }

        internal static string CopyFile(string imageTableName, string fullPath, string newPath, SqlConnection conn)
        {
            if (!string.IsNullOrEmpty(imageTableName))
                return CopyBlobImage(imageTableName, fullPath, conn);

            DeleteFile(imageTableName, newPath, conn);

            try
            {
                File.Copy(fullPath, newPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: \"{0}\" in Attachment.CopyFile (fullPath: {1}, newPath: {2})", ex.Message, fullPath, newPath));
               
            }

            return string.Empty;
        }

        internal static string CopyBlobImage(string imageTableName, string recordId, SqlConnection conn)
        {
            string fileExtension = string.Empty;
            int trackableType = 0;
            byte[] byteArray = null;

            try
            {
                DataTable dt = Attachments.FillDataTable(string.Format(Resources.GetDatabaseImage, imageTableName), conn, "@recordId", recordId);
                if (dt.Rows.Count == 0)
                    return string.Empty;

                fileExtension = dt.Rows[0]["FileExtension"].ToString();
                trackableType = (int)dt.Rows[0]["TrackableType"];
                byteArray = (byte[])dt.Rows[0]["ImageField"];

                return UpdateAttachment.InsertImageTableRecord(imageTableName, fileExtension, trackableType, byteArray, conn);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: \"{0}\" in Attachment.CopyBlobImage (imageTableName: {1}, recordId: {2})", ex.Message, imageTableName, recordId));
                Debug.Print(ex.Message);
                return string.Empty;
            }
        }

        internal static byte[] BlobImageToStream(string imageTableName, string recordId, SqlConnection conn)
        {
            string fileExtension = string.Empty;
            int trackableType = 0;

            try
            {
                DataTable dt = Attachments.FillDataTable(string.Format(Resources.GetDatabaseImage, imageTableName), conn, "@recordId", recordId);
                if (dt.Rows.Count == 0)
                    return null;

                fileExtension = dt.Rows[0]["FileExtension"].ToString();
                trackableType = (int)dt.Rows[0]["TrackableType"];
                return (byte[])dt.Rows[0]["ImageField"];
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: \"{0}\" in Attachment.BlobImageToStream (imageTableName: {1}, recordId: {2})", ex.Message, imageTableName, recordId));
            }
        }

        internal static void DeleteFile(string imageTableName, string fullPath, SqlConnection conn)
        {
            if (!string.IsNullOrEmpty(imageTableName))
            {
                DeleteBlobImage(imageTableName, fullPath, conn);
                return;
            }

            Exporter.DeleteFile(fullPath);
            DeleteCache(fullPath, false);
        }

        internal static void DeleteCache(string fullPath, bool deleteAttachmentThumbsOnly)
        {
            if (string.IsNullOrEmpty(fullPath))
                return;
            string path = Path.GetDirectoryName(fullPath);
            string fileName = string.Format("{0}*.*", Path.GetFileNameWithoutExtension(fullPath));

            if (!deleteAttachmentThumbsOnly)
            {
                Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedPages, fileName)));
                Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedFlyouts, fileName)));
            }

            Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedThumbs, string.Format("a.{0}", fileName))));

            if (!deleteAttachmentThumbsOnly)
            {
                Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedThumbs, fileName)));
                Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedThumbs, string.Format("p.{0}", fileName))));
            }

            if (fileName.StartsWith("."))
                fileName = string.Format("_{0}", fileName.Substring(1));
            Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedThumbs, string.Format("a.{0}", fileName))));

            if (!deleteAttachmentThumbsOnly)
            {
                Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedThumbs, fileName)));
                Exporter.DeleteFiles(Path.Combine(path, Path.Combine(CachedThumbs, string.Format("p.{0}", fileName))));
            }
        }

        internal static void DeleteBlobImage(string imageTableName, string recordId, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(recordId))
                return;

            try
            {
                using (var cmd = new SqlCommand(string.Format(Resources.DeleteImageTableRecords, imageTableName), conn))
                {
                    cmd.Parameters.AddWithValue("@recordId", recordId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: \"{0}\" in Attachment.DeleteBlobImage(imageTableName: {1}, recordId: {2})", ex.Message, imageTableName, recordId));
                
            }
        }

        internal static void UpdatePointerFileName(int pointerId, string newFileName, string imageTableName, int directoryId, int pageNumber, bool isAnImage, SqlConnection conn, int totalPages)
        {
            string sql = Resources.UpdatePCFilesPointerFileName;
            if (isAnImage)
                sql = Resources.UpdateImagePointerFileName;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@fileName", Path.GetFileName(newFileName));
                cmd.Parameters.AddWithValue("@Id", pointerId);
                cmd.Parameters.AddWithValue("@directoryId", directoryId);

                if (isAnImage)
                {
                    cmd.Parameters.AddWithValue("@imageTableName", imageTableName);
                    cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@pageCount", totalPages);
                }

                cmd.ExecuteNonQuery();
            }
        }

        internal static void UpdatePointerFileName(int pointerId, string newFileName, int directoryId, int pageNumber, bool isAnImage, SqlConnection conn)
        {
            UpdatePointerFileName(pointerId, newFileName, string.Empty, directoryId, pageNumber, isAnImage, conn);
        }

        internal static void UpdatePointerFileName(int pointerId, string newFileName, string imageTableName, int directoryId, int pageNumber, bool isAnImage, SqlConnection conn)
        {
            string sql = Resources.UpdatePCFilesPointerFileName;
            if (isAnImage)
                sql = Resources.UpdateImagePointerFileName;

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@fileName", Path.GetFileName(newFileName));
                cmd.Parameters.AddWithValue("@Id", pointerId);
                cmd.Parameters.AddWithValue("@directoryId", directoryId);

                if (isAnImage)
                {
                    int pageCount = 1;
                    Leadtools.Codecs.CodecsImageInfo info = UpdateAttachment.GetCodecInfoFromFile(newFileName, Path.GetExtension(newFileName));
                    if (info is not null)
                        pageCount = info.TotalPages;

                    cmd.Parameters.AddWithValue("@imageTableName", imageTableName);
                    cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@pageCount", pageCount);
                }

                cmd.ExecuteNonQuery();
            }
        }

        public int AddAPages(string fileName, string originalFullPath, string userName, string pcFileExtension, DataRow outputSetting, int pageCount, bool appendToExisting, SqlConnection conn, bool IsAnImage, int totalPages, int height, int width, long sizeOnDisk)
        {
            try
            {
                var update = new UpdateAttachment(outputSetting, userName, pcFileExtension, appendToExisting, conn);
                return update.AddPages(fileName, VersionInfo.TrackablesId, VersionInfo.Version, pageCount, UserID, originalFullPath, IsAnImage, totalPages, height, width, sizeOnDisk);
            }
            catch (Exception ex)
            {
               throw new Exception(string.Format("Error: \"{0}\" in Attachment.AddPages (fileName: {1}, userName: {2}, pcFileExtension: {3}, pageCount: {4})", ex.Message, fileName, userName, pcFileExtension, pageCount));
                
            }
        }

        public int AddPages(string fileName, string originalFullPath, string userName, string pcFileExtension, DataRow outputSetting, int pageCount, bool appendToExisting, SqlConnection conn)
        {
            try
            {
                var update = new UpdateAttachment(outputSetting, userName, pcFileExtension, appendToExisting, conn);
                return update.AddPages(fileName, VersionInfo.TrackablesId, VersionInfo.Version, pageCount, UserID, originalFullPath);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error: \"{0}\" in Attachment.AddPages (fileName: {1}, userName: {2}, pcFileExtension: {3}, pageCount: {4})", ex.Message, fileName, userName, pcFileExtension, pageCount));
                return 0;
            }
        }

        internal void AddImageThumbs(int offset, int startPage, int attachmentNumber, int versionNumber, int count, bool pageThumbnail, int userId, DataRowCollection rows, bool versionGrouping, int itemNumber, string tableName, string tableId, string tableUserName, SqlConnection conn)
        {
            int pointerId = 0;
            var officialRecord = default(bool);
            var officialRecordEnabled = default(bool);
            var checkedOut = default(bool);
            var checkedOutUserId = default(int);
            string fullPath = string.Empty;
            string checkedOutUser = string.Empty;
            string displayText = string.Empty;
            string orgFullPath = string.Empty;
            int directoryId = 0;
            int trackablesId = 0;
            ScanBatchInfo scanBatch = default;
            int thumbWindow = 10;

            int index = offset;
            bool loadNextValues = true;

            if (pageThumbnail && startPage != -1)
            {
                thumbWindow = WordMath.HiWord(startPage);
                startPage = WordMath.LoWord(startPage) + 1;
                if (startPage + thumbWindow < count)
                    count = startPage + thumbWindow;
            }
            else
            {
                startPage = 1;
            }

            for (int currentPage = startPage, loopTo = count; currentPage <= loopTo; currentPage++)
            {
                if (loadNextValues && index < rows.Count)
                {
                    pointerId = (int)rows[index]["PointerId"];
                    checkedOutUser = rows[index]["CheckedOutUser"].ToString();
                    displayText = GetDisplayText(rows[index], conn);
                    if (!(rows[index]["CheckedOutUserId"] is DBNull))
                        checkedOutUserId = (int)rows[index]["CheckedOutUserId"];
                    if (!(rows[index]["CheckedOut"] is DBNull))
                        checkedOut = Convert.ToInt32(rows[index]["CheckedOut"]) != 0;
                    orgFullPath = rows[index]["OrgFullPath"].ToString();
                    directoryId = (int)rows[index]["ScanDirectoriesId"];
                    trackablesId = (int)rows[index]["TrackablesId"];
                    scanBatch = new ScanBatchInfo(rows[index]);

                    if (!(rows[index]["OfficialRecordHandling"] is DBNull))
                        officialRecordEnabled = (bool)rows[index]["OfficialRecordHandling"];
                    if (!(rows[index]["OfficialRecord"] is DBNull) && officialRecordEnabled)
                        officialRecord = Convert.ToInt32(rows[index]["OfficialRecord"]) != 0;

                    try
                    {
                        if (string.IsNullOrEmpty(rows[index]["ImageTableName"].ToString()) && !Information.IsNumeric(rows[index]["FileName"].ToString()))
                        {
                            fullPath = rows[index]["FullPath"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        // do nothing
                    }

                    loadNextValues = false;
                }

                if (currentPage >= GetNextFileFromParts(currentPage))
                {
                    loadNextValues = true;
                    index += 1;
                }

                ThumbsList.Add(new ImageThumb(this, attachmentNumber, versionNumber, currentPage - 1, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, pointerId, versionGrouping, itemNumber, tableName, tableId, false, false, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName, true));
            }
        }

        private int GetNextFileFromParts(int currentPage)
        {
            int pageCount = 0;

            foreach (AttachmentPart part in AttachmentParts)
            {
                if (currentPage <= pageCount)
                    break;
                pageCount += part.PageCount;
            }

            return pageCount;
        }

        internal void AddFileThumbs(int index, int startPage, int attachmentNumber, int versionNumber, int pageNumber, bool pageThumbnail, int userId, DataRowCollection rows, bool versionGrouping, int itemNumber, string tableName, string tableId, string tableUserName, SqlConnection conn)
        {
            string fullPath = rows[index]["FullPath"].ToString();
            if (string.IsNullOrEmpty(fullPath))
                throw new Exception(Permissions.ExceptionString.PathIsEmpty);

            int count = 0;
            int loopCount = 0;
            var officialRecord = default(bool);
            var officialRecordEnabled = default(bool);
            var checkedOutUserId = default(int);
            int pointerId = Conversions.ToInteger(rows[index]["PointerId"]);
            bool checkedOut = !string.IsNullOrEmpty(rows[index]["CheckedOutUserId"].ToString());
            string checkedOutUser = rows[index]["CheckedOutUser"].ToString();
            string checkedOutFolder = rows[index]["CheckedOutFolder"].ToString();
            bool displayNative = GetOverrideSetting(rows[index]["FileName"].ToString(), conn);
            string displayText = GetDisplayText(rows[index], conn);
            string orgFullPath = rows[index]["OrgFullPath"].ToString();
            int directoryId = Conversions.ToInteger(rows[index]["ScanDirectoriesId"]);
            int trackablesId = Conversions.ToInteger(rows[index]["TrackablesId"]);
            var scanBatch = new ScanBatchInfo(rows[index]);

            if (!string.IsNullOrEmpty(rows[index]["CheckedOutUserId"].ToString()))
                checkedOutUserId = Conversions.ToInteger(rows[index]["CheckedOutUserId"]);
            if (!string.IsNullOrEmpty(rows[index]["CheckedOut"].ToString()))
                checkedOut = Conversions.ToInteger(rows[index]["CheckedOut"]) != 0;
            if (!(rows[index]["OfficialRecordHandling"] is DBNull))
                officialRecordEnabled = Conversions.ToBoolean(rows[index]["OfficialRecordHandling"]);
            if (!(rows[index]["OfficialRecord"] is DBNull) && officialRecordEnabled)
                officialRecord = Conversions.ToInteger(rows[index]["OfficialRecord"]) != 0;

            string cachedFolder = Path.Combine(Path.GetDirectoryName(fullPath), CachedThumbs);
            FileNames = GetCachedFileThumbs(cachedFolder, fullPath, pageThumbnail);

            while (loopCount < 2)
            {
                try
                {
                    if (FileNames.Count == 0)
                    {
                        using (var export = new Exporter())
                        {
                            if (pageThumbnail)
                            {
                                var argexport = export;
                                ExportToByteArray(ref argexport, fullPath, cachedFolder, -99, -99, false, displayNative, true, pageThumbnail);
                            }
                            else
                            {
                                var argexport1 = export;
                                ExportToByteArray(ref argexport1, fullPath, cachedFolder, 1, 2, false, displayNative, true, pageThumbnail);
                            }
                        }

                        if (FileNames.Count == 0)
                            FileNames = GetCachedFileThumbs(cachedFolder, fullPath, pageThumbnail);
                    }

                    if (FileNames.Count > 0)
                    {
                        count = 1;
                        if (pageThumbnail)
                            count = FileNames.Count;
                    }

                    AttachmentParts[AttachmentParts.Count - 1].ResetPageCount(Math.Max(FileNames.Count, 1));
                    AddFileThumbs(startPage, count, attachmentNumber, versionNumber, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, pointerId, checkedOutFolder, versionGrouping, tableName, tableId, displayNative, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName, loopCount);
                    loopCount += 2;
                }
                catch (Leadtools.RasterException ex)
                {
                    if (string.Compare(ex.Message, "invalid file format", true) == 0)
                    {
                        foreach (string file in FileNames)
                            Exporter.DeleteFile(file);

                        FileNames = new List<string>();
                        loopCount += 1;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            FileNames = null;
        }

        private void ExportToByteArray(ref Exporter export, string fullPath, string destinationPath, int startPage, int endPage, bool returnValue, bool displayNative, bool thumbnail, bool pageThumbnail)
        {
            try
            {
                byte[] byteArray = null;

                if (startPage == -99 && endPage == -99)
                {
                    byteArray = export.ToByteArray(fullPath, destinationPath, OutputFormat, returnValue, displayNative, thumbnail, pageThumbnail);
                }
                else
                {
                    byteArray = export.ToByteArray(fullPath, destinationPath, OutputFormat, startPage, endPage, returnValue, displayNative, thumbnail, pageThumbnail);
                }

                if (byteArray is not null)
                    FileNames.Add(Output.ByteArrayToString(byteArray));
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);

                if (FileNames is null)
                    FileNames = new List<string>();
                FileNames.Add(fullPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void AddFileThumbs(int startPage, int count, int attachmentNumber, int versionNumber, bool pageThumbnail, bool checkedOut, string checkedOutUser, int checkedOutUserId, bool officialRecord, string fullPath, int pointerId, string checkedOutFolder, bool versionGrouping, string tableName, string tableId, bool displayNative, string displayText, string orgFullPath, int directoryId, ScanBatchInfo scanBatch, int trackablesId, bool officialRecordEnabled, string tableUserName, int errorCount)
        {
            int thumbWindow = 10;

            if (pageThumbnail && startPage != -1)
            {
                thumbWindow = WordMath.HiWord(startPage);
                startPage = WordMath.LoWord(startPage);
                if (startPage + thumbWindow < count)
                    count = startPage + thumbWindow;
            }
            else
            {
                startPage = 0;
            }

            for (int i = startPage, loopTo = count - 1; i <= loopTo; i++)
            {
                RasterImage img = default;

                if (string.Compare(FileNames[i], Permissions.ExceptionString.FileTypeExcluded, true) == 0)
                {
                    ThumbsList.Add(new FileThumb(img, this, attachmentNumber, versionNumber, i, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, pointerId, checkedOutFolder, versionGrouping, i + 1, tableName, tableId, displayNative, true, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName));
                    if (i < 1)
                        PagesList.Add(new FilePage(OutputFormat));
                }
                else
                {
                    try
                    {
                        var codec = new RasterCodecs();
                        CodecsImageInfo info = codec.GetInformation(FileNames[i], false, 1);
                        var rc = new System.Drawing.Rectangle(0, 0, ThumbSize.Width, ThumbSize.Height);
                        //rc = RasterImageList.GetFixedAspectRatioImageRectangle(info.Width, info.Height, rc);
                        rc.Width = info.Width;
                        rc.Height = info.Height;
                        if (info.BitsPerPixel > 4)
                        {
                            img = new RasterImage(codec.Load(FileNames[i], rc.Width, rc.Height, info.BitsPerPixel, RasterSizeFlags.Resample, CodecsLoadByteOrder.RgbOrGray, 1, 1));
                        }
                        else
                        {
                            img = new RasterImage(codec.Load(FileNames[i], 0, CodecsLoadByteOrder.RgbOrGray, 1, 1));
                        }

                        if (info.Width != rc.Width || info.Height != rc.Height)
                        {
                            string path = Path.GetDirectoryName(FileNames[i]);
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            codec.Save(img, FileNames[i], TranslateToLeadToolsFormat(img.BitsPerPixel), img.BitsPerPixel);
                        }

                        ThumbsList.Add(new FileThumb(img, this, attachmentNumber, versionNumber, i, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, pointerId, checkedOutFolder, versionGrouping, i + 1, tableName, tableId, displayNative, true, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName));
                    }

                    catch (Exception ex)
                    {
                        if (string.Compare(ex.Message, Permissions.ExceptionString.FileNotFound, true) == 0 || errorCount > 0)
                        {
                            using (var ms = new MemoryStream(Output.ImageToByteArray(Resources.InvalidThumb)))
                            {
                                img = new RasterCodecs().Load(ms);
                            }
                        }
                        else if (ex is Leadtools.RasterException)
                        {
                            using (var ms = new MemoryStream(Output.ImageToByteArray(Resources.InvalidThumb)))
                            {
                                img = new RasterCodecs().Load(ms);
                            }
                        }

                        throw new Exception(string.Format("Error \"{0}\" in Attachment.AddFileThumbs (fileName: {1})", ex.Message, FileNames[i]));
                        if (img is not null)
                            ThumbsList.Add(new FileThumb(img, this, attachmentNumber, versionNumber, i, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, pointerId, checkedOutFolder, versionGrouping, i + 1, tableName, tableId, false, false, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName));
                    }

                    if (i < 1)
                        PagesList.Add(new FilePage(OutputFormat));
                }
            }
        }

        private List<string> GetCachedFileThumbs(string fullPath, string fileName, bool pageThumbnail)
        {
            var rtn = new List<string>();
            if (!File.Exists(fileName))
                return rtn;

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (fileName.StartsWith("."))
                fileName = string.Format("_{0}", fileName.Substring(1));

            if (pageThumbnail)
            {
                fileName = string.Format("p.{0}", fileName);
            }
            else
            {
                fileName = string.Format("a.{0}", fileName);
            }

            var files = Directory.GetFiles(fullPath, string.Format("{0}*.{1}", fileName, OutputFormat.ToString()));

            if (files is not null && files.Length > 0)
            {
                foreach (string file in files)
                    rtn.Add(file);
            }

            return rtn;
        }

        internal RasterImage GetCachedImageThumb(ref string fileName, int pageNumber, bool pageThumbnail)
        {
            string path = Path.Combine(Path.GetDirectoryName(fileName), CachedThumbs);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string extension = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if (fileName.StartsWith("."))
                fileName = string.Format("_{0}", fileName.Substring(1));

            if (pageThumbnail)
            {
                fileName = string.Format("p.{0}", fileName);
            }
            else
            {
                fileName = string.Format("a.{0}", fileName);
            }

            string part = string.Empty;
            if (pageNumber > 0)
                part = pageNumber.ToString("x4");
            fileName = Path.Combine(path, string.Format("{0}{1}{2}", fileName, part, extension));
            if (!File.Exists(fileName))
                return default;

            try
            {
                var codec = new RasterCodecs();
                CodecsImageInfo info = codec.GetInformation(fileName, false, 1);

                var rc = new System.Drawing.Rectangle(0, 0, ThumbSize.Width, ThumbSize.Height);
                //rc = RasterImageList.GetFixedAspectRatioImageRectangle(info.Width, info.Height, rc);
                rc.Width = info.Width;
                rc.Height = info.Height;
                return new RasterImage(codec.Load(fileName, rc.Width, rc.Height, info.BitsPerPixel, RasterSizeFlags.Resample, CodecsLoadByteOrder.RgbOrGray, 1, 1));
            }
            catch (Exception ex)
            {
                //Logs.LoginError(string.Format("\"{0}\" in Attachment.GetCachedImageThumb", ex.Message));
                return default;
            }
        }

        internal RasterImage GetCachedImage(string filename, int pageNumber, string cachedFolder, Output.Format format, string annotations, bool throwException)
        {
            string cachedFileName = Path.Combine(Path.GetDirectoryName(filename), cachedFolder);
            if (!Directory.Exists(cachedFileName))
                Directory.CreateDirectory(cachedFileName);
            filename = Path.GetFileNameWithoutExtension(filename);

            string part = string.Empty;
            if (pageNumber > 0)
                part = pageNumber.ToString("x4");
            cachedFileName = Path.Combine(cachedFileName, string.Format("{0}{1}.{2}", filename, part, format.ToString().ToLower()));
            if (!File.Exists(cachedFileName))
                return default;

            try
            {
                if (!string.IsNullOrEmpty(annotations))
                {
                    if (cachedFileName.ToLower().Contains(Path.DirectorySeparatorChar + CachedFlyouts.ToLower() + Path.DirectorySeparatorChar) || cachedFileName.ToLower().Contains(Path.AltDirectorySeparatorChar + CachedFlyouts.ToLower() + Path.AltDirectorySeparatorChar))
                    {
                        var info = new FileInfo(cachedFileName);
                        if (info.LastWriteTime < _scaleToGrayDate)
                            return default;
                    }
                }

                using (var fs = new FileStream(cachedFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                {
                    return new RasterCodecs().Load(fs);
                    // Return New RasterImage(New Drawing.Bitmap(fs))
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("\"{0}\" in Attachment.GetCachedImage", ex.Message);
                //Logs.LoginError(message);
                if (throwException)
                    throw new Exception(message);
                return default;
            }
        }

        internal void SaveCachedFlyout(RasterImage img, string fileName, int pageNumber, Output.Format format, bool isAnImage, string annotations)
        {
            if (img is null || string.IsNullOrEmpty(fileName))
                return;

            ImageViewer imageViewer = default;

            try
            {
                var codec = new RasterCodecs();
                string part = string.Empty;
                if (pageNumber > 0)
                    part = pageNumber.ToString("x4");
                string cachedFileName = fileName;

                if (!isAnImage)
                {
                    cachedFileName = Path.Combine(Path.GetDirectoryName(fileName), CachedPages);
                    if (!Directory.Exists(cachedFileName))
                        Directory.CreateDirectory(cachedFileName);
                    cachedFileName = Path.Combine(cachedFileName, string.Format("{0}{1}.{2}", Path.GetFileNameWithoutExtension(fileName), part, format.ToString().ToLower()));
                    if (!File.Exists(cachedFileName))
                        return;
                }

                string flyoutFileName = Path.Combine(Path.GetDirectoryName(fileName), CachedFlyouts);
                if (!Directory.Exists(flyoutFileName))
                    Directory.CreateDirectory(flyoutFileName);
                flyoutFileName = Path.Combine(flyoutFileName, string.Format("{0}{1}.{2}", Path.GetFileNameWithoutExtension(fileName), part, format.ToString().ToLower()));

                if (File.Exists(flyoutFileName))
                {
                    if (string.IsNullOrEmpty(annotations))
                        return;
                    var info = new FileInfo(cachedFileName);
                    if (info.LastWriteTime >= _scaleToGrayDate)
                        return;
                }

                var rc = new System.Drawing.Rectangle(0, 0, FlyoutSize.Width, FlyoutSize.Height);

                if (img.Width < FlyoutSize.Width || img.Height < FlyoutSize.Height)
                {
                    rc.Width = img.Width;
                    rc.Height = img.Height;
                }

                //rc = RasterImageList.GetFixedAspectRatioImageRectangle(img.Width, img.Height, rc);

                using (var newImg = new RasterImage(codec.Load(cachedFileName, 24, TranslateCodecsLoadByteOrder(img.Order), 1, 1)))
                {
                    imageViewer = new ImageViewer();
                    imageViewer.AutoDisposeImages = true;
                    imageViewer.Image = newImg;
                    Page.RealizeRedactions(imageViewer, annotations, Attachments.AnnotationsDrawMode.RedactionOnly);

                    var resizeCmd = new Leadtools.ImageProcessing.ResizeCommand();
                    resizeCmd.Flags = RasterSizeFlags.Resample;
                    resizeCmd.DestinationImage = new RasterImage(RasterMemoryFlags.User, rc.Width, rc.Height, 24, img.Order, img.ViewPerspective, null, null, 0, null);
                    resizeCmd.Run(imageViewer.Image);
                    codec.Save(resizeCmd.DestinationImage, flyoutFileName, RasterImageFormat.Jpeg, 24);
                    resizeCmd = default;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("\"{0}\" in Attachment.SaveCachedFlyout", ex.Message));
            }

            if (imageViewer is not null)
            {
                if (imageViewer.Image is not null)
                    imageViewer.Image.Dispose();
                imageViewer.Image.Dispose();
                imageViewer = default;
            }
        }

        public static int ConvertBitsPerPixel(Output.Format format, int bitsPerPixel)
        {
            switch (format)
            {
                case var @case when @case == Output.Format.Bmp:
                    {
                        if (bitsPerPixel < 4)
                            return 24;
                        return bitsPerPixel;
                    }
                case var case1 when case1 == Output.Format.Gif:
                    {
                        return 8;
                    }
                case var case2 when case2 == Output.Format.Png:
                    {
                        return 24;
                    }
                case var case3 when case3 == Output.Format.Tif:
                    {
                        return bitsPerPixel;
                    }

                default:
                    {
                        return 24;
                    }
            }
        }

        private RasterImageFormat TranslateToLeadToolsFormat(int bitsPerPixel)
        {
            return TranslateToLeadToolsFormat(OutputFormat, bitsPerPixel);
        }

        internal RasterImageFormat TranslateToLeadToolsFormat(Output.Format format, int bitsPerPixel)
        {
            switch (format)
            {
                case var @case when @case == Output.Format.Bmp:
                    {
                        return RasterImageFormat.Bmp;
                    }
                case var case1 when case1 == Output.Format.Gif:
                    {
                        return RasterImageFormat.Gif;
                    }
                case var case2 when case2 == Output.Format.Png:
                    {
                        return RasterImageFormat.Png;
                    }
                case var case3 when case3 == Output.Format.Tif:
                    {
                        if (bitsPerPixel <= 2)
                            return RasterImageFormat.CcittGroup4;
                        return RasterImageFormat.Tif;
                    }

                default:
                    {
                        return RasterImageFormat.Jpeg;
                    }
            }
        }

        private CodecsLoadByteOrder TranslateCodecsLoadByteOrder(RasterByteOrder order)
        {
            switch (order)
            {
                case var @case when @case == RasterByteOrder.Bgr:
                    {
                        return CodecsLoadByteOrder.BgrOrGray;
                    }

                default:
                    {
                        return CodecsLoadByteOrder.RgbOrGray;
                    }
            }
        }

        private string GetDisplayText(DataRow row, SqlConnection conn)
        {
            if (DisplayType == DisplayTypes.Undefined)
            {
                try
                {
                    var cmd = new SqlCommand(Resources.GetSettings, conn);
                    cmd.Parameters.AddWithValue("@section", "Attachments");
                    cmd.Parameters.AddWithValue("@item", "ThumbText");
                    string value = cmd.ExecuteScalar().ToString();
                    if (!string.IsNullOrEmpty(value) && Information.IsNumeric(value))
                        _displayType = (DisplayTypes)Enum.Parse(typeof(DisplayTypes), value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            string rtn = string.Empty;
            string displayText = string.Empty;

            if (!(row["OrgFileName"] is DBNull) && !string.IsNullOrEmpty(row["OrgFileName"].ToString()))
                displayText = row["OrgFileName"].ToString();

            switch (DisplayType)
            {
                case DisplayTypes.DisplayFields:
                case DisplayTypes.DisplayFieldsIncludeInFooter:
                    {
                        bool orphan = false;
                        if (!(row["Orphan"] is DBNull))
                            orphan = Convert.ToInt32(row["Orphan"]) != 0;
                        if (!orphan)
                            rtn = Attachments.GetDisplayFields(row, conn);
                        break;
                    }
                // if orphan, rtn is left Empty and display is handled by viewer
                case DisplayTypes.FileName:
                case DisplayTypes.FileNameIncludeInFooter:
                    {
                        // if OrgFullPath is empty, rtn is left Empty and display is handled by viewer
                        if (!(row["OrgFullPath"] is DBNull) && !string.IsNullOrEmpty(row["OrgFullPath"].ToString()))
                            rtn = Path.GetFileName(row["OrgFullPath"].ToString());
                        break;
                    }

                default:
                    {
                        break;
                    }
                    // rtn is left Empty and display is handled by viewer
            }

            if (string.IsNullOrEmpty(displayText))
                return rtn;
            return string.Format("{0}{1}{2}", displayText, DisplayTextDivider, rtn);
        }

        private bool GetOverrideSetting(string fileName, SqlConnection conn)
        {
            if (!(this is FileAttachment) || string.IsNullOrEmpty(fileName))
                return false;

            string extension = Path.GetExtension(fileName).Replace(".", string.Empty).Trim();

            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    var cmd = new SqlCommand(Resources.GetSettings, conn);
                    cmd.Parameters.AddWithValue("@section", "UseNativeApp");
                    cmd.Parameters.AddWithValue("@item", extension);
                    string value = cmd.ExecuteScalar().ToString();

                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!Information.IsNumeric(value))
                            return Conversions.ToBoolean(value);
                        return Conversions.ToInteger(value) != 0;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    // do nothing, check for all (*)
                }

                extension = "*";
            }

            return false;
        }

        #region  IDisposable Support 
        private bool disposedValue = false;        // To detect redundant calls
                                                   // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    while (PagesList.Count > 0)
                    {
                        Page pg = PagesList[0];
                        PagesList.RemoveAt(0);
                        pg.Dispose();
                        pg = default;
                    }
                    while (ThumbsList.Count > 0)
                    {
                        Thumb th = ThumbsList[0];
                        ThumbsList.RemoveAt(0);
                        th.Dispose();
                        th = default;
                    }
                }
            }
            disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
