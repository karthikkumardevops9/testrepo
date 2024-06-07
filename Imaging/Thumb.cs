using System;
using System.IO;
using System.Runtime.Serialization;
using Leadtools;
using Smead.Security;
/// <summary>
/// A thumbnail image of a single page.
/// </summary>
/// <remarks>Thumbs are generated when an attachment is saved  </remarks>
namespace MSRecordsEngine.Imaging
{
    [DataContract()]
    public abstract partial class Thumb : IDisposable
    {

        internal abstract void GetThumb(Attachment attachment, int pageNumber, int pointerId, bool versionGrouping, string tableName, string tableId, bool useOverlay, bool pageThumbnail, bool inThumbWindow);

        protected Thumb()
        {
            _pageNumber = -1;
        }

        protected Thumb(Attachment attachment, int attachmentNumber, int versionNumber, int pageNumber, bool pageThumbnail, bool checkedOut, string checkedOutUser, int checkedOutUserId, bool officialRecord, string fullPath, string checkedOutFolder, int pointerId, bool versionGrouping, int itemNumber, string tableName, string tableId, bool displayNative, bool useOverlay, string displayText, string orgFullPath, int directoryId, ScanBatchInfo scanBatch, int trackablesId, bool officialRecordEnabled, string tableUserName, bool inThumbWindow) : this()
        {
            _attachmentNumber = attachmentNumber;
            _versionNumber = versionNumber;
            if (attachment.VersionInfo.Orphan)
                _versionNumber = attachmentNumber;

            if (attachment.VersionInfo.Orphan)
            {
                _itemNumber = itemNumber;
            }
            else if (versionGrouping)
            {
                _itemNumber = _versionNumber;
            }
            // ElseIf pageThumbnail Then
            // _itemNumber = pageNumber
            else
            {
                _itemNumber = _attachmentNumber;
            }

            _displayText = displayText;
            _displayNative = displayNative;
            _checkedOut = checkedOut;
            _checkedOutUser = checkedOutUser;
            _checkedOutUserId = checkedOutUserId;
            _checkedOutFolder = checkedOutFolder;
            _officialRecord = officialRecord;
            OfficialRecordEnabled = officialRecordEnabled;
            _extension = Path.GetExtension(fullPath);
            _fullPath = fullPath;
            _tableName = tableName;
            _tableUserName = tableUserName;
            _tableId = tableId;
            _pointerId = pointerId;
            Security = attachment.SecurityInfo;
            OrgFullPath = orgFullPath;
            DirectoryId = directoryId;
            ScanBatch = scanBatch;
            TrackableId = trackablesId;
            _ticket = Encrypt.HashTicket(attachment.UserID, attachment.DatabaseName, tableName, Attachments.StripLeadingZeros(tableId));

            if (versionGrouping)
            {
                _totalPages = attachment.get_PageCount(_versionNumber, versionGrouping, tableName, tableId);
            }
            else
            {
                _totalPages = attachment.get_PageCount(attachmentNumber, tableName, tableId);
            }

            _multiPage = !pageThumbnail & _totalPages > 1;
            if (_totalPages == 0)
                _totalPages = attachment.get_PageCount(attachmentNumber, tableName, tableId);
            GetThumb(attachment, pageNumber, pointerId, versionGrouping, tableName, tableId, useOverlay, pageThumbnail, inThumbWindow);
        }

        [DataMember()]
        public bool CheckedOut
        {
            get
            {
                return _checkedOut;
            }
            set
            {
                _checkedOut = value;
            }
        }
        private bool _checkedOut = false;

        [DataMember()]
        public string CheckedOutUser
        {
            get
            {
                return _checkedOutUser;
            }
            set
            {
                _checkedOutUser = value;
            }
        }
        private string _checkedOutUser = string.Empty;

        [DataMember()]
        public int CheckedOutUserId
        {
            get
            {
                return _checkedOutUserId;
            }
            set
            {
                _checkedOutUserId = value;
            }
        }
        private int _checkedOutUserId;

        [DataMember()]
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
        private int _attachmentNumber;

        [DataMember()]
        public int VersionNumber
        {
            get
            {
                return _versionNumber;
            }
            set
            {
                _versionNumber = value;
            }
        }
        private int _versionNumber;

        [DataMember()]
        public int ItemNumber
        {
            get
            {
                return _itemNumber;
            }
            set
            {
                _itemNumber = value;
            }
        }
        private int _itemNumber;

        [DataMember()]
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }
        private string _tableName;

        [DataMember()]
        public string TableID
        {
            get
            {
                return _tableId;
            }
            set
            {
                _tableId = value;
            }
        }
        private string _tableId;

