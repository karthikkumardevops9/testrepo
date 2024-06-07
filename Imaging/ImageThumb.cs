using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using Leadtools;
using Leadtools.Codecs;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using MSRecordsEngine.Properties;


namespace MSRecordsEngine.Imaging
{
    [Serializable()]
    public partial class ImageThumb : Thumb
    {

        private ImageThumb() : base()
        {
        }

        public ImageThumb(Attachment attachment, int attachmentNumber, int versionNumber, int pageNumber, bool pageThumbnail, bool checkedOut, string checkedOutUser, int checkedOutUserId, bool officialRecord, string fullPath, int pointerId, bool versionGrouping, int itemNumber, string tableName, string tableId, bool displayNative, bool useOverlay, string displayText, string orgFullPath, int directoryId, ScanBatchInfo scanBatch, int trackablesId, bool officialRecordEnabled, string tableUserName, bool inThumbWindow) : base(attachment, attachmentNumber, versionNumber, pageNumber, pageThumbnail, checkedOut, checkedOutUser, checkedOutUserId, officialRecord, fullPath, string.Empty, pointerId, versionGrouping, itemNumber, tableName, tableId, displayNative, useOverlay, displayText, orgFullPath, directoryId, scanBatch, trackablesId, officialRecordEnabled, tableUserName, inThumbWindow)
        {
        }

        internal override void GetThumb(Attachment attachment, int pageNumber, int pointerId, bool versionGrouping, string tableName, string tableId, bool useOverlay, bool pageThumbnail, bool inThumbWindow)
        {
            var offset = default(int);

            _useOverlay = false;
            _pageNumber = pageNumber;
            int groupingNumber = AttachmentNumber;
            if (versionGrouping)
                groupingNumber = VersionNumber;
            // If versionGrouping And Not attachment.VersionInfo.Orphan Then groupingNumber = VersionNumber
            if (!inThumbWindow)
                return;

            foreach (AttachmentPart part in attachment.AttachmentParts)
            {
                if (part.GroupingNumber == groupingNumber & string.Compare(part.TableName, tableName, true) == 0 & string.Compare(part.TableID, tableId, true) == 0)
                {
                    if (part.PageCount + offset > _pageNumber)
                    {
                        LoadData(attachment, part, pageNumber - offset, pointerId, pageThumbnail);
                        break;
                    }

                    offset += part.PageCount;
                }
            }
        }

        private void LoadData(Attachment attachment, AttachmentPart attachmentPart, int pageNumber, int pointerId, bool pageThumbnail)
        {
            if (_image is not null && !string.IsNullOrEmpty(Annotations))
                return;

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
                using (SqlConnection conn = Attachments.GetConnection(attachment.DatabaseName))
                {
                    if (string.IsNullOrEmpty(Annotations))
                        Annotations = Attachments.GetAnnotation(Attachments.AttachmentTypes.tkImage, pageNumber + 1, pointerId, conn);
                    if (_image is not null)
                        return;

                    if (!string.IsNullOrEmpty(attachmentPart.ImageTableName))
                    {
                        LoadDatabaseThumb(attachment, attachmentPart, pageNumber, conn);
                    }
                    else
                    {
                        LoadThumb(attachment, attachmentPart.FullPath, pageNumber, pageThumbnail);
                    }
                }
            }
            catch (Exception ex)
            {
                // need more info logged to help determine cause
                // _image = Page.RasterImageToByteArray(New RasterImage(My.Resources.InvalidThumb))
                if (!string.IsNullOrEmpty(attachmentPart.ImageTableName))
                {
                    //Logs.LoginError(string.Format("Error \"{0}\" in ImageThumb.LoadData", ex.Message));
                }
                else
                {
                    //Logs.LoginError(string.Format("Error \"{0}\" in ImageThumb.LoadData (fileName: {1})", ex.Message, attachmentPart.FullPath));
                }
                using (var ms = new MemoryStream(Output.ImageToByteArray(Resources.InvalidThumb)))
                {
                    _image = new RasterCodecs().Load(ms);
                }

            }
        }

        private void LoadThumb(Attachment attachment, string fileName, int pageNumber, bool pageThumbnail)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception(Permissions.ExceptionString.PathIsEmpty);
            if (!File.Exists(fileName))
                throw new Exception(Permissions.ExceptionString.FileNotFound, new Exception(fileName));

            var codec = new RasterCodecs();
            CodecsImageInfo info = codec.GetInformation(fileName, false, pageNumber + 1);
            bool isAPCFile = UpdateAttachment.IsAPCFile(info.Format, Path.GetExtension(fileName));
            string cachedFileName = fileName;

            if (!isAPCFile)
            {
                OriginalHeight = info.Height;
                OriginalWidth = info.Width;
            }

            _image = attachment.GetCachedImageThumb(ref cachedFileName, pageNumber, pageThumbnail);
            if (_image is not null || isAPCFile)
                return;

            if (info.BitsPerPixel > 4)
            {
                var rc = new Rectangle(0, 0, attachment.ThumbSize.Width, attachment.ThumbSize.Height);
                //rc = RasterImageList.GetFixedAspectRatioImageRectangle(info.Width, info.Height, rc);
                rc.Width = info.Width;
                rc.Height = info.Height;
                _image = new RasterImage(codec.Load(fileName, rc.Width, rc.Height, info.BitsPerPixel, RasterSizeFlags.Resample,CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1));
                codec.Save(_image, cachedFileName, _image.OriginalFormat, _image.BitsPerPixel);
            }
            else
            {
                _image = new RasterImage(codec.Load(fileName, 0, CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1));
            }
        }

        private void LoadDatabaseThumb(Attachment attachment, AttachmentPart part, int pageNumber, SqlConnection conn)
        {
            if (string.IsNullOrEmpty(part.FileName) || !Information.IsNumeric(part.FileName))
                throw new Exception(Permissions.ExceptionString.InvalidImage);

            DataTable dt = Attachments.FillDataTable(string.Format(Resources.GetDatabaseImage, part.ImageTableName), conn, "@recordId", part.FileName);
            if (dt.Rows.Count == 0)
                throw new Exception(Permissions.ExceptionString.ImageNotFound);

            byte[] byteArray = (byte[])dt.Rows[0]["ImageField"];
            string fileExtension = dt.Rows[0]["FileExtension"].ToString();
            if (!string.IsNullOrEmpty(fileExtension))
            {
                if (!fileExtension.StartsWith("."))
                    fileExtension = string.Format(".{0}", fileExtension);
                Extension = fileExtension;
            }
            var codec = new RasterCodecs();
            // _image = Page.RasterImageToByteArray(New RasterImage(Attachments.Codec.Load(New System.IO.MemoryStream(byteArray), 0, Codecs.CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1)))
            _image = new RasterImage(codec.Load(new MemoryStream(byteArray), 0, CodecsLoadByteOrder.RgbOrGray, pageNumber + 1, pageNumber + 1));
        }
    }
}
