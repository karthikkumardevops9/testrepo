using System;
using System.IO;
using Leadtools;
using Leadtools.Codecs;
using MSRecordsEngine.Properties;
namespace MSRecordsEngine.Imaging
{
    [Serializable()]
    public partial class FileThumb : Thumb
    {

        private FileThumb() : base()
        {
        }

        public FileThumb(int pageNumber) : this()
        {
            _pageNumber = pageNumber;
        }

        public FileThumb(RasterImage image, Attachment attachment, int attachmentNumber, int versionNumber, int pageNumber, bool pageThumbnail, bool checkedOut, string checkedOutUser, int checkedOutUserId, bool officialRecord, string fullPath, int pointerId, string checkedOutFolder, bool versionGrouping, int itemNumber, string tableName, string tableId, bool displayNative, bool useOverlay, string displayText, string orgFullPath, int directoryId, ScanBatchInfo scanBatch, int trackablesId, bool officialRecordEnabled, string tableUserName) : base(attachment, attachmentNumber, versionNumber, pageNumber, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, checkedOutFolder, pointerId, versionGrouping, itemNumber, tableName, tableId, displayNative, useOverlay, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName, false)
        {
            LoadThumb(image, attachment, pageNumber, versionGrouping, tableName, tableId, pageThumbnail);
        }

        internal override void GetThumb(Attachment attachment, int pageNumber, int pointerId, bool versionGrouping, string tableName, string tableId, bool useOverlay, bool pageThumbnail, bool inThumbWindow)
        {
            _useOverlay = useOverlay;
            _pageNumber = pageNumber;
        }

        internal void LoadThumb(RasterImage image, Attachment attachment, int pageNumber, bool versionGrouping, string tableName, string tableId, bool pageThumbnail)
        {
            if (_image is not null)
                return;
            GetThumb(attachment, pageNumber, 0, versionGrouping, tableName, tableId, _useOverlay, pageThumbnail, true);

            if (string.IsNullOrEmpty(attachment.DatabaseName) || !true)
            {
                using (var ms = new MemoryStream(Output.ImageToByteArray(Resources.InvalidThumb)))
                {
                    _image = new RasterCodecs().Load(ms);
                }
                return;
            }

            try
            {
                _image = image;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error \"{0}\" in FileThumb.LoadThumb", ex.Message));
                using (var ms = new MemoryStream(Output.ImageToByteArray(Resources.InvalidThumb)))
                {
                    _image = new RasterCodecs().Load(ms);
                }
            }
        }
    }
}