        [DataMember()]
        public string Ticket
        {
            get
            {
                return _ticket;
            }
            set
            {
                _ticket = value;
            }
        }
        private string _ticket;

        [DataMember()]
        public byte[] Image
        {
            get
            {
                try
                {
                    return Page.RasterImageToByteArray(_image);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error \"{0}\" in Thumb.Image", ex.Message));
                    
                }
            }
            set
            {
                _image = Page.ByteArrayToRasterImage(value);
            }
        }
        protected RasterImage _image = default;

        [DataMember()]
        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
            set
            {
                _pageNumber = value;
            }
        }
        protected int _pageNumber;

        [DataMember()]
        public int TotalPages
        {
            get
            {
                return _totalPages;
            }
            set
            {
                _totalPages = value;
            }
        }
        private int _totalPages;

        [DataMember()]
        public bool MultiPage
        {
            get
            {
                return _multiPage;
            }
            set
            {
                _multiPage = value;
            }
        }
        private bool _multiPage = false;

        [DataMember()]
        public bool OfficialRecord
        {
            get
            {
                return _officialRecord;
            }
            set
            {
                _officialRecord = value;
            }
        }
        private bool _officialRecord = false;
        /// <summary>
        /// Gets whether this thumb is redacted or not.
        /// </summary>
        /// <remarks>??? There may need to be a third state for n/a to be used with PCFiles.</remarks>
        [DataMember()]
        public bool Redacted
        {
            get
            {
                return _redacted;
            }
            set
            {
                _redacted = value;
            }
        }
        private bool _redacted;

        [DataMember()]
        public string Annotations
        {
            get
            {
                return _annotations;
            }
            set
            {
                _annotations = value;
            }
        }
        private string _annotations = string.Empty;

        [DataMember()]
        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }
        private string _extension = string.Empty;

        [DataMember()]
        public bool UseOverlay
        {
            get
            {
                return _useOverlay;
            }
            set
            {
                _useOverlay = value;
            }
        }
        protected bool _useOverlay;

        [DataMember()]
        public string CheckedOutFolder
        {
            get
            {
                return _checkedOutFolder;
            }
            set
            {
                _checkedOutFolder = value;
            }
        }
        private string _checkedOutFolder = string.Empty;

        [DataMember()]
        public string FullPath
        {
            get
            {
                return _fullPath;
            }
            set
            {
                _fullPath = value;
            }
        }
        private string _fullPath = string.Empty;

        [DataMember()]
        public int OriginalHeight
        {
            get
            {
                return _originalHeight;
            }
            set
            {
                _originalHeight = value;
            }
        }
        private int _originalHeight;

        [DataMember()]
        public int OriginalWidth
        {
            get
            {
                return _originalWidth;
            }
            set
            {
                _originalWidth = value;
            }
        }
        private int _originalWidth;

        public bool DisplayNative
        {
            get
            {
                return _displayNative;
            }
        }
        [DataMember()]
        private bool _displayNative;

        [DataMember()]
        public int PointerId
        {
            get
            {
                return _pointerId;
            }
            set
            {
                _pointerId = value;
            }
        }
        private int _pointerId;

        public SecurityInfo SecurityInfo
        {
            get
            {
                return Security;
            }
        }
        [DataMember()]
        private SecurityInfo Security = new SecurityInfo();

        [DataMember()]
        public string DisplayText
        {
            get
            {
                return _displayText;
            }
            set
            {
                _displayText = value;
            }
        }
        private string _displayText = string.Empty;

        public int DirectoriesId
        {
            get
            {
                return DirectoryId;
            }
        }
        [DataMember()]
        private int DirectoryId;

        public string OrigFullPath
        {
            get
            {
                return OrgFullPath;
            }
        }
        [DataMember()]
        private string OrgFullPath = string.Empty;

        public ScanBatchInfo ScanBatchInfo
        {
            get
            {
                return ScanBatch;
            }
        }
        [DataMember()]
        private ScanBatchInfo ScanBatch = new ScanBatchInfo();

        public int TrackablesId
        {
            get
            {
                return TrackableId;
            }
        }
        [DataMember()]
        private int TrackableId;

        public bool OfficialRecordIsEnabled
        {
            get
            {
                return OfficialRecordEnabled;
            }
        }
        [DataMember()]
        private bool OfficialRecordEnabled = false;

        [DataMember()]
        public string TableUserName
        {
            get
            {
                return _tableUserName;
            }
            set
            {
                _tableName = value;
            }
        }
        private string _tableUserName;

        #region  IDisposable Support 
        private bool disposedValue = false;        // To detect redundant calls
                                                   // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_image is not null)
                        _image.Dispose();
                    _image = null;
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